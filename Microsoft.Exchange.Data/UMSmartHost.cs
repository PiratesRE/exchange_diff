using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class UMSmartHost : SmartHost
	{
		public UMSmartHost(string address) : base(address)
		{
		}

		public string DisplayedAddress
		{
			get
			{
				return this.ToString();
			}
		}

		public new static UMSmartHost Parse(string address)
		{
			return new UMSmartHost(address);
		}

		public static bool IsValidAddress(string address)
		{
			SmartHost smartHost;
			return !address.EndsWith(".") && SmartHost.TryParse(address, out smartHost);
		}

		public override string ToString()
		{
			if (base.IsIPAddress)
			{
				return this.IpAddressToString();
			}
			return base.Domain.ToString();
		}

		private string IpAddressToString()
		{
			return base.Address.ToString();
		}
	}
}
