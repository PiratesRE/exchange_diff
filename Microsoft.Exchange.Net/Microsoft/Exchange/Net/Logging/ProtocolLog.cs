using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net.Logging
{
	internal sealed class ProtocolLog : IDisposable
	{
		public ProtocolLog(ProtocolLogConfiguration protocolLogConfiguration)
		{
			ArgumentValidator.ThrowIfNull("protocolLogConfiguration", protocolLogConfiguration);
			this.isEnabled = protocolLogConfiguration.IsEnabled;
			this.schema = new LogSchema(protocolLogConfiguration.SoftwareName, protocolLogConfiguration.SoftwareVersion.ToString(), protocolLogConfiguration.LogTypeName, ProtocolLog.Fields);
			this.log = new Log(protocolLogConfiguration.LogFilePrefix, new LogHeaderFormatter(this.schema), protocolLogConfiguration.LogComponent);
			this.ConfigureLog(protocolLogConfiguration.LogFilePath, protocolLogConfiguration.AgeQuota, protocolLogConfiguration.DirectorySizeQuota, protocolLogConfiguration.PerFileSizeQuota, protocolLogConfiguration.ProtocolLoggingLevel);
			ExTraceGlobals.ProtocolLogTracer.TraceDebug((long)this.GetHashCode(), "Protocol Log Created and Configured");
		}

		internal ProtocolLoggingLevel LoggingLevel
		{
			get
			{
				return this.loggingLevel;
			}
		}

		public void Dispose()
		{
			this.Close();
		}

		public void ConfigureLog(string path, long ageQuota, long directorySizeQuota, long perFileSizeQuota, ProtocolLoggingLevel loggingLevel)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("path", path);
			ProtocolLog.ThrowIfArgumentLessThanZero("ageQuota", ageQuota);
			ProtocolLog.ThrowIfArgumentLessThanZero("directorySizeQuota", directorySizeQuota);
			ProtocolLog.ThrowIfArgumentLessThanZero("perFileSizeQuota", perFileSizeQuota);
			ProtocolLog.ThrowIfArg1LessThenArg2("directorySizeQuota", directorySizeQuota, "perFileSizeQuota", perFileSizeQuota);
			this.log.Configure(path, TimeSpan.FromHours((double)ageQuota), directorySizeQuota * 1024L, perFileSizeQuota * 1024L);
			this.loggingLevel = loggingLevel;
			ExTraceGlobals.ProtocolLogTracer.TraceDebug((long)this.GetHashCode(), "Protocol Logs Configured");
		}

		public ProtocolLogSession OpenSession(ulong sessionId, string remoteServer, string localServer)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.schema);
			logRowFormatter[1] = sessionId.ToString("X16", NumberFormatInfo.InvariantInfo);
			logRowFormatter[3] = remoteServer;
			logRowFormatter[2] = localServer;
			return new ProtocolLogSession(this, logRowFormatter);
		}

		public void Close()
		{
			if (this.log != null)
			{
				this.log.Close();
			}
			this.isEnabled = false;
			ExTraceGlobals.ProtocolLogTracer.TraceDebug((long)this.GetHashCode(), "Protocol Logs Closed");
		}

		internal void Append(LogRowFormatter row)
		{
			if (!this.isEnabled)
			{
				return;
			}
			this.log.Append(row, 0);
		}

		private static void ThrowIfArgumentLessThanZero(string name, long arg)
		{
			if (arg < 0L)
			{
				throw new ArgumentOutOfRangeException(name, arg, "The value is set to less than 0.");
			}
		}

		private static void ThrowIfArg1LessThenArg2(string name1, long arg1, string name2, long arg2)
		{
			if (arg1 < arg2)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The value of {0} is set to less than {1}.", new object[]
				{
					name1,
					name2
				}));
			}
		}

		private static readonly string[] Fields = new string[]
		{
			"date-time",
			"session-id",
			"local-server",
			"remote-server",
			"event",
			"data",
			"context"
		};

		private Log log;

		private LogSchema schema;

		private bool isEnabled;

		private ProtocolLoggingLevel loggingLevel;

		internal enum Field
		{
			Time,
			SessionId,
			LocalServer,
			RemoteServer,
			Event,
			Data,
			Context
		}
	}
}
