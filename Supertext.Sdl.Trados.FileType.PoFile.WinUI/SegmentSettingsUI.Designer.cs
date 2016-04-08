using System;
using System.Windows.Forms;

namespace Supertext.Sdl.Trados.FileType.PoFile.WinUI
{
    partial class SegmentSettingsUI
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cb_MessageStringAsSource;
        private System.Windows.Forms.CheckBox cb_TargetTextNeeded;
        private TextBox tb_example;
        private TextBox tb_sourceSegment;
        private TextBox tb_targetSegment;
        private TextBox tb_note;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_MessageStringAsSource = new System.Windows.Forms.CheckBox();
            this.cb_TargetTextNeeded = new System.Windows.Forms.CheckBox();
            this.tb_example = new System.Windows.Forms.TextBox();
            this.tb_sourceSegment = new System.Windows.Forms.TextBox();
            this.tb_targetSegment = new System.Windows.Forms.TextBox();
            this.tb_note = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_MessageStringAsSource);
            this.groupBox1.Controls.Add(this.cb_TargetTextNeeded);
            this.groupBox1.Controls.Add(this.tb_example);
            this.groupBox1.Controls.Add(this.tb_sourceSegment);
            this.groupBox1.Controls.Add(this.tb_targetSegment);
            this.groupBox1.Controls.Add(this.tb_note);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(392, 332);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // cb_MessageStringAsSource
            // 
            this.cb_MessageStringAsSource.AutoSize = true;
            this.cb_MessageStringAsSource.Location = new System.Drawing.Point(6, 19);
            this.cb_MessageStringAsSource.Name = "cb_MessageStringAsSource";
            this.cb_MessageStringAsSource.Size = new System.Drawing.Size(146, 17);
            this.cb_MessageStringAsSource.TabIndex = 0;
            this.cb_MessageStringAsSource.Text = global::Supertext.Sdl.Trados.FileType.PoFile.WinUI.PluginResources.Setting_MessageString_As_Source;
            this.cb_MessageStringAsSource.UseVisualStyleBackColor = true;
            this.cb_MessageStringAsSource.CheckedChanged += new System.EventHandler(this.cb_MessageStringAsSource_CheckedChanged);
            // 
            // cb_TargetTextNeeded
            // 
            this.cb_TargetTextNeeded.AutoSize = true;
            this.cb_TargetTextNeeded.Location = new System.Drawing.Point(6, 42);
            this.cb_TargetTextNeeded.Name = "cb_TargetTextNeeded";
            this.cb_TargetTextNeeded.Size = new System.Drawing.Size(180, 17);
            this.cb_TargetTextNeeded.TabIndex = 0;
            this.cb_TargetTextNeeded.Text = global::Supertext.Sdl.Trados.FileType.PoFile.WinUI.PluginResources.Setting_Target_Text_Needed;
            this.cb_TargetTextNeeded.UseVisualStyleBackColor = true;
            this.cb_TargetTextNeeded.CheckedChanged += new System.EventHandler(this.cb_TargetTextNeeded_CheckedChanged);
            // 
            // tb_example
            // 
            this.tb_example.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_example.Location = new System.Drawing.Point(25, 65);
            this.tb_example.Multiline = true;
            this.tb_example.Name = "tb_example";
            this.tb_example.ReadOnly = true;
            this.tb_example.Size = new System.Drawing.Size(146, 30);
            this.tb_example.TabIndex = 1;
            // 
            // tb_sourceSegment
            // 
            this.tb_sourceSegment.Location = new System.Drawing.Point(25, 104);
            this.tb_sourceSegment.Name = "tb_sourceSegment";
            this.tb_sourceSegment.ReadOnly = true;
            this.tb_sourceSegment.Size = new System.Drawing.Size(173, 20);
            this.tb_sourceSegment.TabIndex = 2;
            // 
            // tb_targetSegment
            // 
            this.tb_targetSegment.Location = new System.Drawing.Point(204, 104);
            this.tb_targetSegment.Name = "tb_targetSegment";
            this.tb_targetSegment.ReadOnly = true;
            this.tb_targetSegment.Size = new System.Drawing.Size(173, 20);
            this.tb_targetSegment.TabIndex = 0;
            // 
            // tb_note
            // 
            this.tb_note.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_note.Location = new System.Drawing.Point(25, 137);
            this.tb_note.Multiline = true;
            this.tb_note.Name = "tb_note";
            this.tb_note.ReadOnly = true;
            this.tb_note.Size = new System.Drawing.Size(367, 179);
            this.tb_note.TabIndex = 3;
            // 
            // SegmentSettingsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "SegmentSettingsUI";
            this.Size = new System.Drawing.Size(392, 332);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
