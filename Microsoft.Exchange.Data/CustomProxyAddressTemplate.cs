using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class CustomProxyAddressTemplate : ProxyAddressTemplate
	{
		public CustomProxyAddressTemplate(CustomProxyAddressPrefix prefix, string addressTemplate, bool isPrimaryAddress) : base(prefix, addressTemplate, isPrimaryAddress)
		{
		}
	}
}
