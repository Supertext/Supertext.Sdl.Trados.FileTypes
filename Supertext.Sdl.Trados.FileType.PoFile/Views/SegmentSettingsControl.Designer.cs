using System.Windows.Forms;

namespace Supertext.Sdl.Trados.FileType.PoFile.Views
{
    partial class SegmentSettingsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SegmentSettingsControl));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_MessageStringAsSource = new System.Windows.Forms.CheckBox();
            this.cb_TargetTextNeeded = new System.Windows.Forms.CheckBox();
            this.tb_note = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tb_targetSegment = new System.Windows.Forms.TextBox();
            this.tb_sourceSegment = new System.Windows.Forms.TextBox();
            this.tb_example = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_MessageStringAsSource);
            this.groupBox1.Controls.Add(this.cb_TargetTextNeeded);
            this.groupBox1.Controls.Add(this.tb_note);
            this.groupBox1.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cb_MessageStringAsSource
            // 
            resources.ApplyResources(this.cb_MessageStringAsSource, "cb_MessageStringAsSource");
            this.cb_MessageStringAsSource.Name = "cb_MessageStringAsSource";
            this.cb_MessageStringAsSource.UseVisualStyleBackColor = true;
            this.cb_MessageStringAsSource.CheckedChanged += new System.EventHandler(this.cb_MessageStringAsSource_CheckedChanged);
            // 
            // cb_TargetTextNeeded
            // 
            resources.ApplyResources(this.cb_TargetTextNeeded, "cb_TargetTextNeeded");
            this.cb_TargetTextNeeded.Name = "cb_TargetTextNeeded";
            this.cb_TargetTextNeeded.UseVisualStyleBackColor = true;
            this.cb_TargetTextNeeded.CheckedChanged += new System.EventHandler(this.cb_TargetTextNeeded_CheckedChanged);
            // 
            // tb_note
            // 
            this.tb_note.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.tb_note, "tb_note");
            this.tb_note.Name = "tb_note";
            this.tb_note.ReadOnly = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tb_targetSegment);
            this.groupBox2.Controls.Add(this.tb_sourceSegment);
            this.groupBox2.Controls.Add(this.tb_example);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // tb_targetSegment
            // 
            resources.ApplyResources(this.tb_targetSegment, "tb_targetSegment");
            this.tb_targetSegment.Name = "tb_targetSegment";
            this.tb_targetSegment.ReadOnly = true;
            // 
            // tb_sourceSegment
            // 
            resources.ApplyResources(this.tb_sourceSegment, "tb_sourceSegment");
            this.tb_sourceSegment.Name = "tb_sourceSegment";
            this.tb_sourceSegment.ReadOnly = true;
            // 
            // tb_example
            // 
            this.tb_example.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.tb_example, "tb_example");
            this.tb_example.Name = "tb_example";
            this.tb_example.ReadOnly = true;
            // 
            // SegmentSettingsControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "SegmentSettingsControl";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBox2;
    }
}
