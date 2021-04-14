using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.MailboxTransport.StoreDriverDelivery;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.MailboxTransport.Delivery
{
	internal class DeliveryThrottlingLog : IDeliveryThrottlingLog
	{
		public event Action<string> TrackSummary;

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
		}

		private static string[] InitializeHeaders()
		{
			string[] array = new string[Enum.GetValues(typeof(DeliveryThrottlingLog.Field)).Length];
			array[0] = "dateTime";
			array[1] = "eventID";
			array[2] = "sequenceNumber";
			array[3] = "scope";
			array[4] = "resource";
			array[5] = "threshold";
			array[6] = "impactUnits";
			array[7] = "impact";
			array[8] = "impactRate";
			array[9] = "tenantId";
			array[10] = "recipient";
			array[11] = "mdb";
			array[12] = "mdbHealth";
			array[13] = "customData";
			return array;
		}

		private string GetNextSequenceNumber()
		{
			this.sequenceNumber += 1L;
			return this.sequenceNumber.ToString("X16", NumberFormatInfo.InvariantInfo);
		}

		private void LogReset()
		{
			if (!this.enabled)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(DeliveryThrottlingLog.logSchema);
			logRowFormatter[1] = ThrottlingEvent.Reset;
			this.Append(logRowFormatter);
		}

		private void Append(LogRowFormatter row)
		{
			try
			{
				this.asyncLog.Append(row, 0);
			}
			catch (ObjectDisposedException)
			{
				ExTraceGlobals.StoreDriverDeliveryTracer.TraceDebug(0L, "Appending to Delivery Throttling log failed with ObjectDisposedException");
			}
		}

		public void Configure(IThrottlingConfig config)
		{
			ArgumentValidator.ThrowIfNull("config", config);
			if (!config.MailboxDeliveryThrottlingEnabled || !config.ThrottlingLogEnabled || config.ThrottlingLogPath == null || string.IsNullOrEmpty(config.ThrottlingLogPath.PathName))
			{
				this.enabled = false;
				return;
			}
			DeliveryThrottlingLog.logSchema = new LogSchema("Microsoft Mailbox Transport Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Mailbox Transport Delivery Throttling Log", DeliveryThrottlingLog.fields);
			this.asyncLog = new AsyncLog("DeliveryThrottling", new LogHeaderFormatter(DeliveryThrottlingLog.logSchema), "DeliveryThrottling");
			this.asyncLog.Configure(config.ThrottlingLogPath.PathName, config.ThrottlingLogMaxAge, (long)(config.ThrottlingLogMaxDirectorySize.IsUnlimited ? 0UL : config.ThrottlingLogMaxDirectorySize.Value.ToBytes()), (long)(config.ThrottlingLogMaxFileSize.IsUnlimited ? 0UL : config.ThrottlingLogMaxFileSize.Value.ToBytes()), config.ThrottlingLogBufferSize, config.ThrottlingLogFlushInterval, config.AsyncLogInterval);
			this.enabled = true;
			this.summaryLogTimer = new GuardedTimer(new TimerCallback(this.SummaryLogWorker), null, config.ThrottlingSummaryLoggingInterval, config.ThrottlingSummaryLoggingInterval);
			this.LogReset();
		}

		public void Close()
		{
			if (this.asyncLog != null)
			{
				this.asyncLog.Close();
				this.asyncLog = null;
			}
			if (this.summaryLogTimer != null)
			{
				this.summaryLogTimer.Dispose(true);
				this.summaryLogTimer = null;
			}
		}

		public void SummaryLogWorker(object state)
		{
			if (this.TrackSummary != null)
			{
				this.TrackSummary(this.GetNextSequenceNumber());
			}
		}

		public void LogSummary(string sequenceNumber, ThrottlingScope scope, ThrottlingResource resource, double resourceThreshold, ThrottlingImpactUnits impactUnits, uint impact, double impactRate, Guid externalOrganizationId, string recipient, string mdbName, IList<KeyValuePair<string, double>> mdbHealth, IList<KeyValuePair<string, string>> customData)
		{
			if (!this.enabled)
			{
				return;
			}
			ArgumentValidator.ThrowIfNullOrEmpty("sequenceNumber", sequenceNumber);
			LogRowFormatter logRowFormatter = new LogRowFormatter(DeliveryThrottlingLog.logSchema);
			logRowFormatter[2] = sequenceNumber;
			logRowFormatter[1] = ThrottlingEvent.SummaryThrottle;
			logRowFormatter[3] = scope;
			logRowFormatter[4] = resource;
			if (!resourceThreshold.Equals(double.NaN))
			{
				logRowFormatter[5] = resourceThreshold;
			}
			logRowFormatter[6] = impactUnits;
			logRowFormatter[7] = impact;
			logRowFormatter[8] = impactRate;
			if (externalOrganizationId != Guid.Empty)
			{
				logRowFormatter[9] = externalOrganizationId;
			}
			if (!string.IsNullOrEmpty(recipient))
			{
				logRowFormatter[10] = recipient;
			}
			if (!string.IsNullOrEmpty(mdbName))
			{
				logRowFormatter[11] = mdbName;
			}
			if (mdbHealth != null && mdbHealth.Count > 0)
			{
				logRowFormatter[12] = mdbHealth;
			}
			if (customData != null && customData.Count > 0)
			{
				logRowFormatter[13] = customData;
			}
			this.Append(logRowFormatter);
		}

		private const string ThrottlingLogFilePrefix = "DeliveryThrottling";

		private const string ThrottlingLogComponent = "DeliveryThrottling";

		private const string SoftwareName = "Microsoft Mailbox Transport Server";

		private const string LogType = "Mailbox Transport Delivery Throttling Log";

		private static readonly string[] fields = DeliveryThrottlingLog.InitializeHeaders();

		private static LogSchema logSchema;

		private bool enabled;

		private GuardedTimer summaryLogTimer;

		private AsyncLog asyncLog;

		private long sequenceNumber = DateTime.UtcNow.Ticks;

		private enum Field
		{
			Time,
			EventId,
			SequenceNumber,
			Scope,
			Resource,
			ResourceThreshold,
			ImpactUnits,
			Impact,
			ImpactRate,
			TenantID,
			Recipient,
			MDB,
			MDBHealth,
			CustomData
		}
	}
}
