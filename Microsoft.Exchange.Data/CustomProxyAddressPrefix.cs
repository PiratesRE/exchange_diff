using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class CustomProxyAddressPrefix : ProxyAddressPrefix
	{
		public CustomProxyAddressPrefix(string prefix) : base(prefix)
		{
			this.displayName = base.PrimaryPrefix;
		}

		public CustomProxyAddressPrefix(string prefix, string displayName) : base(prefix)
		{
			this.displayName = displayName;
		}

		public override ProxyAddress GetProxyAddress(string address, bool isPrimaryAddress)
		{
			return new CustomProxyAddress(this, address, isPrimaryAddress);
		}

		public override ProxyAddressTemplate GetProxyAddressTemplate(string addressTemplate, bool isPrimaryAddress)
		{
			return new CustomProxyAddressTemplate(this, addressTemplate, isPrimaryAddress);
		}

		public override string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		private readonly string displayName;
	}
}
