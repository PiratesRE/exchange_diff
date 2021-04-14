namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract partial class ProxyAddressDialog : global::Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeDialog
	{
		private void InitializeComponent()
		{
			this.exchangePage = new global::Microsoft.Exchange.Management.SystemManager.WinForms.ProxyAddressDialog.ProxyAddressContentPage(this);
			base.SuspendLayout();
			this.exchangePage.AutoSize = true;
			this.exchangePage.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.exchangePage.Dock = global::System.Windows.Forms.DockStyle.Fill;
			base.Controls.Add(this.exchangePage);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::Microsoft.Exchange.Management.SystemManager.WinForms.ProxyAddressDialog.ProxyAddressContentPage exchangePage;
	}
}
