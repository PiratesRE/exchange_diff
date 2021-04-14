using System;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class ChainBuildParameter
	{
		public ChainBuildParameter(ChainMatchIssuer match, TimeSpan timeout, bool overrideCRLTime, TimeSpan freshnessTime)
		{
			this.match = match;
			this.urlRetrievalTimeout = timeout;
			this.overrideRevocationTime = overrideCRLTime;
			this.freshnessDelta = freshnessTime;
		}

		internal ChainMatchIssuer Match
		{
			get
			{
				return this.match;
			}
			set
			{
				this.match = value;
			}
		}

		public TimeSpan UrlRetrievalTimeout
		{
			get
			{
				return this.urlRetrievalTimeout;
			}
			set
			{
				this.urlRetrievalTimeout = value;
			}
		}

		public bool OverrideRevocationTime
		{
			get
			{
				return this.overrideRevocationTime;
			}
			set
			{
				this.overrideRevocationTime = value;
			}
		}

		public TimeSpan RevocationFreshnessDelta
		{
			get
			{
				return this.freshnessDelta;
			}
			set
			{
				this.freshnessDelta = value;
			}
		}

		private ChainMatchIssuer match;

		private TimeSpan urlRetrievalTimeout;

		private bool overrideRevocationTime;

		private TimeSpan freshnessDelta;
	}
}
