namespace Supertext.Sdl.Trados.FileType.JsonFile
{
    partial class ParsingSettingsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParsingSettingsControl));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._enablePathFilter = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this._tagDefinitionLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._rulesListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._removeAllButton = new System.Windows.Forms.Button();
            this._addRuleButton = new System.Windows.Forms.Button();
            this._removeRuleButton = new System.Windows.Forms.Button();
            this._editRuleButton = new System.Windows.Forms.Button();
            this._helpButton = new System.Windows.Forms.Button();
            this._extractButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this._tagDefinitionLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this._enablePathFilter, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // _enablePathFilter
            // 
            resources.ApplyResources(this._enablePathFilter, "_enablePathFilter");
            this._enablePathFilter.Name = "_enablePathFilter";
            this._enablePathFilter.UseVisualStyleBackColor = true;
            this._enablePathFilter.CheckedChanged += new System.EventHandler(this._enablePathFilter_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this._tagDefinitionLayoutPanel);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // _tagDefinitionLayoutPanel
            // 
            resources.ApplyResources(this._tagDefinitionLayoutPanel, "_tagDefinitionLayoutPanel");
            this._tagDefinitionLayoutPanel.Controls.Add(this._rulesListView, 0, 0);
            this._tagDefinitionLayoutPanel.Controls.Add(this._removeAllButton, 1, 3);
            this._tagDefinitionLayoutPanel.Controls.Add(this._addRuleButton, 1, 0);
            this._tagDefinitionLayoutPanel.Controls.Add(this._removeRuleButton, 1, 2);
            this._tagDefinitionLayoutPanel.Controls.Add(this._editRuleButton, 1, 1);
            this._tagDefinitionLayoutPanel.Controls.Add(this._helpButton, 1, 5);
            this._tagDefinitionLayoutPanel.Controls.Add(this._extractButton, 1, 4);
            this._tagDefinitionLayoutPanel.Name = "_tagDefinitionLayoutPanel";
            // 
            // _rulesListView
            // 
            resources.ApplyResources(this._rulesListView, "_rulesListView");
            this._rulesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this._rulesListView.FullRowSelect = true;
            this._rulesListView.Name = "_rulesListView";
            this._tagDefinitionLayoutPanel.SetRowSpan(this._rulesListView, 6);
            this._rulesListView.UseCompatibleStateImageBehavior = false;
            this._rulesListView.View = System.Windows.Forms.View.Details;
            this._rulesListView.SelectedIndexChanged += new System.EventHandler(this._rulesListView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // _removeAllButton
            // 
            resources.ApplyResources(this._removeAllButton, "_removeAllButton");
            this._removeAllButton.Name = "_removeAllButton";
            this._removeAllButton.UseVisualStyleBackColor = true;
            this._removeAllButton.Click += new System.EventHandler(this._removeAllButton_Click);
            // 
            // _addRuleButton
            // 
            resources.ApplyResources(this._addRuleButton, "_addRuleButton");
            this._addRuleButton.Name = "_addRuleButton";
            this._addRuleButton.UseVisualStyleBackColor = true;
            this._addRuleButton.Click += new System.EventHandler(this._addRuleButton_Click);
            // 
            // _removeRuleButton
            // 
            resources.ApplyResources(this._removeRuleButton, "_removeRuleButton");
            this._removeRuleButton.Name = "_removeRuleButton";
            this._removeRuleButton.UseVisualStyleBackColor = true;
            this._removeRuleButton.Click += new System.EventHandler(this._removeRuleButton_Click);
            // 
            // _editRuleButton
            // 
            resources.ApplyResources(this._editRuleButton, "_editRuleButton");
            this._editRuleButton.Name = "_editRuleButton";
            this._editRuleButton.UseVisualStyleBackColor = true;
            this._editRuleButton.Click += new System.EventHandler(this._editRuleButton_Click);
            // 
            // _helpButton
            // 
            resources.ApplyResources(this._helpButton, "_helpButton");
            this._helpButton.Name = "_helpButton";
            this._helpButton.UseVisualStyleBackColor = true;
            this._helpButton.Click += new System.EventHandler(this._helpButton_Click);
            // 
            // _extractButton
            // 
            resources.ApplyResources(this._extractButton, "_extractButton");
            this._extractButton.Name = "_extractButton";
            this._extractButton.UseVisualStyleBackColor = true;
            this._extractButton.Click += new System.EventHandler(this._extractButton_Click);
            // 
            // ParsingSettingsControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "ParsingSettingsControl";
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this._tagDefinitionLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox _enablePathFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TableLayoutPanel _tagDefinitionLayoutPanel;
        private System.Windows.Forms.ListView _rulesListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button _addRuleButton;
        private System.Windows.Forms.Button _removeAllButton;
        private System.Windows.Forms.Button _removeRuleButton;
        private System.Windows.Forms.Button _editRuleButton;
        private System.Windows.Forms.Button _helpButton;
        private System.Windows.Forms.Button _extractButton;
    }
}
