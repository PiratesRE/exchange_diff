using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.EdgeSync;

namespace Microsoft.Exchange.EdgeSync.Logging
{
	internal class EdgeSyncLog
	{
		public EdgeSyncLog(string softwareName, Version softwareVersion, string logTypeName, string logFilePrefix, string logComponent)
		{
			this.schema = new LogSchema(softwareName, softwareVersion.ToString(), logTypeName, EdgeSyncLog.Fields);
			this.log = new Log(logFilePrefix, new LogHeaderFormatter(this.schema), logComponent);
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		public void Configure(string path, TimeSpan ageQuota, Unlimited<ByteQuantifiedSize> sizeQuota, Unlimited<ByteQuantifiedSize> perFileSizeQuota)
		{
			this.log.Configure(path, ageQuota, (long)(sizeQuota.IsUnlimited ? 262144000UL : sizeQuota.Value.ToBytes()), (long)(perFileSizeQuota.IsUnlimited ? 10485760UL : perFileSizeQuota.Value.ToBytes()));
		}

		public EdgeSyncLogSession OpenSession(string sessionId, string remoteServerFqdn, int remotePort, string localServerFqdn, EdgeSyncLoggingLevel loggingLevel)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.schema);
			logRowFormatter[1] = sessionId;
			logRowFormatter[4] = remoteServerFqdn;
			logRowFormatter[5] = remotePort;
			logRowFormatter[3] = localServerFqdn;
			return new EdgeSyncLogSession(this, logRowFormatter, loggingLevel);
		}

		public void Close()
		{
			if (this.log != null)
			{
				this.log.Close();
			}
		}

		internal void Append(LogRowFormatter row)
		{
			try
			{
				this.log.Append(row, 0);
			}
			catch (ObjectDisposedException)
			{
				ExTraceGlobals.ProcessTracer.TraceDebug((long)this.GetHashCode(), "EdgeSync Log Append failed with ObjectDisposedException");
			}
		}

		private const int DefaultQuota = 262144000;

		private const int DefaultPerFileQuota = 10485760;

		private static readonly string[] Fields = new string[]
		{
			"date-time",
			"session-id",
			"sequence-number",
			"local-serverfqdn",
			"remote-serverfqdn",
			"remote-port",
			"event",
			"level",
			"data",
			"context",
			"sync-mode",
			"sync-type",
			"dc"
		};

		private Log log;

		private LogSchema schema;

		private bool enabled;

		internal enum Field
		{
			Time,
			SessionId,
			SequenceNumber,
			LocalServerFqdn,
			RemoteServerFqdn,
			RemotePort,
			Event,
			Level,
			Data,
			Context,
			SyncMode,
			SyncType,
			DC
		}
	}
}
