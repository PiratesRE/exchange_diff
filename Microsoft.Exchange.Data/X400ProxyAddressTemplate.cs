using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class X400ProxyAddressTemplate : ProxyAddressTemplate
	{
		public X400ProxyAddressTemplate(string addressTemplate, bool isPrimaryAddress) : base(ProxyAddressPrefix.X400, addressTemplate, isPrimaryAddress)
		{
		}
	}
}
