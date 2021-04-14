namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class ScheduleEditorDialog : global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeDialog
	{
		private void InitializeComponent()
		{
			this.scheduleEditor = new global::Microsoft.Exchange.Management.SystemManager.WinForms.ScheduleEditor();
			this.tableLayoutPanel = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoTableLayoutPanel();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.scheduleEditor.Dock = global::System.Windows.Forms.DockStyle.Top;
			this.scheduleEditor.AutoSize = true;
			this.scheduleEditor.Margin = new global::System.Windows.Forms.Padding(0);
			this.scheduleEditor.Name = "scheduleEditor";
			this.scheduleEditor.Size = new global::System.Drawing.Size(394, 470);
			this.scheduleEditor.TabIndex = 0;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.ColumnCount = 1;
			this.tableLayoutPanel.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 100f));
			this.tableLayoutPanel.Controls.Add(this.scheduleEditor, 0, 0);
			this.tableLayoutPanel.Dock = global::System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel.Location = new global::System.Drawing.Point(0, 0);
			this.tableLayoutPanel.Margin = new global::System.Windows.Forms.Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.Padding = new global::System.Windows.Forms.Padding(12, 12, 0, 16);
			this.tableLayoutPanel.RowCount = 1;
			this.tableLayoutPanel.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.Size = new global::System.Drawing.Size(406, 482);
			this.tableLayoutPanel.TabIndex = 0;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			base.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			base.Controls.Add(this.tableLayoutPanel);
			this.MinimumSize = new global::System.Drawing.Size(427, 0);
			base.Name = "ScheduleEditorDialog";
			base.ClientSize = new global::System.Drawing.Size(427, 470);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Controls.SetChildIndex(this.tableLayoutPanel, 0);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.ScheduleEditor scheduleEditor;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoTableLayoutPanel tableLayoutPanel;
	}
}
