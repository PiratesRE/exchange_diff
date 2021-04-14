using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	internal class SOARecord
	{
		public string PrimaryNameServer { get; private set; }

		public string ResponsibleMailAddress { get; private set; }

		public int Serial { get; private set; }

		public int Refresh { get; private set; }

		public int Retry { get; private set; }

		public int Expire { get; private set; }

		public int DefaultTTL { get; private set; }

		public int ProcessResponse(byte[] message, int position)
		{
			int num = position;
			this.PrimaryNameServer = DnsHelper.ReadDomain(message, ref num);
			this.ResponsibleMailAddress = DnsHelper.ReadDomain(message, ref num);
			this.Serial = DnsHelper.GetInt(message, num);
			num += 4;
			this.Refresh = DnsHelper.GetInt(message, num);
			num += 4;
			this.Retry = DnsHelper.GetInt(message, num);
			num += 4;
			this.Expire = DnsHelper.GetInt(message, num);
			num += 4;
			this.DefaultTTL = DnsHelper.GetInt(message, num);
			num += 4;
			if (string.IsNullOrWhiteSpace(this.PrimaryNameServer))
			{
				throw new FormatException("SOARecord PrimaryNameServer is empty");
			}
			return num;
		}

		public override string ToString()
		{
			return string.Format("SOA PrimaryNameServer={0}, ResponsibleMailAddress={1}", this.PrimaryNameServer, this.ResponsibleMailAddress);
		}
	}
}
