using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Transport.Logging
{
	internal class ProtocolLog : IProtocolLog
	{
		public ProtocolLog(string softwareName, Version softwareVersion, string logTypeName, string logFilePrefix, string logComponent)
		{
			this.schema = new LogSchema(softwareName, softwareVersion.ToString(), logTypeName, ProtocolLog.Fields);
			this.log = new AsyncLog(logFilePrefix, new LogHeaderFormatter(this.schema), logComponent);
			this.logMode = logFilePrefix;
		}

		public void Configure(LocalLongFullPath path, TimeSpan ageQuota, Unlimited<ByteQuantifiedSize> sizeQuota, Unlimited<ByteQuantifiedSize> perFileSizeQuota, int bufferSize, TimeSpan streamFlushInterval, TimeSpan backgroundWriteInterval)
		{
			this.Configure((path != null) ? path.PathName : null, ageQuota, (long)(sizeQuota.IsUnlimited ? 0UL : sizeQuota.Value.ToBytes()), (long)(perFileSizeQuota.IsUnlimited ? 0UL : perFileSizeQuota.Value.ToBytes()), bufferSize, streamFlushInterval, backgroundWriteInterval);
		}

		public void Configure(string path, TimeSpan ageQuota, long sizeQuota, long perFileSizeQuota, int bufferSize, TimeSpan streamFlushInterval, TimeSpan backgroundWriteInterval)
		{
			if (string.IsNullOrEmpty(path))
			{
				if (this.enabled)
				{
					if (string.CompareOrdinal(this.logMode, "SEND") == 0)
					{
						ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "SendProtocol Log output path was set to null, using old output path");
						Components.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SendProtocolLogPathIsNullUsingOld, null, new object[0]);
					}
					if (string.CompareOrdinal(this.logMode, "RECV") == 0)
					{
						ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "ReceiveProtocol Log output path was set to null using old output path");
						Components.EventLogger.LogEvent(TransportEventLogConstants.Tuple_ReceiveProtocolLogPathIsNullUsingOld, null, new object[0]);
						return;
					}
				}
				else
				{
					if (string.CompareOrdinal(this.logMode, "SEND") == 0)
					{
						ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "SendProtocol Log output path was set to null, SendProtocol Log is disabled");
						Components.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SendProtocolLogPathIsNull, null, new object[0]);
					}
					if (string.CompareOrdinal(this.logMode, "RECV") == 0)
					{
						ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "ReceiveProtocol Log output path was set to null, ReceiveProtocol Log is disabled");
						Components.EventLogger.LogEvent(TransportEventLogConstants.Tuple_ReceiveProtocolLogPathIsNull, null, new object[0]);
						return;
					}
				}
			}
			else
			{
				this.log.Configure(path, ageQuota, sizeQuota, perFileSizeQuota, bufferSize, streamFlushInterval, backgroundWriteInterval);
				this.enabled = true;
			}
		}

		public IProtocolLogSession OpenSession(string connectorId, ulong sessionId, IPEndPoint remoteEndPoint, IPEndPoint localEndPoint, ProtocolLoggingLevel loggingLevel)
		{
			ProtocolLogRowFormatter protocolLogRowFormatter = new ProtocolLogRowFormatter(this.schema);
			protocolLogRowFormatter[1] = connectorId;
			protocolLogRowFormatter[2] = sessionId.ToString("X16", NumberFormatInfo.InvariantInfo);
			protocolLogRowFormatter[5] = remoteEndPoint;
			protocolLogRowFormatter[4] = localEndPoint;
			return new ProtocolLogSession(this, protocolLogRowFormatter, loggingLevel);
		}

		public void Flush()
		{
			if (!this.enabled || this.closed)
			{
				return;
			}
			if (this.log != null)
			{
				this.log.Flush();
			}
		}

		public void Close()
		{
			this.closed = true;
			if (this.log != null)
			{
				this.log.Close();
			}
		}

		internal void Append(ProtocolLogRowFormatter row)
		{
			if (!this.enabled || this.closed)
			{
				return;
			}
			try
			{
				this.log.Append(row, 0);
			}
			catch (ObjectDisposedException)
			{
				ExTraceGlobals.GeneralTracer.TraceDebug((long)this.GetHashCode(), "Protocol Log Append failed with ObjectDisposedException");
			}
		}

		private static readonly string[] Fields = new string[]
		{
			"date-time",
			"connector-id",
			"session-id",
			"sequence-number",
			"local-endpoint",
			"remote-endpoint",
			"event",
			"data",
			"context"
		};

		private readonly AsyncLog log;

		private readonly LogSchema schema;

		private readonly string logMode;

		private bool enabled;

		private bool closed;

		internal enum Field
		{
			Time,
			ConnectorId,
			SessionId,
			SequenceNumber,
			LocalEndPoint,
			RemoteEndPoint,
			Event,
			Data,
			Context
		}
	}
}
