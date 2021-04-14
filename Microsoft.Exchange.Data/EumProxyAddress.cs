using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class EumProxyAddress : ProxyAddress
	{
		public EumProxyAddress(string address, bool isPrimaryAddress) : base(ProxyAddressPrefix.UM, address, isPrimaryAddress)
		{
			if (Microsoft.Exchange.Data.EumAddress.IsValidEumAddress(address))
			{
				this.eumAddress = address;
				return;
			}
			throw new ArgumentOutOfRangeException(DataStrings.ExceptionInvalidEumAddress(address), null);
		}

		public string EumAddress
		{
			get
			{
				return this.eumAddress;
			}
		}

		public static explicit operator EumAddress(EumProxyAddress value)
		{
			return new EumAddress(value.AddressString);
		}

		private static bool Is1252LetterOrDigit(char c)
		{
			return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9');
		}

		private const string HexDigits = "0123456789ABCDEF";

		private string eumAddress;
	}
}
