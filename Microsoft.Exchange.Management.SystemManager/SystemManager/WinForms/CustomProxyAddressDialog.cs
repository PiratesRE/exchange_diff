using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class CustomProxyAddressDialog : CustomAddressDialog
	{
		protected override ProxyAddressBaseDataHandler DataHandler
		{
			get
			{
				if (this.dataHandler == null)
				{
					this.dataHandler = new ProxyAddressDataHandler();
				}
				return this.dataHandler;
			}
		}

		private ProxyAddressDataHandler dataHandler;
	}
}
