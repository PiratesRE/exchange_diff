using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class LightWeightLog
	{
		public LightWeightLog(string softwareName, Version softwareVersion, string logTypeName, string logFilePrefix, string logComponent)
		{
			this.schema = new LogSchema(softwareName, softwareVersion.ToString(), logTypeName, LightWeightLog.Fields);
			LogHeaderFormatter headerFormatter = new LogHeaderFormatter(this.schema, true);
			this.log = new Log(logFilePrefix, headerFormatter, logComponent);
		}

		public void Configure(string path, TimeSpan ageQuota, long sizeQuota, long perFileSizeQuota)
		{
			this.log.Configure(path, ageQuota, sizeQuota, perFileSizeQuota, false);
			this.enabled = true;
		}

		public void Configure(string path, LogFileRollOver logFileRollOverSettings, TimeSpan maxAge)
		{
			this.log.Configure(path, logFileRollOverSettings, maxAge);
			this.enabled = true;
		}

		public LightWeightLogSession OpenSession(ulong sessionId, IPEndPoint remoteEndPoint, IPEndPoint localEndPoint, ProtocolLoggingLevel loggingLevel)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.schema);
			logRowFormatter[1] = sessionId.ToString("X16", NumberFormatInfo.InvariantInfo);
			logRowFormatter[4] = remoteEndPoint.ToString();
			logRowFormatter[3] = localEndPoint.ToString();
			return new LightWeightLogSession(this, logRowFormatter);
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
			if (!this.enabled)
			{
				return;
			}
			try
			{
				this.log.Append(row, 0);
			}
			catch (ObjectDisposedException arg)
			{
				ProtocolBaseServices.ServerTracer.TraceError<ObjectDisposedException>(0L, "Protocol Log Append failed with ObjectDisposedException\r\n{0}", arg);
			}
		}

		private static readonly string[] Fields = new string[]
		{
			"dateTime",
			"sessionId",
			"seqNumber",
			"sIp",
			"cIp",
			"user",
			"duration",
			"rqsize",
			"rpsize",
			"command",
			"parameters",
			"context"
		};

		private Log log;

		private LogSchema schema;

		private bool enabled;

		internal enum Field
		{
			Time,
			SessionId,
			SequenceNumber,
			LocalEndPoint,
			RemoteEndPoint,
			User,
			Duration,
			RequestSize,
			ResponseSize,
			Command,
			Parameters,
			Context
		}
	}
}
