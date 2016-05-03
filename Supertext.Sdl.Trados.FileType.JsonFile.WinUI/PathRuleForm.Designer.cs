namespace Supertext.Sdl.Trados.FileType.JsonFile.WinUI
{
    partial class PathRuleForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PathRuleForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.l_Opening = new System.Windows.Forms.Label();
            this._pathRuleTextBox = new System.Windows.Forms.TextBox();
            this._ruleGroupBox = new System.Windows.Forms.GroupBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this._ruleGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnOk, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._ruleGroupBox, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this._pathRuleTextBox, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.l_Opening, 0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // l_Opening
            // 
            resources.ApplyResources(this.l_Opening, "l_Opening");
            this.l_Opening.Name = "l_Opening";
            // 
            // _pathRuleTextBox
            // 
            resources.ApplyResources(this._pathRuleTextBox, "_pathRuleTextBox");
            this._pathRuleTextBox.Name = "_pathRuleTextBox";
            // 
            // _ruleGroupBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._ruleGroupBox, 2);
            this._ruleGroupBox.Controls.Add(this.tableLayoutPanel3);
            resources.ApplyResources(this._ruleGroupBox, "_ruleGroupBox");
            this._ruleGroupBox.Name = "_ruleGroupBox";
            this._ruleGroupBox.TabStop = false;
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // PathRuleForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PathRuleForm";
            this.ShowIcon = false;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this._ruleGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox _ruleGroupBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TextBox _pathRuleTextBox;
        private System.Windows.Forms.Label l_Opening;
    }
}