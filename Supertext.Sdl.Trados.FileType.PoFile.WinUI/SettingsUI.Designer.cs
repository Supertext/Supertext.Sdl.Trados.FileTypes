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
            groupBox1 = new System.Windows.Forms.GroupBox();
            cb_MessageStringAsSource = new System.Windows.Forms.CheckBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cb_MessageStringAsSource);
            groupBox1.Location = new System.Drawing.Point(13, 13);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(370, 308);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Source settings";
            // 
            // cb_MessageStringAsSource
            // 
            cb_MessageStringAsSource.AutoSize = true;
            cb_MessageStringAsSource.Checked = true;
            cb_MessageStringAsSource.CheckState = System.Windows.Forms.CheckState.Checked;
            cb_MessageStringAsSource.Location = new System.Drawing.Point(12, 34);
            cb_MessageStringAsSource.Name = "cb_MessageStringAsSource";
            cb_MessageStringAsSource.Size = new System.Drawing.Size(121, 17);
            cb_MessageStringAsSource.TabIndex = 0;
            cb_MessageStringAsSource.Text = "Message string as source";
            cb_MessageStringAsSource.UseVisualStyleBackColor = true;
            cb_MessageStringAsSource.CheckedChanged += new System.EventHandler(cb_MessageStringAsSource_CheckedChanged);
            // 
            // SettingsUI
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(groupBox1);
            Name = "SettingsUI";
            Size = new System.Drawing.Size(392, 332);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cb_MessageStringAsSource;
    }
}
