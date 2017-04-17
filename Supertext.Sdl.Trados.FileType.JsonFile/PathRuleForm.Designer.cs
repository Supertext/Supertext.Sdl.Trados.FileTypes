﻿namespace Supertext.Sdl.Trados.FileType.JsonFile
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
            this._ruleGroupBox = new System.Windows.Forms.GroupBox();
            this._pathRulesTable = new System.Windows.Forms.TableLayoutPanel();
            this._sourcePathPatternTextBox = new System.Windows.Forms.TextBox();
            this._sourcePathLabel = new System.Windows.Forms.Label();
            this._ignoreCaseCheckBox = new System.Windows.Forms.CheckBox();
            this._targetPathPatternTextBox = new System.Windows.Forms.TextBox();
            this._targetPathLabel = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this._ruleGroupBox.SuspendLayout();
            this._pathRulesTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this._ruleGroupBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnOk, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // _ruleGroupBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._ruleGroupBox, 2);
            this._ruleGroupBox.Controls.Add(this._pathRulesTable);
            resources.ApplyResources(this._ruleGroupBox, "_ruleGroupBox");
            this._ruleGroupBox.Name = "_ruleGroupBox";
            this._ruleGroupBox.TabStop = false;
            // 
            // _pathRulesTable
            // 
            resources.ApplyResources(this._pathRulesTable, "_pathRulesTable");
            this._pathRulesTable.Controls.Add(this._sourcePathPatternTextBox, 1, 1);
            this._pathRulesTable.Controls.Add(this._sourcePathLabel, 0, 1);
            this._pathRulesTable.Controls.Add(this._ignoreCaseCheckBox, 1, 3);
            this._pathRulesTable.Controls.Add(this._targetPathPatternTextBox, 1, 2);
            this._pathRulesTable.Controls.Add(this._targetPathLabel, 0, 2);
            this._pathRulesTable.Name = "_pathRulesTable";
            // 
            // _sourcePathPatternTextBox
            // 
            resources.ApplyResources(this._sourcePathPatternTextBox, "_sourcePathPatternTextBox");
            this._sourcePathPatternTextBox.Name = "_sourcePathPatternTextBox";
            // 
            // _sourcePathLabel
            // 
            resources.ApplyResources(this._sourcePathLabel, "_sourcePathLabel");
            this._sourcePathLabel.Name = "_sourcePathLabel";
            // 
            // _ignoreCaseCheckBox
            // 
            resources.ApplyResources(this._ignoreCaseCheckBox, "_ignoreCaseCheckBox");
            this._ignoreCaseCheckBox.Name = "_ignoreCaseCheckBox";
            this._ignoreCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // _targetPathPatternTextBox
            // 
            resources.ApplyResources(this._targetPathPatternTextBox, "_targetPathPatternTextBox");
            this._targetPathPatternTextBox.Name = "_targetPathPatternTextBox";
            // 
            // _targetPathLabel
            // 
            resources.ApplyResources(this._targetPathLabel, "_targetPathLabel");
            this._targetPathLabel.Name = "_targetPathLabel";
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
            // PathRuleForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PathRuleForm";
            this.ShowIcon = false;
            this.tableLayoutPanel1.ResumeLayout(false);
            this._ruleGroupBox.ResumeLayout(false);
            this._pathRulesTable.ResumeLayout(false);
            this._pathRulesTable.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox _ruleGroupBox;
        private System.Windows.Forms.TableLayoutPanel _pathRulesTable;
        private System.Windows.Forms.TextBox _sourcePathPatternTextBox;
        private System.Windows.Forms.Label _sourcePathLabel;
        private System.Windows.Forms.CheckBox _ignoreCaseCheckBox;
        private System.Windows.Forms.TextBox _targetPathPatternTextBox;
        private System.Windows.Forms.Label _targetPathLabel;
    }
}