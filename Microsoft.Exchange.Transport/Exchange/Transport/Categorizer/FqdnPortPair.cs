using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class FqdnPortPair
	{
		public FqdnPortPair(string fqdn, ushort port)
		{
			this.fqdn = fqdn;
			this.port = port;
		}

		public override bool Equals(object obj)
		{
			FqdnPortPair fqdnPortPair = obj as FqdnPortPair;
			return fqdnPortPair != null && this.fqdn.Equals(fqdnPortPair.fqdn, StringComparison.OrdinalIgnoreCase) && this.port == fqdnPortPair.port;
		}

		public override int GetHashCode()
		{
			return StringComparer.OrdinalIgnoreCase.GetHashCode(this.fqdn) ^ (int)this.port;
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", this.fqdn, this.port.ToString());
		}

		private readonly string fqdn;

		private readonly ushort port;
	}
}
