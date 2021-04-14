using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	internal class NSRecord
	{
		public string DomainName { get; private set; }

		public int ProcessResponse(byte[] message, int position)
		{
			this.DomainName = DnsHelper.ReadDomain(message, ref position);
			if (string.IsNullOrWhiteSpace(this.DomainName))
			{
				throw new FormatException(string.Format("NSRecord domainName is empty", new object[0]));
			}
			return position;
		}

		public override string ToString()
		{
			return string.Format("NS={0}", this.DomainName);
		}
	}
}
