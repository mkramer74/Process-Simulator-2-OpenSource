﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using API;
using Snap7;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Utils;
using Utils.Logger;
using Utils.Segmentation;

namespace Connection.S7IsoTCP
{
    public class Connection : IConnection, IDisposable
    {
        public S7Client                 mClient                 = new S7Client();
        public string                   mIP                     = "127.0.0.1";
        private int                     mRack                   = 0;
        public int                      Rack
        {
            get { return mRack;  }
            set
            {
                mRack = value;
            }
        }
        private int                     mSlot                   = 0;
        public int                      Slot
        {
            get { return mSlot; }
            set
            {
                mSlot = value;
            }
        }
        public EConnectionType          mConnectionType         = EConnectionType.PG;

        private System.Timers.Timer     mMainCycleTimer;
        public long                     mMainCycleTimeMS;
        private volatile bool           mDisconnect             = false;

        public uint                     mErrorsBeforeDisconnect = 3;
        private uint                    mErrorsCounter          = 0;

        private double                  mSlowingFactor          = 0.0D;
        public uint                     Slowdown
        {
            get { return (uint)mSlowingFactor; }
            set
            {
                if (value <= 0)
                {
                    mSlowingFactor = 0.0D;
                    mDeltaSlow = 0.0D;
                }
                else if (value >= 100)
                {
                    mSlowingFactor = 100.0D;
                    mDeltaSlow = 1.0D;
                }
                else
                {
                    mSlowingFactor = (double)value;
                    mDeltaSlow = 1.0D - 0.5D * Math.Log10(100.0D - mSlowingFactor);
                }
                mSlowCounter = 0.0D;
            }
        }
        private double                  mDeltaSlow              = 0.0D;
        private double                  mSlowCounter            = 0.0D;

        #region Read/Write optimisation

            private int                 mPDULength          = 0;

            private bool                mOptimize           = false;
            private void                optimize()
            {
                var lSegments   = new SegmentBuilder((uint)mPDULength, false);

                if (lSegments.Enabled)
                {
                    string lSegName;

                    int lCount = mItemList.Count;
                    for (int i = 0; i < lCount; i++)
                    {
                        if (mItemList[i].mMemoryType == EArea.M)
                        {
                            lSegName = "M";
                        }
                        else if (mItemList[i].mMemoryType == EArea.DB)
                        {
                            lSegName = "DB" + mItemList[i].DB.ToString();
                        }
                        else if (mItemList[i].mMemoryType == EArea.I)
                        {
                            lSegName = "I";
                        }
                        else
                        {
                            lSegName = "Q";
                        }

                        lSegments.addItem(lSegName, mItemList[i]);
                    }

                    var lSeg    = lSegments.getSegments();
                    mSegActual  = new bool[lSeg.Item1.Length];
                    mSegStart   = lSeg.Item1;
                    mSegBuffer  = new byte[lSeg.Item1.Length][];

                    for (int i = 0; i < lSeg.Item1.Length; i++)
                    {
                        mSegBuffer[i] = new byte[lSeg.Item2[i]];
                    }
                }
            }

            private bool[]              mSegActual;
            private void                resetSegActual()
            {
                if (mSegActual == null) return;

                for (int i = 0; i < mSegActual.Length; i++)
                {
                    mSegActual[i] = false;
                }
            }
            private int[]               mSegStart;
            private byte[][]            mSegBuffer;
            private int                 readSegment(int aSegmentID, EArea aMemoryType, int aDB)
            {
                return mClient.ReadArea((int)aMemoryType, 
                                                aDB, 
                                                    mSegStart[aSegmentID], 
                                                        mSegBuffer[aSegmentID].Length, 
                                                            (int)EWordlen.S7_Byte, 
                                                                mSegBuffer[aSegmentID]);
            }

