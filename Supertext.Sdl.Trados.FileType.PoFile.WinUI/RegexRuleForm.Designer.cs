﻿namespace Supertext.Sdl.Trados.FileType.PoFile.WinUI
{
    partial class RegexRuleForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegexRuleForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this._ruleTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this._ruleGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.l_Opening = new System.Windows.Forms.Label();
            this._endTagTextBox = new System.Windows.Forms.TextBox();
            this.l_Closing = new System.Windows.Forms.Label();
            this._startTagTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this._translateComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this._segmentationHintComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this._ignoreCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this._ruleGroupBox.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this._ruleTypeComboBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._ruleGroupBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // groupBox2
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox2, 2);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Controls.Add(this.btnOk);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // _ruleTypeComboBox
            // 
            resources.ApplyResources(this._ruleTypeComboBox, "_ruleTypeComboBox");
            this._ruleTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._ruleTypeComboBox.FormattingEnabled = true;
            this._ruleTypeComboBox.Name = "_ruleTypeComboBox";
            this._ruleTypeComboBox.SelectedIndexChanged += new System.EventHandler(this._ruleTypeComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // _ruleGroupBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._ruleGroupBox, 2);
            this._ruleGroupBox.Controls.Add(this.tableLayoutPanel3);
            resources.ApplyResources(this._ruleGroupBox, "_ruleGroupBox");
            this._ruleGroupBox.Name = "_ruleGroupBox";
            this._ruleGroupBox.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.l_Opening, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this._endTagTextBox, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.l_Closing, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this._startTagTextBox, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this._ignoreCaseCheckBox, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // l_Opening
            // 
            resources.ApplyResources(this.l_Opening, "l_Opening");
            this.l_Opening.Name = "l_Opening";
            // 
            // _endTagTextBox
            // 
            resources.ApplyResources(this._endTagTextBox, "_endTagTextBox");
            this._endTagTextBox.Name = "_endTagTextBox";
            this._endTagTextBox.TextChanged += new System.EventHandler(this.tb_Closing_TextChanged);
            // 
            // l_Closing
            // 
            resources.ApplyResources(this.l_Closing, "l_Closing");
            this.l_Closing.Name = "l_Closing";
            // 
            // _startTagTextBox
            // 
            resources.ApplyResources(this._startTagTextBox, "_startTagTextBox");
            this._startTagTextBox.Name = "_startTagTextBox";
            this._startTagTextBox.TextChanged += new System.EventHandler(this.tb_Opening_TextChanged);
            // 
            // groupBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this._translateComboBox, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // _translateComboBox
            // 
            resources.ApplyResources(this._translateComboBox, "_translateComboBox");
            this._translateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._translateComboBox.FormattingEnabled = true;
            this._translateComboBox.Name = "_translateComboBox";
            // 
            // groupBox3
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox3, 2);
            this.groupBox3.Controls.Add(this.tableLayoutPanel4);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this._segmentationHintComboBox, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // _segmentationHintComboBox
            // 
            resources.ApplyResources(this._segmentationHintComboBox, "_segmentationHintComboBox");
            this._segmentationHintComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._segmentationHintComboBox.FormattingEnabled = true;
            this._segmentationHintComboBox.Name = "_segmentationHintComboBox";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // _errorProvider
            // 
            this._errorProvider.ContainerControl = this;
            // 
            // _ignoreCaseCheckBox
            // 
            resources.ApplyResources(this._ignoreCaseCheckBox, "_ignoreCaseCheckBox");
            this._ignoreCaseCheckBox.Name = "_ignoreCaseCheckBox";
            this._ignoreCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // RegexRuleForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RegexRuleForm";
            this.ShowIcon = false;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this._ruleGroupBox.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox _ruleTypeComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox _ruleGroupBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label l_Opening;
        private System.Windows.Forms.TextBox _endTagTextBox;
        private System.Windows.Forms.Label l_Closing;
        private System.Windows.Forms.TextBox _startTagTextBox;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox _translateComboBox;
        private System.Windows.Forms.ErrorProvider _errorProvider;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.ComboBox _segmentationHintComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox _ignoreCaseCheckBox;
        private System.Windows.Forms.Label label4;
    }
}