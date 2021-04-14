using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class CustomProxyAddress : ProxyAddress
	{
		public CustomProxyAddress(CustomProxyAddressPrefix prefix, string address, bool isPrimaryAddress) : base(prefix, address, isPrimaryAddress)
		{
			if (string.IsNullOrEmpty(prefix.PrimaryPrefix))
			{
				throw new ArgumentOutOfRangeException(DataStrings.ExceptionEmptyPrefix(address), null);
			}
		}
	}
}
