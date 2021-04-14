using System;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal sealed class ADReplicationLinkMetrics
	{
		public ADReplicationLinkMetrics(string partnerDnsHostName, long upToDatenessUsn)
		{
			this.NeighborDnsHostName = partnerDnsHostName;
			this.UpToDatenessUsn = upToDatenessUsn;
		}

		public string NeighborDnsHostName { get; private set; }

		public long UpToDatenessUsn { get; private set; }

		public long Debt { get; set; }
	}
}
