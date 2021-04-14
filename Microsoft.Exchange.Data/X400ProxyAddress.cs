using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class X400ProxyAddress : ProxyAddress
	{
		public X400ProxyAddress(string address, bool isPrimaryAddress) : this(X400AddressParser.GetCanonical(address, false, out address), address, isPrimaryAddress)
		{
		}

		private X400ProxyAddress(bool endingWithSemicolon, string address, bool isPrimaryAddress) : base(ProxyAddressPrefix.X400, address, isPrimaryAddress)
		{
			this.endingWithSemicolon = endingWithSemicolon;
		}

		internal bool EndingWithSemicolon
		{
			get
			{
				return this.endingWithSemicolon;
			}
		}

		private bool endingWithSemicolon;
	}
}