            private void                read(DataItem aItem)
            {
                if (aItem.SegID < 0)
                {
                    aItem.read(mClient);
                }
                else
                {
                    if (mSegActual[aItem.SegID] == false)
                    {
                        int lResult = readSegment(aItem.SegID, aItem.mMemoryType, aItem.DB);
                        if (lResult != 0)
                        {
                            reportError("Error reading data for group of Items: " + mClient.ErrorText(lResult));
                            return;
                        }

                        mSegActual[aItem.SegID] = true;
                    }

                    if (mSegActual[aItem.SegID])
                    {
                        var lValue  = new byte[aItem.SegLength];
                        int lIndex  = aItem.SegAddress - mSegStart[aItem.SegID];

                        if (aItem.DataType == EWordlen.S7_Bit)
                        {
                            if ((mSegBuffer[aItem.SegID][lIndex] & (1 << aItem.Bit)) != 0)
                            {
                                lValue[0] = 1;
                            }
                            else
                            {
                                lValue[0] = 0;
                            }
                        }
                        else
                        {
                            Array.Copy(mSegBuffer[aItem.SegID], lIndex, lValue, 0, lValue.Length);
                        }

                        aItem.setValueFromPLC(lValue);
                    }                    
                }
            }
            private void                write(DataItem aItem)
            {
                mWriteRequests = mWriteRequests + 1;
                aItem.write(mClient);
            }

        #endregion

        public long                     mWriteRequests;

        private void                    MainCycle(object aSender, ElapsedEventArgs aEventArgs)
        {
            long lStartMS = MiscUtils.TimerMS;

            if (mDisconnect == false)
            {
                    try
                    {
                        if (mItemList.Count != 0)
                        {
                            #region Item List Changed

                                if (mItemListChanged)
                                {
                                    mItemRWList.Clear();
                                    mItemListLock.EnterReadLock();
                                    //========================================
                                    try
                                    {
                                        mItemRWList.AddRange(mItemList);
                                        mItemListChanged = false;
                                    }
                                    finally
                                    {
                                        //========================================
                                        mItemListLock.ExitReadLock();
                                    }

                                    foreach (DataItem lDataItem in mItemRWList)
                                    {
                                        lDataItem.SegID = -1;
                                    }

                                    mOptimize = true;
                                }
                                else if (mOptimize)
                                {
                                    mOptimize = false;
                                    optimize();
                                }

                            #endregion

                            int lCount = mItemRWList.Count;
                            DataItem lItem;

                            for (int i = 0; i < lCount; i++)
                            {
                                lItem = mItemRWList[i];

                                if (mDisconnect) { break; }

                                #region Write

                                    if (lItem.mNeedWrite)
                                    {
                                        write(lItem);
                                    }

                                #endregion

                                if (mDisconnect) { break; }

                                #region Read

                                    if (lItem.mNeedWrite != true)
                                    {
                                        read(lItem);
                                    }

                                #endregion

                                if (mDisconnect) { break; }

                                mSlowCounter = mSlowCounter + mDeltaSlow;
                                if (mSlowCounter >= 1.0D)
                                {
                                    mSlowCounter = mSlowCounter - 1.0D;
                                    Thread.Sleep(MiscUtils.TimeSlice);
                                }
                            }
                        }
                    }
                    catch (Exception lExc)
                    {
                        mDisconnect = true;
                        raiseConnectionError(lExc.Message);
                    }
            }

            resetSegActual();

            mMainCycleTimeMS = MiscUtils.TimerMS - lStartMS;

            if (mDisconnect)
            {
                int lResult = mClient.Disconnect();

                if (lResult != 0)
                {
                    Log.Error("S7IsoTCP disconnection error. " + mClient.ErrorText(lResult));
                }

                mConnected = false;
                raiseConnectionState();
                mCycleEndEvent.Set();
                mDisconnectEvent.Set();
            }
            else
            {
                mMainCycleTimer.Start();
                mCycleEndEvent.Set();
            }
        }

        private ManualResetEvent        mCycleEndEvent          = new ManualResetEvent(true);

        public void                     waitCycleEnd()
        {
            if (mConnected)
            {
                mCycleEndEvent.Reset();
                if (mConnected)
                {
                    mCycleEndEvent.WaitOne();
                }
            }
        }

        public void                     reportError(string aError)
        {
            if (mConnected && mErrorsBeforeDisconnect != 0)
            {
                mErrorsCounter = mErrorsCounter + 1;

                if (mErrorsCounter >= mErrorsBeforeDisconnect)
                {
                    mErrorsCounter = 0;
                    mDisconnect = true;
                }
            }
            else
            {
                mErrorsCounter = 0;
            }

            raiseConnectionError(aError);
        }

