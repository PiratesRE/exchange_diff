using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal abstract class STSBase
	{
		protected static ExEventLog eventLogger
		{
			get
			{
				return AuthServiceHelper.EventLogger;
			}
		}

		protected static LiveIdBasicAuthenticationCountersInstance counters
		{
			get
			{
				return AuthServiceHelper.PerformanceCounters;
			}
		}

		protected STSBase()
		{
		}

		public STSBase(int traceId, LiveIdInstanceType instance, NamespaceStats stats)
		{
			this.traceId = traceId;
			this.ClockSkew = TimeSpan.Zero;
			this.ClockSkewThreshold = TimeSpan.FromMinutes(5.0);
			this.ServerTime = DateTime.MinValue;
			this.Instance = instance;
			this.namespaceStats = stats;
		}

		public LiveIdInstanceType Instance { get; private set; }

		public virtual string StsTag
		{
			get
			{
				return "";
			}
		}

		public NameValueCollection ExtraHeaders { get; set; }

		public bool CalculateClockSkew(HttpWebResponse hwr)
		{
			object obj = hwr.Headers["Date"];
			DateTime serverTime;
			if (obj != null && DateTime.TryParse(obj.ToString(), out serverTime))
			{
				this.ServerTime = serverTime;
				this.ClockSkew = serverTime.ToUniversalTime() - DateTime.UtcNow;
				return true;
			}
			return false;
		}

		public DateTime ServerTime { get; protected set; }

		public TimeSpan ClockSkew { get; set; }

		public TimeSpan ClockSkewThreshold { get; set; }

		public long Latency { get; protected set; }

		public long RPSParseLatency { get; protected set; }

		public long SSLConnectionLatency { get; protected set; }

		public bool IsBadCredentials { get; protected set; }

		public bool IsExpiredCreds { get; protected set; }

		public bool AppPasswordRequired { get; protected set; }

		public bool IsAccountNotProvisioned { get; protected set; }

		public bool IsUnfamiliarLocation { get; protected set; }

		public string RecoveryUrl { get; protected set; }

		public bool PossibleClockSkew { get; protected set; }

		public string ErrorString { get; protected internal set; }

		protected static void WriteBytes(Stream stream, byte[] bytes)
		{
			stream.Write(bytes, 0, bytes.Length);
		}

		protected NamespaceStats namespaceStats;

		protected int traceId;
	}
}
