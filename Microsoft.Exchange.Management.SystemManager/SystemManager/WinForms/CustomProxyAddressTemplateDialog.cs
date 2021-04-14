using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class CustomProxyAddressTemplateDialog : CustomAddressDialog
	{
		protected override ProxyAddressBaseDataHandler DataHandler
		{
			get
			{
				if (this.dataHandler == null)
				{
					this.dataHandler = new ProxyAddressTemplateDataHandler();
				}
				return this.dataHandler;
			}
		}

		private ProxyAddressTemplateDataHandler dataHandler;
	}
}
