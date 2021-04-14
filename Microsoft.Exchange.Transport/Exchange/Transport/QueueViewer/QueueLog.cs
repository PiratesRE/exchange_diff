using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net.DiagnosticsAggregation;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Transport.QueueViewer
{
	internal class QueueLog
	{
		public static void Start()
		{
			if (!Components.TransportAppConfig.QueueConfiguration.QueueLoggingEnabled)
			{
				return;
			}
			QueueLog.queueLogSchema = new LogSchema("Microsoft Exchange Server", "15.00.1497.015", "Transport Queue Log", QueueLog.Fields);
			Server transportServer = Components.Configuration.LocalServer.TransportServer;
			string text = (transportServer.QueueLogPath == null) ? null : transportServer.QueueLogPath.PathName;
			if (!string.IsNullOrEmpty(text))
			{
				QueueLog.log = new Log(QueueLogSchema.LogPrefix, new LogHeaderFormatter(QueueLog.queueLogSchema), "QueueLogs");
				QueueLog.log.Configure(text, transportServer.QueueLogMaxAge, (long)(transportServer.QueueLogMaxDirectorySize.IsUnlimited ? 9223372036854775807UL : transportServer.QueueLogMaxDirectorySize.Value.ToBytes()), (long)(transportServer.QueueLogMaxFileSize.IsUnlimited ? 9223372036854775807UL : transportServer.QueueLogMaxFileSize.Value.ToBytes()), 1048576, TimeSpan.FromSeconds(2.0));
				QueueLog.enabled = true;
				QueueLog.LogServiceStart();
			}
		}

		public static void LogServiceStart()
		{
			if (!QueueLog.enabled)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(QueueLog.queueLogSchema);
			logRowFormatter[20] = "Started MsExchangeTransport service.";
			logRowFormatter[2] = QueueLogEventId.START;
			QueueLog.log.Append(logRowFormatter, 0);
		}

		public static void Log(QueueAggregationInfo queueObject)
		{
			if (!QueueLog.enabled)
			{
				return;
			}
			int num = 0;
			foreach (LocalQueueInfo localQueueInfo in queueObject.QueueInfo)
			{
				QueueLog.LogQueueInfo(localQueueInfo, queueObject.Time, ++num);
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(QueueLog.queueLogSchema);
			logRowFormatter[0] = DateTime.UtcNow;
			logRowFormatter[2] = QueueLogEventId.SUMMARY;
			logRowFormatter[20] = string.Format("TotalMessageCount = {0}; PoisonMessageCount = {1}", queueObject.TotalMessageCount, queueObject.PoisonMessageCount);
			QueueLog.Append(logRowFormatter);
		}

		public static void Stop()
		{
			if (QueueLog.log != null)
			{
				QueueLog.enabled = false;
				QueueLog.log.Close();
				QueueLog.log = null;
			}
		}

		private static string[] InitializeFields()
		{
			string[] array = new string[Enum.GetValues(typeof(QueueLog.QueueLogField)).Length];
			array[0] = "Timestamp";
			array[1] = "SequenceNumber";
			array[2] = "EventId";
			array[3] = "QueueIdentity";
			array[4] = "Status";
			array[5] = "DeliveryType";
			array[6] = "NextHopDomain";
			array[8] = "MessageCount";
			array[7] = "NextHopKey";
			array[10] = "LockedMessageCount";
			array[9] = "DeferredMessageCount";
			array[11] = "IncomingRate";
			array[14] = "NextHopCategory";
			array[12] = "OutgoingRate";
			array[15] = "RiskLevel";
			array[16] = "OutboundIPPool";
			array[19] = "LastError";
			array[13] = "Velocity";
			array[17] = "NextHopConnector";
			array[18] = "TlsDomain";
			array[20] = "Data";
			array[21] = "CustomData";
			return array;
		}

		private static void Append(LogRowFormatter row)
		{
			try
			{
				QueueLog.log.Append(row, -1);
			}
			catch (ObjectDisposedException)
			{
				ExTraceGlobals.MessageTrackingTracer.TraceDebug(0L, "Message tracking append failed with ObjectDisposedException");
			}
		}

		private static void LogQueueInfo(LocalQueueInfo localQueueInfo, DateTime timeToStamp, int sequenceNumber)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(QueueLog.queueLogSchema);
			logRowFormatter[0] = timeToStamp;
			logRowFormatter[1] = sequenceNumber;
			logRowFormatter[2] = QueueLogEventId.QUEUE;
			logRowFormatter[3] = localQueueInfo.QueueIdentity;
			logRowFormatter[4] = localQueueInfo.Status;
			logRowFormatter[5] = localQueueInfo.DeliveryType;
			logRowFormatter[6] = localQueueInfo.NextHopDomain;
			logRowFormatter[8] = localQueueInfo.MessageCount;
			logRowFormatter[7] = localQueueInfo.NextHopKey;
			logRowFormatter[9] = localQueueInfo.DeferredMessageCount;
			logRowFormatter[10] = localQueueInfo.LockedMessageCount;
			logRowFormatter[11] = localQueueInfo.IncomingRate;
			logRowFormatter[12] = localQueueInfo.OutgoingRate;
			logRowFormatter[13] = localQueueInfo.Velocity;
			logRowFormatter[14] = localQueueInfo.NextHopCategory;
			logRowFormatter[15] = localQueueInfo.RiskLevel;
			logRowFormatter[16] = localQueueInfo.OutboundIPPool;
			logRowFormatter[17] = localQueueInfo.NextHopConnector;
			logRowFormatter[18] = localQueueInfo.TlsDomain;
			string text = localQueueInfo.LastError;
			if (!string.IsNullOrEmpty(text) && text.Length > 100)
			{
				text = localQueueInfo.LastError.Substring(0, 100);
			}
			logRowFormatter[19] = text;
			QueueLog.Append(logRowFormatter);
		}

		private const string LogComponentName = "QueueLogs";

		private static readonly string[] Fields = QueueLog.InitializeFields();

		private static LogSchema queueLogSchema;

		private static Log log;

		private static bool enabled;

		internal enum QueueLogField
		{
			Time,
			SequenceNumber,
			EventId,
			QueueIdentity,
			Status,
			DeliveryType,
			NextHopDomain,
			NextHopKey,
			MessageCount,
			DeferredMessageCount,
			LockedMessageCount,
			IncomingRate,
			OutgoingRate,
			Velocity,
			NextHopCategory,
			RiskLevel,
			OutboundIPPool,
			NextHopConnector,
			TlsDomain,
			LastError,
			Data,
			CustomData
		}
	}
}
