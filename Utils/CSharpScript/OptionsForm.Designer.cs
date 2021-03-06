﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace Utils.CSharpScript
{
    partial class OptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label_watchdog = new System.Windows.Forms.Label();
            this.label_TTime = new System.Windows.Forms.Label();
            this.spinEdit_Trigger = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit_Watchdog = new DevExpress.XtraEditors.SpinEdit();
            this.label_On = new System.Windows.Forms.Label();
            this.itemEditBox_OnItem = new Utils.SpecialControls.ItemEditBox();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit_Trigger.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit_Watchdog.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // label_watchdog
            // 
            this.label_watchdog.AutoSize = true;
            this.label_watchdog.Location = new System.Drawing.Point(7, 83);
            this.label_watchdog.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_watchdog.Name = "label_watchdog";
            this.label_watchdog.Size = new System.Drawing.Size(106, 17);
            this.label_watchdog.TabIndex = 25;
            this.label_watchdog.Text = "Watchdog [ms]:";
            // 
            // label_TTime
            // 
            this.label_TTime.AutoSize = true;
            this.label_TTime.Location = new System.Drawing.Point(25, 52);
            this.label_TTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_TTime.Name = "label_TTime";
            this.label_TTime.Size = new System.Drawing.Size(88, 17);
            this.label_TTime.TabIndex = 26;
            this.label_TTime.Text = "Trigger [ms]:";
            // 
            // spinEdit_Trigger
            // 
            this.spinEdit_Trigger.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEdit_Trigger.Location = new System.Drawing.Point(117, 48);
            this.spinEdit_Trigger.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.spinEdit_Trigger.Name = "spinEdit_Trigger";
            this.spinEdit_Trigger.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit_Trigger.Properties.IsFloatValue = false;
            this.spinEdit_Trigger.Properties.LookAndFeel.SkinName = "Office 2010 Black";
            this.spinEdit_Trigger.Properties.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
            this.spinEdit_Trigger.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.spinEdit_Trigger.Properties.Mask.EditMask = "N00";
            this.spinEdit_Trigger.Properties.MaxValue = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.spinEdit_Trigger.Size = new System.Drawing.Size(211, 24);
            this.spinEdit_Trigger.TabIndex = 1;
            this.spinEdit_Trigger.EditValueChanged += new System.EventHandler(this.spinEdit_Trigger_EditValueChanged);
            // 
            // spinEdit_Watchdog
            // 
            this.spinEdit_Watchdog.EditValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spinEdit_Watchdog.Location = new System.Drawing.Point(117, 79);
            this.spinEdit_Watchdog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.spinEdit_Watchdog.Name = "spinEdit_Watchdog";
            this.spinEdit_Watchdog.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit_Watchdog.Properties.IsFloatValue = false;
            this.spinEdit_Watchdog.Properties.LookAndFeel.SkinName = "Office 2010 Black";
            this.spinEdit_Watchdog.Properties.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
            this.spinEdit_Watchdog.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.spinEdit_Watchdog.Properties.Mask.EditMask = "N00";
            this.spinEdit_Watchdog.Properties.MaxValue = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.spinEdit_Watchdog.Properties.MinValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spinEdit_Watchdog.Size = new System.Drawing.Size(211, 24);
            this.spinEdit_Watchdog.TabIndex = 2;
            this.spinEdit_Watchdog.EditValueChanged += new System.EventHandler(this.spinEdit_Watchdog_EditValueChanged);
            // 
            // label_On
            // 
            this.label_On.AutoSize = true;
            this.label_On.Location = new System.Drawing.Point(82, 21);
            this.label_On.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_On.Name = "label_On";
            this.label_On.Size = new System.Drawing.Size(31, 17);
            this.label_On.TabIndex = 27;
            this.label_On.Text = "On:";
            // 
            // itemEditBox_OnItem
            // 
            this.itemEditBox_OnItem.ItemName = "";
            this.itemEditBox_OnItem.ItemRequirements = "Binary, Read, Write";
            this.itemEditBox_OnItem.ItemToolTip = "";
            this.itemEditBox_OnItem.Location = new System.Drawing.Point(117, 14);
            this.itemEditBox_OnItem.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.itemEditBox_OnItem.Name = "itemEditBox_OnItem";
            this.itemEditBox_OnItem.Size = new System.Drawing.Size(211, 30);
            this.itemEditBox_OnItem.TabIndex = 0;
            this.itemEditBox_OnItem.ButtonClick += new System.EventHandler(this.ItemButtonClick);
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 110);
            this.Controls.Add(this.itemEditBox_OnItem);
            this.Controls.Add(this.label_On);
            this.Controls.Add(this.spinEdit_Watchdog);
            this.Controls.Add(this.spinEdit_Trigger);
            this.Controls.Add(this.label_watchdog);
            this.Controls.Add(this.label_TTime);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OptionsForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit_Trigger.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit_Watchdog.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_watchdog;
        private System.Windows.Forms.Label label_TTime;
        private DevExpress.XtraEditors.SpinEdit spinEdit_Trigger;
        private DevExpress.XtraEditors.SpinEdit spinEdit_Watchdog;
        private System.Windows.Forms.Label label_On;
        private SpecialControls.ItemEditBox itemEditBox_OnItem;
    }
}