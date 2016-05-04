namespace Supertext.Sdl.Trados.FileType.PoFile.WinUI
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
            this._ignoreCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this._translateComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this._segmentationHintComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
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
            this._errorProvider.SetError(this.tableLayoutPanel1, resources.GetString("tableLayoutPanel1.Error"));
            this._errorProvider.SetIconAlignment(this.tableLayoutPanel1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tableLayoutPanel1.IconAlignment"))));
            this._errorProvider.SetIconPadding(this.tableLayoutPanel1, ((int)(resources.GetObject("tableLayoutPanel1.IconPadding"))));
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox2, 2);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Controls.Add(this.btnOk);
            this._errorProvider.SetError(this.groupBox2, resources.GetString("groupBox2.Error"));
            this._errorProvider.SetIconAlignment(this.groupBox2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox2.IconAlignment"))));
            this._errorProvider.SetIconPadding(this.groupBox2, ((int)(resources.GetObject("groupBox2.IconPadding"))));
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._errorProvider.SetError(this.btnCancel, resources.GetString("btnCancel.Error"));
            this._errorProvider.SetIconAlignment(this.btnCancel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnCancel.IconAlignment"))));
            this._errorProvider.SetIconPadding(this.btnCancel, ((int)(resources.GetObject("btnCancel.IconPadding"))));
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._errorProvider.SetError(this.btnOk, resources.GetString("btnOk.Error"));
            this._errorProvider.SetIconAlignment(this.btnOk, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnOk.IconAlignment"))));
            this._errorProvider.SetIconPadding(this.btnOk, ((int)(resources.GetObject("btnOk.IconPadding"))));
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // _ruleTypeComboBox
            // 
            resources.ApplyResources(this._ruleTypeComboBox, "_ruleTypeComboBox");
            this._ruleTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._errorProvider.SetError(this._ruleTypeComboBox, resources.GetString("_ruleTypeComboBox.Error"));
            this._ruleTypeComboBox.FormattingEnabled = true;
            this._errorProvider.SetIconAlignment(this._ruleTypeComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("_ruleTypeComboBox.IconAlignment"))));
            this._errorProvider.SetIconPadding(this._ruleTypeComboBox, ((int)(resources.GetObject("_ruleTypeComboBox.IconPadding"))));
            this._ruleTypeComboBox.Name = "_ruleTypeComboBox";
            this._ruleTypeComboBox.SelectedIndexChanged += new System.EventHandler(this._ruleTypeComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this._errorProvider.SetError(this.label1, resources.GetString("label1.Error"));
            this._errorProvider.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
            this._errorProvider.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding"))));
            this.label1.Name = "label1";
            // 
            // _ruleGroupBox
            // 
            resources.ApplyResources(this._ruleGroupBox, "_ruleGroupBox");
            this.tableLayoutPanel1.SetColumnSpan(this._ruleGroupBox, 2);
            this._ruleGroupBox.Controls.Add(this.tableLayoutPanel3);
            this._errorProvider.SetError(this._ruleGroupBox, resources.GetString("_ruleGroupBox.Error"));
            this._errorProvider.SetIconAlignment(this._ruleGroupBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("_ruleGroupBox.IconAlignment"))));
            this._errorProvider.SetIconPadding(this._ruleGroupBox, ((int)(resources.GetObject("_ruleGroupBox.IconPadding"))));
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
            this._errorProvider.SetError(this.tableLayoutPanel3, resources.GetString("tableLayoutPanel3.Error"));
            this._errorProvider.SetIconAlignment(this.tableLayoutPanel3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tableLayoutPanel3.IconAlignment"))));
            this._errorProvider.SetIconPadding(this.tableLayoutPanel3, ((int)(resources.GetObject("tableLayoutPanel3.IconPadding"))));
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // l_Opening
            // 
            resources.ApplyResources(this.l_Opening, "l_Opening");
            this._errorProvider.SetError(this.l_Opening, resources.GetString("l_Opening.Error"));
            this._errorProvider.SetIconAlignment(this.l_Opening, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("l_Opening.IconAlignment"))));
            this._errorProvider.SetIconPadding(this.l_Opening, ((int)(resources.GetObject("l_Opening.IconPadding"))));
            this.l_Opening.Name = "l_Opening";
            // 
            // _endTagTextBox
            // 
            resources.ApplyResources(this._endTagTextBox, "_endTagTextBox");
            this._errorProvider.SetError(this._endTagTextBox, resources.GetString("_endTagTextBox.Error"));
            this._errorProvider.SetIconAlignment(this._endTagTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("_endTagTextBox.IconAlignment"))));
            this._errorProvider.SetIconPadding(this._endTagTextBox, ((int)(resources.GetObject("_endTagTextBox.IconPadding"))));
            this._endTagTextBox.Name = "_endTagTextBox";
            this._endTagTextBox.TextChanged += new System.EventHandler(this.tb_Closing_TextChanged);
            // 
            // l_Closing
            // 
            resources.ApplyResources(this.l_Closing, "l_Closing");
            this._errorProvider.SetError(this.l_Closing, resources.GetString("l_Closing.Error"));
            this._errorProvider.SetIconAlignment(this.l_Closing, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("l_Closing.IconAlignment"))));
            this._errorProvider.SetIconPadding(this.l_Closing, ((int)(resources.GetObject("l_Closing.IconPadding"))));
            this.l_Closing.Name = "l_Closing";
            // 
            // _startTagTextBox
            // 
            resources.ApplyResources(this._startTagTextBox, "_startTagTextBox");
            this._errorProvider.SetError(this._startTagTextBox, resources.GetString("_startTagTextBox.Error"));
            this._errorProvider.SetIconAlignment(this._startTagTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("_startTagTextBox.IconAlignment"))));
            this._errorProvider.SetIconPadding(this._startTagTextBox, ((int)(resources.GetObject("_startTagTextBox.IconPadding"))));
            this._startTagTextBox.Name = "_startTagTextBox";
            this._startTagTextBox.TextChanged += new System.EventHandler(this.tb_Opening_TextChanged);
            // 
            // _ignoreCaseCheckBox
            // 
            resources.ApplyResources(this._ignoreCaseCheckBox, "_ignoreCaseCheckBox");
            this._errorProvider.SetError(this._ignoreCaseCheckBox, resources.GetString("_ignoreCaseCheckBox.Error"));
            this._errorProvider.SetIconAlignment(this._ignoreCaseCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("_ignoreCaseCheckBox.IconAlignment"))));
            this._errorProvider.SetIconPadding(this._ignoreCaseCheckBox, ((int)(resources.GetObject("_ignoreCaseCheckBox.IconPadding"))));
            this._ignoreCaseCheckBox.Name = "_ignoreCaseCheckBox";
            this._ignoreCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this._errorProvider.SetError(this.groupBox1, resources.GetString("groupBox1.Error"));
            this._errorProvider.SetIconAlignment(this.groupBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox1.IconAlignment"))));
            this._errorProvider.SetIconPadding(this.groupBox1, ((int)(resources.GetObject("groupBox1.IconPadding"))));
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this._translateComboBox, 1, 0);
            this._errorProvider.SetError(this.tableLayoutPanel2, resources.GetString("tableLayoutPanel2.Error"));
            this._errorProvider.SetIconAlignment(this.tableLayoutPanel2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tableLayoutPanel2.IconAlignment"))));
            this._errorProvider.SetIconPadding(this.tableLayoutPanel2, ((int)(resources.GetObject("tableLayoutPanel2.IconPadding"))));
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this._errorProvider.SetError(this.label2, resources.GetString("label2.Error"));
            this._errorProvider.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment"))));
            this._errorProvider.SetIconPadding(this.label2, ((int)(resources.GetObject("label2.IconPadding"))));
            this.label2.Name = "label2";
            // 
            // _translateComboBox
            // 
            resources.ApplyResources(this._translateComboBox, "_translateComboBox");
            this._translateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._errorProvider.SetError(this._translateComboBox, resources.GetString("_translateComboBox.Error"));
            this._translateComboBox.FormattingEnabled = true;
            this._errorProvider.SetIconAlignment(this._translateComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("_translateComboBox.IconAlignment"))));
            this._errorProvider.SetIconPadding(this._translateComboBox, ((int)(resources.GetObject("_translateComboBox.IconPadding"))));
            this._translateComboBox.Name = "_translateComboBox";
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox3, 2);
            this.groupBox3.Controls.Add(this.tableLayoutPanel4);
            this._errorProvider.SetError(this.groupBox3, resources.GetString("groupBox3.Error"));
            this._errorProvider.SetIconAlignment(this.groupBox3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox3.IconAlignment"))));
            this._errorProvider.SetIconPadding(this.groupBox3, ((int)(resources.GetObject("groupBox3.IconPadding"))));
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this._segmentationHintComboBox, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label3, 0, 0);
            this._errorProvider.SetError(this.tableLayoutPanel4, resources.GetString("tableLayoutPanel4.Error"));
            this._errorProvider.SetIconAlignment(this.tableLayoutPanel4, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tableLayoutPanel4.IconAlignment"))));
            this._errorProvider.SetIconPadding(this.tableLayoutPanel4, ((int)(resources.GetObject("tableLayoutPanel4.IconPadding"))));
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // _segmentationHintComboBox
            // 
            resources.ApplyResources(this._segmentationHintComboBox, "_segmentationHintComboBox");
            this._segmentationHintComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._errorProvider.SetError(this._segmentationHintComboBox, resources.GetString("_segmentationHintComboBox.Error"));
            this._segmentationHintComboBox.FormattingEnabled = true;
            this._errorProvider.SetIconAlignment(this._segmentationHintComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("_segmentationHintComboBox.IconAlignment"))));
            this._errorProvider.SetIconPadding(this._segmentationHintComboBox, ((int)(resources.GetObject("_segmentationHintComboBox.IconPadding"))));
            this._segmentationHintComboBox.Name = "_segmentationHintComboBox";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this._errorProvider.SetError(this.label3, resources.GetString("label3.Error"));
            this._errorProvider.SetIconAlignment(this.label3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label3.IconAlignment"))));
            this._errorProvider.SetIconPadding(this.label3, ((int)(resources.GetObject("label3.IconPadding"))));
            this.label3.Name = "label3";
            // 
            // _errorProvider
            // 
            this._errorProvider.ContainerControl = this;
            resources.ApplyResources(this._errorProvider, "_errorProvider");
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
    }
}