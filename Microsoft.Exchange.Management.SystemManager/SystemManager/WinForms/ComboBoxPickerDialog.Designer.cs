namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class ComboBoxPickerDialog : global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeDialog
	{
		private void InitializeComponent()
		{
			this.tableLayoutPanel = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoTableLayoutPanel();
			this.comboBox = new global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeComboBox();
			this.label = new global::System.Windows.Forms.Label();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 1;
			this.tableLayoutPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel.Controls.Add(this.comboBox, 0, 1);
			this.tableLayoutPanel.Controls.Add(this.label, 0, 0);
			this.tableLayoutPanel.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel.AutoLayout = false;
			this.tableLayoutPanel.Location = new global::System.Drawing.Point(0, 0);
			this.tableLayoutPanel.Margin = new global::System.Windows.Forms.Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.Padding = new global::System.Windows.Forms.Padding(13, 12, 16, 12);
			this.tableLayoutPanel.RowCount = 2;
			this.tableLayoutPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.Size = new global::System.Drawing.Size(370, 60);
			this.tableLayoutPanel.TabIndex = 0;
			this.comboBox.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.comboBox.DropDownStyle = global::System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox.FormattingEnabled = true;
			this.comboBox.Location = new global::System.Drawing.Point(16, 28);
			this.comboBox.Margin = new global::System.Windows.Forms.Padding(3, 3, 0, 0);
			this.comboBox.Name = "comboBox";
			this.comboBox.Size = new global::System.Drawing.Size(338, 21);
			this.comboBox.TabIndex = 1;
			this.label.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.label.AutoSize = true;
			this.label.Location = new global::System.Drawing.Point(13, 12);
			this.label.Margin = new global::System.Windows.Forms.Padding(0);
			this.label.Name = "label";
			this.label.Size = new global::System.Drawing.Size(341, 13);
			this.label.TabIndex = 0;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			this.AutoSize = true;
			base.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			base.CancelVisible = true;
			base.ClientSize = new global::System.Drawing.Size(370, 99);
			base.Controls.Add(this.tableLayoutPanel);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Name = "ComboBoxPickerDialog";
			base.Controls.SetChildIndex(this.tableLayoutPanel, 0);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoTableLayoutPanel tableLayoutPanel;

		private global::System.Windows.Forms.ComboBox comboBox;

		private global::System.Windows.Forms.Label label;
	}
}
