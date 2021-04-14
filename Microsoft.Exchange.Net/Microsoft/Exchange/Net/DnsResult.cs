using System;
using System.Net;

namespace Microsoft.Exchange.Net
{
	internal class DnsResult
	{
		internal DnsResult(DnsStatus status, IPAddress server, TimeSpan timeToLive) : this(status, server, null, timeToLive)
		{
		}

		internal DnsResult(DnsStatus status, IPAddress server, DnsRecordList list, TimeSpan timeToLive)
		{
			this.status = status;
			this.list = list;
			this.server = server;
			if (timeToLive == DnsResult.NoExpiration)
			{
				this.expires = DnsResult.PermanentEntryDate;
			}
			else
			{
				this.expires = DateTime.UtcNow + timeToLive;
			}
			this.UpdateLastAccess();
		}

		internal DnsStatus Status
		{
			get
			{
				return this.status;
			}
		}

		internal DnsRecordList List
		{
			get
			{
				return this.list;
			}
		}

		internal IPAddress Server
		{
			get
			{
				return this.server;
			}
		}

		internal DateTime LastAccess
		{
			get
			{
				return this.lastAccess;
			}
			set
			{
				this.lastAccess = value;
			}
		}

		internal DateTime Expires
		{
			get
			{
				return this.expires;
			}
			set
			{
				this.expires = value;
			}
		}

		internal bool HasExpired
		{
			get
			{
				return this.expires <= DateTime.UtcNow;
			}
		}

		internal bool IsPermanentEntry
		{
			get
			{
				return this.expires == DnsResult.PermanentEntryDate;
			}
		}

		internal static TimeSpan TimeToLiveWithinLimits(TimeSpan timeToLive)
		{
			if (timeToLive < DnsResult.MinTimeToLive)
			{
				return DnsResult.MinTimeToLive;
			}
			if (timeToLive > DnsResult.MaxTimeToLive)
			{
				return DnsResult.MaxTimeToLive;
			}
			return timeToLive;
		}

		internal void UpdateLastAccess()
		{
			if (!this.IsPermanentEntry)
			{
				this.lastAccess = DateTime.UtcNow;
			}
		}

		public override string ToString()
		{
			return string.Format("Server={0}; Status={1}; Results={2}; Expires={3}", new object[]
			{
				this.server,
				this.Status,
				this.List,
				this.expires
			});
		}

		internal static readonly TimeSpan DefaultTimeToLive = TimeSpan.FromMinutes(60.0);

		internal static readonly TimeSpan MinTimeToLive = TimeSpan.FromMinutes(5.0);

		internal static readonly TimeSpan MaxTimeToLive = TimeSpan.FromMinutes(60.0);

		internal static readonly TimeSpan ErrorTimeToLive = TimeSpan.FromMinutes(5.0);

		internal static readonly TimeSpan NoExpiration = TimeSpan.MaxValue;

		private static readonly DateTime PermanentEntryDate = DateTime.MaxValue;

		private readonly DnsStatus status;

		private readonly DnsRecordList list;

		private IPAddress server;

		private DateTime expires;

		private DateTime lastAccess;
	}
}
