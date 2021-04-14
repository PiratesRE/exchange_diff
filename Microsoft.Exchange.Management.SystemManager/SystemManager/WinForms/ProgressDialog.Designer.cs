namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class ProgressDialog : global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeDialog
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				global::System.Windows.Forms.Application.Idle -= new global::System.EventHandler(this.Application_Idle);
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.progressBar = new global::Microsoft.ManagementGUI.WinForms.ExchangeProgressBar();
			this.statusLabel = new global::System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoTableLayoutPanel();
			this.tableLayoutPanel1.SuspendLayout();
			base.SuspendLayout();
			this.progressBar.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.progressBar.Location = new global::System.Drawing.Point(16, 28);
			this.progressBar.Margin = new global::System.Windows.Forms.Padding(3, 3, 0, 0);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new global::System.Drawing.Size(282, 12);
			this.progressBar.Style = global::System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBar.TabIndex = 1;
			this.statusLabel.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.statusLabel.AutoSize = true;
			this.statusLabel.Location = new global::System.Drawing.Point(13, 12);
			this.statusLabel.Margin = new global::System.Windows.Forms.Padding(0);
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new global::System.Drawing.Size(285, 13);
			this.statusLabel.TabIndex = 0;
			this.statusLabel.Text = "statusLabel";
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 50f));
			this.tableLayoutPanel1.Controls.Add(this.progressBar, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.statusLabel, 0, 0);
			this.tableLayoutPanel1.Dock = global::System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel1.AutoLayout = true;
			this.tableLayoutPanel1.Location = new global::System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new global::System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new global::System.Windows.Forms.Padding(13, 12, 16, 12);
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new global::System.Drawing.Size(314, 52);
			this.tableLayoutPanel1.TabIndex = 0;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.ClientSize = new global::System.Drawing.Size(314, 96);
			base.Controls.Add(this.tableLayoutPanel1);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Name = "ProgressDialog";
			base.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::Microsoft.ManagementGUI.WinForms.ExchangeProgressBar progressBar;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoTableLayoutPanel tableLayoutPanel1;

		private global::System.Windows.Forms.Label statusLabel;
	}
}
