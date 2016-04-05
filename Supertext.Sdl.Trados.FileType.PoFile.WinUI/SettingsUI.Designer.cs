namespace Supertext.Sdl.Trados.FileType.PoFile.WinUI
{
    partial class SettingsUI
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
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_MessageStringAsSource);
            this.groupBox1.Controls.Add(this.cb_TargetTextNeeded);
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
            this.cb_MessageStringAsSource.Location = new System.Drawing.Point(12, 34);
            this.cb_MessageStringAsSource.Name = "cb_MessageStringAsSource";
            this.cb_MessageStringAsSource.Size = new System.Drawing.Size(146, 17);
            this.cb_MessageStringAsSource.TabIndex = 0;
            this.cb_MessageStringAsSource.Text = PluginResources.Setting_MessageString_As_Source;
            this.cb_MessageStringAsSource.UseVisualStyleBackColor = true;
            this.cb_MessageStringAsSource.CheckedChanged += new System.EventHandler(this.cb_MessageStringAsSource_CheckedChanged);
            //
            // 
            //

            // 
            // cb_TargetTextNeeded
            // 
            this.cb_TargetTextNeeded.AutoSize = true;
            this.cb_TargetTextNeeded.Location = new System.Drawing.Point(12, 140);
            this.cb_TargetTextNeeded.Name = "cb_TargetTextNeeded";
            this.cb_TargetTextNeeded.Size = new System.Drawing.Size(180, 17);
            this.cb_TargetTextNeeded.TabIndex = 0;
            this.cb_TargetTextNeeded.Text = PluginResources.Setting_Target_Text_Needed;
            this.cb_TargetTextNeeded.UseVisualStyleBackColor = true;
            this.cb_TargetTextNeeded.CheckedChanged += new System.EventHandler(this.cb_TargetTextNeeded_CheckedChanged);
            // 
            // SettingsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "SettingsUI";
            this.Size = new System.Drawing.Size(392, 332);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cb_MessageStringAsSource;
        private System.Windows.Forms.CheckBox cb_TargetTextNeeded;
    }
}