        public void                     connect()
        {
            mClient.SetConnectionType((ushort)mConnectionType);
            int lResult = mClient.ConnectTo(mIP, mRack, mSlot);
            if (lResult != 0)
            {
                throw new InvalidOperationException("Unable to connect. " + mClient.ErrorText(lResult));
            }

            mPDULength  = mClient.NegotiatedPduLength() - 18; // 18 byte payload of S7 telegramm

            mOptimize       = true;
            mConnected      = true;
            mDisconnect     = false;
            mWriteRequests  = 0;

            if (mMainCycleTimer == null)
            {
                mMainCycleTimer             = new System.Timers.Timer(MiscUtils.TimeSlice);
                mMainCycleTimer.Elapsed     += new ElapsedEventHandler(MainCycle);
                mMainCycleTimer.AutoReset   = false;
            }
            mMainCycleTimer.Start();

            raiseConnectionState();
        }

        public void                     disconnect()
        {
            mDisconnectEvent.Reset();
            mDisconnect = true;
            if (mConnected)
            {
                mDisconnectEvent.WaitOne();
            }
        }
        private ManualResetEvent        mDisconnectEvent        = new ManualResetEvent(true);

        private volatile bool           mConnected              = false;
        public bool                     Connected
        {
            get
            {
                return mConnected;
            }
        }

        public event EventHandler       ConnectionState;
        private void                    raiseConnectionState()
        {
            ConnectionState?.Invoke(this, EventArgs.Empty);
        }

        private string                  mLastError              = "";
        public string                   LastError
        {
            get
            {
                return mLastError;
            }
        }
        public event EventHandler<MessageStringEventArgs> ConnectionError;
        private void                    raiseConnectionError(string aMessage)
        {
            mLastError = aMessage;
            ConnectionError?.Invoke(this, new MessageStringEventArgs(aMessage));
        }

        #region Items

            private List<DataItem>          mItemList       = new List<DataItem>();
            private List<DataItem>          mItemRWList     = new List<DataItem>();
            private ReaderWriterLockSlim    mItemListLock   = new ReaderWriterLockSlim();
            private volatile bool           mItemListChanged;

            public void                     addItem(DataItem aItem)
            {
                aItem.mConnection   = this;
                ConnectionState     += new EventHandler(aItem.onConnectionStateChanged);
                aItem.initAccess();

                mItemListLock.EnterWriteLock();
                //========================================
                try
                {
                    mItemList.Add(aItem);
                    mItemListChanged = true;
                }
                finally
                {
                    //========================================
                    mItemListLock.ExitWriteLock();
                }
            }

            public void                     removeItem(DataItem aItem)
            {
                ConnectionState -= aItem.onConnectionStateChanged;

                mItemListLock.EnterWriteLock();
                //========================================
                try
                {
                    mItemList.Remove(aItem);
                    mItemListChanged = true;
                }
                finally
                {
                    //========================================
                    mItemListLock.ExitWriteLock();
                }
            }

            public int                      NumberOfItems
            {
                get
                {
                    return mItemList.Count;
                }
            }

        #endregion

        #region IDisposable

            private bool                mDisposed           = false;

            public void                 Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void      Dispose(bool aDisposing)
            {
                if (!mDisposed)
                {
                    if (aDisposing)
                    {
                        mItemList.Clear();
                        mItemRWList.Clear();

                        if (mMainCycleTimer != null)
                        {
                            mMainCycleTimer.Dispose();
                            mMainCycleTimer = null;
                        }

                        if (mItemListLock != null)
                        {
                            mItemListLock.Dispose();
                            mItemListLock = null;
                        }

                        if (mDisconnectEvent != null)
                        {
                            mDisconnectEvent.Dispose();
                            mDisconnectEvent = null;
                        }

                        if (mCycleEndEvent != null)
                        {
                            mCycleEndEvent.Dispose();
                            mCycleEndEvent = null;
                        }
                    }

                    mDisposed = true;
                }
            }

        #endregion
    }
}
