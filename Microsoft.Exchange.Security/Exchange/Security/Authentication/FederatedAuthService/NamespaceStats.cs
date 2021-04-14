using System;
using System.Threading;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class NamespaceStats
	{
		public bool IsBlacklisted
		{
			get
			{
				DateTime? dateTime = (DateTime?)this.blacklistEnds;
				return dateTime != null && DateTime.UtcNow < dateTime;
			}
		}

		public bool IsTarpitted
		{
			get
			{
				DateTime? dateTime = (DateTime?)this.tarpitEnds;
				return dateTime != null || DateTime.UtcNow < dateTime;
			}
		}

		public bool IsExpired
		{
			get
			{
				DateTime? dateTime = (DateTime?)this.expires;
				return dateTime == null || DateTime.UtcNow > dateTime;
			}
		}

		public DateTime BlacklistExpires
		{
			get
			{
				DateTime? dateTime = (DateTime?)this.blacklistEnds;
				if (dateTime == null)
				{
					return DateTime.UtcNow;
				}
				return dateTime.GetValueOrDefault();
			}
			set
			{
				this.blacklistEnds = value;
			}
		}

		public DateTime TarpitExpires
		{
			get
			{
				DateTime? dateTime = (DateTime?)this.tarpitEnds;
				if (dateTime == null)
				{
					return DateTime.UtcNow;
				}
				return dateTime.GetValueOrDefault();
			}
			set
			{
				this.tarpitEnds = value;
			}
		}

		public DateTime Expires
		{
			get
			{
				DateTime? dateTime = (DateTime?)this.expires;
				if (dateTime == null)
				{
					return DateTime.UtcNow;
				}
				return dateTime.GetValueOrDefault();
			}
			set
			{
				this.expires = value;
			}
		}

		public bool ClaimExpiration()
		{
			return Interlocked.Exchange(ref this.expires, null) != null;
		}

		public bool ClaimBlacklistExpired()
		{
			return Interlocked.Exchange(ref this.blacklistEnds, null) != null;
		}

		public bool ClaimTarpitExpired()
		{
			return Interlocked.Exchange(ref this.tarpitEnds, null) != null;
		}

		private object blacklistEnds;

		private object expires;

		private object tarpitEnds;

		public string Fqdn;

		public int Count;

		public int TimedOut;

		public int Failed;

		public int BadPassword;

		public int TokenSize;

		public DateTime Created = DateTime.UtcNow;

		public string User = string.Empty;

		public bool VerifiedNamespace;

		public int ADFSRulesDeny;
	}
}
