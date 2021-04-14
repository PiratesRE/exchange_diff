namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract partial class CustomAddressDialog : global::Microsoft.Exchange.Management.SystemManager.WinForms.ProxyAddressDialog
	{
		private void InitializeComponent()
		{
			this.components = new global::System.ComponentModel.Container();
			this.addressLabel = new global::System.Windows.Forms.Label();
			this.addressTextBox = new global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeTextBox();
			this.prefixLabel = new global::System.Windows.Forms.Label();
			this.prefixTextBox = new global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeTextBox();
			this.proxyErrorProvider = new global::System.Windows.Forms.ErrorProvider(this.components);
			this.tableLayoutPanel1 = new global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoTableLayoutPanel();
			((global::System.ComponentModel.ISupportInitialize)this.proxyErrorProvider).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			base.ContentPage.SuspendLayout();
			base.SuspendLayout();
			this.addressLabel.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.addressLabel.AutoSize = true;
			this.addressLabel.Location = new global::System.Drawing.Point(13, 12);
			this.addressLabel.Margin = new global::System.Windows.Forms.Padding(0);
			this.addressLabel.Name = "addressLabel";
			this.addressLabel.Size = new global::System.Drawing.Size(400, 13);
			this.addressLabel.TabIndex = 0;
			this.addressLabel.Text = "addressLabel";
			this.addressTextBox.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.addressTextBox.Location = new global::System.Drawing.Point(16, 28);
			this.addressTextBox.Margin = new global::System.Windows.Forms.Padding(3, 3, 0, 0);
			this.addressTextBox.Name = "addressTextBox";
			this.addressTextBox.Size = new global::System.Drawing.Size(397, 20);
			this.addressTextBox.TabIndex = 1;
			this.addressTextBox.MaxLength = global::Microsoft.Exchange.Data.ProxyAddressBase.MaxLength - 2;
			this.prefixLabel.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.prefixLabel.AutoSize = true;
			this.prefixLabel.Location = new global::System.Drawing.Point(13, 60);
			this.prefixLabel.Margin = new global::System.Windows.Forms.Padding(0, 12, 0, 0);
			this.prefixLabel.Name = "prefixLabel";
			this.prefixLabel.Size = new global::System.Drawing.Size(400, 13);
			this.prefixLabel.TabIndex = 2;
			this.prefixLabel.Text = "prefixLabel";
			this.prefixTextBox.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.prefixTextBox.Location = new global::System.Drawing.Point(16, 76);
			this.prefixTextBox.Margin = new global::System.Windows.Forms.Padding(3, 3, 0, 0);
			this.prefixTextBox.Name = "prefixTextBox";
			this.prefixTextBox.Size = new global::System.Drawing.Size(397, 20);
			this.prefixTextBox.TabIndex = 3;
			this.prefixTextBox.MaxLength = 9;
			this.proxyErrorProvider.BlinkStyle = global::System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this.proxyErrorProvider.ContainerControl = this;
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 100f));
			this.tableLayoutPanel1.Controls.Add(this.addressLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.prefixTextBox, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.prefixLabel, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.addressTextBox, 0, 1);
			this.tableLayoutPanel1.Dock = global::System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel1.AutoLayout = true;
			this.tableLayoutPanel1.Location = new global::System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new global::System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new global::System.Windows.Forms.Padding(13, 12, 16, 12);
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new global::System.Drawing.Size(429, 108);
			this.tableLayoutPanel1.TabIndex = 0;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelVisible = true;
			base.ClientSize = new global::System.Drawing.Size(429, 225);
			base.Margin = new global::System.Windows.Forms.Padding(5, 3, 5, 3);
			base.Name = "CustomAddressDialog";
			base.OkEnabled = true;
			base.ContentPage.Controls.Add(this.tableLayoutPanel1);
			base.Controls.SetChildIndex(base.ContentPage, 0);
			((global::System.ComponentModel.ISupportInitialize)this.proxyErrorProvider).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			base.ContentPage.ResumeLayout(false);
			base.ContentPage.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::System.Windows.Forms.Label addressLabel;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeTextBox addressTextBox;

		private global::System.Windows.Forms.Label prefixLabel;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeTextBox prefixTextBox;

		private global::System.Windows.Forms.ErrorProvider proxyErrorProvider;

		private global::System.ComponentModel.IContainer components;

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.AutoTableLayoutPanel tableLayoutPanel1;
	}
}
