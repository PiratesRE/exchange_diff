using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class InfoWatsonThrottlingData
	{
		public InfoWatsonThrottlingData(string hash, DateTime nextAllowableLogTimeUtc)
		{
			this.Hash = hash;
			this.NextAllowableLogTimeUtc = nextAllowableLogTimeUtc;
			this.LastAccessTimeUtc = DateTime.UtcNow;
		}

		public DateTime NextAllowableLogTimeUtc { get; set; }

		public DateTime LastAccessTimeUtc { get; set; }

		public string Hash { get; private set; }
	}
}
