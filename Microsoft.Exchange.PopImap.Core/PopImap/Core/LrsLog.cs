using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class LrsLog
	{
		public LrsLog(string softwareName, Version softwareVersion, string logTypeName, string logFilePrefix, string logComponent)
		{
			this.schema = new LogSchema(softwareName, softwareVersion.ToString(), logTypeName, LrsLog.Fields);
			LogHeaderFormatter headerFormatter = new LogHeaderFormatter(this.schema, true);
			this.log = new Log(logFilePrefix, headerFormatter, logComponent);
			this.EventID = ProtocolBaseServices.ServiceName;
		}

		public string EventID { get; private set; }

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

		public LrsSession OpenSession(string primarySmtpAddress, IPEndPoint remoteEndPoint, IPEndPoint localEndPoint)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.schema);
			logRowFormatter[4] = this.EventID;
			logRowFormatter[6] = primarySmtpAddress;
			logRowFormatter[10] = "NA";
			LrsSession lrsSession = new LrsSession(this, logRowFormatter);
			lrsSession.SetEndpoints(remoteEndPoint, localEndPoint);
			return lrsSession;
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
				ProtocolBaseServices.ServerTracer.TraceError<ObjectDisposedException>(0L, "LRS Log Append failed with ObjectDisposedException\r\n{0}", arg);
			}
		}

		private static readonly string[] Fields = new string[]
		{
			"date-time",
			"client-ip",
			"server-ip",
			"message-id",
			"event-id",
			"sender-address",
			"recipient-address",
			"total-bytes",
			"recipient-count",
			"message-subject",
			"report-status"
		};

		private Log log;

		private LogSchema schema;

		private bool enabled;

		internal enum Field
		{
			Time,
			ClientIP,
			ServerIP,
			MessageID,
			EventID,
			Sender,
			RecipientAddresses,
			TotalBytes,
			RecipientCount,
			Subject,
			ReportStatus
		}
	}
}
