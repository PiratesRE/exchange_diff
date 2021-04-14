using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class StoreDriverDeliveryDiagnostics : IDiagnosable
	{
		internal static Breadcrumbs<DatabaseHealthBreadcrumb> HealthHistory
		{
			get
			{
				return StoreDriverDeliveryDiagnostics.healthHistory;
			}
		}

		protected static TroubleshootingContext TroubleshootingContext
		{
			get
			{
				return StoreDriverDeliveryDiagnostics.transportTroubleshootingContext;
			}
		}

		protected static DeliveriesInProgress HangDetector
		{
			get
			{
				return StoreDriverDeliveryDiagnostics.hangDetector;
			}
		}

		protected static int DeliveringThreads
		{
			get
			{
				return StoreDriverDeliveryDiagnostics.deliveringThreads;
			}
		}

		public static void LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			StoreDriverDeliveryDiagnostics.eventLogger.LogEvent(tuple, periodicKey, messageArgs);
		}

		public static void LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			StoreDriverDeliveryDiagnostics.eventLogger.LogEvent(organizationId, tuple, periodicKey, messageArgs);
		}

		public static void Initialize()
		{
			StoreDriverDeliveryDiagnostics.hangDetector = new DeliveriesInProgress(Components.Configuration.LocalServer.MaxConcurrentMailboxDeliveries);
		}

		public static bool DetectDeliveryHang(out string hangBreadcrumb)
		{
			if (StoreDriverDeliveryDiagnostics.HangDetector == null)
			{
				StoreDriverDeliveryDiagnostics.Diag.TraceDebug(0L, "Store driver component is being initialized and HangDetector hasn't been created yet, we are not hanging.");
				hangBreadcrumb = null;
				return false;
			}
			ulong num;
			MailItemDeliver mailItemDeliver;
			if (StoreDriverDeliveryDiagnostics.HangDetector.DetectHang(Components.Configuration.AppConfig.RemoteDelivery.StoreDriverRecipientDeliveryHangThreshold, out num, out mailItemDeliver))
			{
				hangBreadcrumb = mailItemDeliver.DeliveryBreadcrumb.ToString();
				string[] messageArgs = new string[]
				{
					num.ToString(),
					(mailItemDeliver.Recipient == null) ? "NotSet" : mailItemDeliver.Recipient.Email.ToString(),
					Components.Configuration.AppConfig.RemoteDelivery.StoreDriverRecipientDeliveryHangThreshold.ToString(),
					hangBreadcrumb
				};
				if (mailItemDeliver.MbxTransportMailItem != null)
				{
					PoisonMessage.Context = new MessageContext(mailItemDeliver.MbxTransportMailItem.RecordId, mailItemDeliver.MbxTransportMailItem.InternetMessageId, MessageProcessingSource.StoreDriverLocalDelivery);
				}
				StoreDriverDeliveryDiagnostics.LogEvent(MailboxTransportEventLogConstants.Tuple_DeliveryHang, "DeliveryHang", messageArgs);
				return true;
			}
			hangBreadcrumb = null;
			return false;
		}

		public static void UpdateDeliveryThreadCounters()
		{
			DeliveryThrottling.Instance.UpdateMdbThreadCounters();
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "StoreDriverDelivery";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			bool flag = parameters.Argument.IndexOf("exceptions", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = parameters.Argument.IndexOf("callstacks", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag3 = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag4 = flag3 || parameters.Argument.IndexOf("basic", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag5 = parameters.Argument.IndexOf("currentThreads", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag6 = flag4 || parameters.Argument.IndexOf("config", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag7 = !flag6 || parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			XElement xelement = new XElement(((IDiagnosable)this).GetDiagnosticComponentName());
			if (flag7)
			{
				xelement.Add(new XElement("help", "Supported arguments: config, basic, verbose, exceptions, callstacks, currentThreads, help."));
			}
			if (flag4)
			{
				xelement.Add(new XElement("deliveringThreads", StoreDriverDeliveryDiagnostics.deliveringThreads));
				xelement.Add(DeliveryThrottling.Instance.DeliveryServerDiagnostics);
				xelement.Add(DeliveryThrottling.Instance.DeliveryDatabaseDiagnostics);
				xelement.Add(DeliveryThrottling.Instance.DeliveryRecipientDiagnostics);
			}
			if (flag5 && StoreDriverDeliveryDiagnostics.HangDetector != null)
			{
				xelement.Add(StoreDriverDeliveryDiagnostics.HangDetector.GetDiagnosticInfo());
			}
			if (flag3)
			{
				XElement xelement2 = new XElement("HealthHistory");
				foreach (DatabaseHealthBreadcrumb databaseHealthBreadcrumb in ((IEnumerable<DatabaseHealthBreadcrumb>)StoreDriverDeliveryDiagnostics.healthHistory))
				{
					xelement2.Add(databaseHealthBreadcrumb.GetDiagnosticInfo());
				}
				xelement.Add(xelement2);
				xelement.Add(MailItemDeliver.GetDiagnosticInfo());
				string content;
				using (StringWriter stringWriter = new StringWriter(new StringBuilder(1024)))
				{
					MemoryTraceBuilder memoryTraceBuilder = StoreDriverDeliveryDiagnostics.TroubleshootingContext.MemoryTraceBuilder;
					if (memoryTraceBuilder == null)
					{
						stringWriter.Write("No traces were flushed from any thread yet, or in-memory tracing is disabled.");
					}
					else
					{
						memoryTraceBuilder.Dump(stringWriter, true, true);
					}
					content = stringWriter.ToString();
				}
				xelement.Add(new XElement("tracing", content));
			}
			if (flag)
			{
				StoreDriverDeliveryDiagnostics.DumpExceptionStatistics(xelement);
			}
			if (flag2)
			{
				StoreDriverDeliveryDiagnostics.DumpExceptionCallstacks(xelement);
			}
			return xelement;
		}

		internal static void DumpExceptionStatistics(XElement storeDriverElement)
		{
			XElement xelement = new XElement("ExceptionStatisticRecords");
			lock (StoreDriverDeliveryDiagnostics.deliveryExceptionStatisticRecords)
			{
				XElement xelement2 = new XElement("DeliveryExceptionStatistics");
				foreach (KeyValuePair<string, StoreDriverDeliveryDiagnostics.ExceptionStatisticRecord<StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord>> keyValuePair in StoreDriverDeliveryDiagnostics.deliveryExceptionStatisticRecords)
				{
					XElement xelement3 = new XElement("Exception");
					xelement3.Add(new XAttribute("HitCount", keyValuePair.Value.CountSinceServiceStart));
					xelement3.Add(new XAttribute("Type", keyValuePair.Key));
					foreach (StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord deliveryOccurrenceRecord in keyValuePair.Value.LastOccurrences)
					{
						xelement3.Add(deliveryOccurrenceRecord.GetDiagnosticInfo());
					}
					xelement2.Add(xelement3);
				}
				xelement.Add(xelement2);
			}
			storeDriverElement.Add(xelement);
		}

		internal static void DumpExceptionCallstacks(XElement storeDriverElement)
		{
			XElement xelement = new XElement("ExceptionCallstackRecords");
			lock (StoreDriverDeliveryDiagnostics.exceptionDeliveryCallstackRecords)
			{
				XElement xelement2 = new XElement("DeliveryCallstackRecords");
				foreach (KeyValuePair<MessageAction, Dictionary<string, StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord>> keyValuePair in StoreDriverDeliveryDiagnostics.exceptionDeliveryCallstackRecords)
				{
					XElement xelement3 = new XElement(keyValuePair.Key.ToString());
					foreach (KeyValuePair<string, StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord> keyValuePair2 in keyValuePair.Value)
					{
						XElement diagnosticInfo = keyValuePair2.Value.GetDiagnosticInfo();
						XElement xelement4 = new XElement("Callstack", keyValuePair2.Key);
						xelement4.Add(new XAttribute("Length", keyValuePair2.Key.Length));
						diagnosticInfo.Add(xelement4);
						xelement3.Add(diagnosticInfo);
					}
					xelement2.Add(xelement3);
				}
				xelement.Add(xelement2);
			}
			storeDriverElement.Add(xelement);
			storeDriverElement.Add(new XElement("ExceptionCallstackTrappedBySubstring", StoreDriverDeliveryDiagnostics.exceptionCallstackTrappedBySubstring));
		}

		internal static void RecordExceptionForDiagnostics(MessageStatus messageStatus, IMessageConverter messageConverter)
		{
			if (messageStatus.Exception != null && (messageStatus.Action == MessageAction.NDR || messageStatus.Action == MessageAction.Retry || messageStatus.Action == MessageAction.RetryQueue || messageStatus.Action == MessageAction.Reroute || messageStatus.Action == MessageAction.RetryMailboxServer || messageStatus.Action == MessageAction.Skip))
			{
				StoreDriverDeliveryDiagnostics.UpdateDeliveryExceptionStatisticRecords(messageStatus, Components.TransportAppConfig.RemoteDelivery.MaxStoreDriverDeliveryExceptionOccurrenceHistoryPerException, Components.Configuration.AppConfig.RemoteDelivery.MaxStoreDriverDeliveryExceptionCallstackHistoryPerBucket, (MailItemDeliver)messageConverter);
				StoreDriverDeliveryDiagnostics.TrapCallstackWithConfiguredSubstring(messageStatus, Components.TransportAppConfig.RemoteDelivery.StoreDriverExceptionCallstackToTrap);
			}
		}

		internal static void UpdateDeliveryExceptionStatisticRecords(MessageStatus messageStatus, int lastOccurrencesPerException, int callstacksPerBucket, MailItemDeliver mailItemDeliver)
		{
			StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord deliveryOccurrenceRecord = new StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord(DateTime.UtcNow, mailItemDeliver.MbxTransportMailItem.DatabaseName, StoreDriverDelivery.MailboxServerFqdn, mailItemDeliver.MbxTransportMailItem.InternetMessageId, mailItemDeliver.Recipient.Email, mailItemDeliver.RecipientStartTime, mailItemDeliver.SessionId, mailItemDeliver.MbxTransportMailItem.MimeSize, mailItemDeliver.MbxTransportMailItem.MailItemRecipientCount, mailItemDeliver.MbxTransportMailItem.MimeSender, mailItemDeliver.MbxTransportMailItem.RoutingTimeStamp, mailItemDeliver.Stage);
			if (lastOccurrencesPerException > 0)
			{
				string key = StoreDriverDeliveryDiagnostics.GenerateExceptionKey(messageStatus);
				lock (StoreDriverDeliveryDiagnostics.deliveryExceptionStatisticRecords)
				{
					StoreDriverDeliveryDiagnostics.ExceptionStatisticRecord<StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord> value;
					if (!StoreDriverDeliveryDiagnostics.deliveryExceptionStatisticRecords.TryGetValue(key, out value))
					{
						value = default(StoreDriverDeliveryDiagnostics.ExceptionStatisticRecord<StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord>);
						value.LastOccurrences = new Queue<StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord>(lastOccurrencesPerException);
					}
					if (value.LastOccurrences.Count == lastOccurrencesPerException)
					{
						value.LastOccurrences.Dequeue();
					}
					value.LastOccurrences.Enqueue(deliveryOccurrenceRecord);
					value.CountSinceServiceStart++;
					StoreDriverDeliveryDiagnostics.deliveryExceptionStatisticRecords[key] = value;
				}
			}
			StoreDriverDeliveryDiagnostics.UpdateDeliveryExceptionCallstackRecords(messageStatus, callstacksPerBucket, deliveryOccurrenceRecord);
		}

		internal static void TrapCallstackWithConfiguredSubstring(MessageStatus messageStatus, string substringOfCallstackToTrap)
		{
			if (!string.IsNullOrEmpty(substringOfCallstackToTrap))
			{
				string text = messageStatus.Exception.ToString();
				if (0 <= text.IndexOf(substringOfCallstackToTrap))
				{
					StoreDriverDeliveryDiagnostics.exceptionCallstackTrappedBySubstring = text;
				}
			}
		}

		internal static void IncrementDeliveringThreads()
		{
			Interlocked.Increment(ref StoreDriverDeliveryDiagnostics.deliveringThreads);
		}

		internal static void DecrementDeliveringThreads()
		{
			Interlocked.Decrement(ref StoreDriverDeliveryDiagnostics.deliveringThreads);
		}

		private static string GenerateExceptionKey(MessageStatus messageStatus)
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			stringBuilder.Append(messageStatus.Action.ToString());
			stringBuilder.Append(":");
			stringBuilder.Append(messageStatus.Exception.GetType().FullName);
			Exception ex = messageStatus.Exception;
			if (ex is SmtpResponseException)
			{
				stringBuilder.Append(";");
				stringBuilder.Append(messageStatus.Response.StatusCode);
				stringBuilder.Append(";");
				stringBuilder.Append(messageStatus.Response.EnhancedStatusCode);
			}
			while (ex.InnerException != null)
			{
				stringBuilder.Append("~");
				stringBuilder.Append(ex.InnerException.GetType().Name);
				ex = ex.InnerException;
			}
			return stringBuilder.ToString();
		}

		private static void UpdateDeliveryExceptionCallstackRecords(MessageStatus messageStatus, int callstacksPerBucket, StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord occurrenceRecord)
		{
			if (callstacksPerBucket > 0)
			{
				string key = messageStatus.Exception.ToString();
				lock (StoreDriverDeliveryDiagnostics.exceptionDeliveryCallstackRecords)
				{
					Dictionary<string, StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord> dictionary;
					if (!StoreDriverDeliveryDiagnostics.exceptionDeliveryCallstackRecords.TryGetValue(messageStatus.Action, out dictionary))
					{
						dictionary = new Dictionary<string, StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord>(callstacksPerBucket);
						StoreDriverDeliveryDiagnostics.exceptionDeliveryCallstackRecords[messageStatus.Action] = dictionary;
					}
					StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord deliveryOccurrenceRecord;
					if (!dictionary.TryGetValue(key, out deliveryOccurrenceRecord) && dictionary.Count == callstacksPerBucket)
					{
						DateTime t = DateTime.MaxValue;
						string key2 = null;
						foreach (KeyValuePair<string, StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord> keyValuePair in dictionary)
						{
							if (keyValuePair.Value.Timestamp < t)
							{
								t = keyValuePair.Value.Timestamp;
								key2 = keyValuePair.Key;
							}
						}
						dictionary.Remove(key2);
					}
					dictionary[key] = occurrenceRecord;
				}
			}
		}

		internal static void ClearExceptionRecords()
		{
			lock (StoreDriverDeliveryDiagnostics.exceptionDeliveryCallstackRecords)
			{
				StoreDriverDeliveryDiagnostics.exceptionDeliveryCallstackRecords.Clear();
			}
			lock (StoreDriverDeliveryDiagnostics.deliveryExceptionStatisticRecords)
			{
				StoreDriverDeliveryDiagnostics.deliveryExceptionStatisticRecords.Clear();
			}
			StoreDriverDeliveryDiagnostics.exceptionCallstackTrappedBySubstring = null;
		}

		internal const int EstimatedDescriptionStringLength = 400;

		private const string ProcessAccessManagerComponentName = "StoreDriverDelivery";

		protected static readonly Trace Diag = ExTraceGlobals.StoreDriverDeliveryTracer;

		private static DeliveriesInProgress hangDetector;

		private static TroubleshootingContext transportTroubleshootingContext = new TroubleshootingContext("Transport");

		private static ExEventLog eventLogger = new ExEventLog(new Guid("{D81003EF-1A7B-4AF0-BA18-236DB5A83114}"), "MSExchange Store Driver Delivery");

		private static Breadcrumbs<DatabaseHealthBreadcrumb> healthHistory = new Breadcrumbs<DatabaseHealthBreadcrumb>(32);

		private static Dictionary<MessageAction, Dictionary<string, StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord>> exceptionDeliveryCallstackRecords = new Dictionary<MessageAction, Dictionary<string, StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord>>(6);

		private static Dictionary<string, StoreDriverDeliveryDiagnostics.ExceptionStatisticRecord<StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord>> deliveryExceptionStatisticRecords = new Dictionary<string, StoreDriverDeliveryDiagnostics.ExceptionStatisticRecord<StoreDriverDeliveryDiagnostics.DeliveryOccurrenceRecord>>(100);

		private static string exceptionCallstackTrappedBySubstring;

		private static volatile int deliveringThreads;

		private struct ExceptionStatisticRecord<T>
		{
			internal Queue<T> LastOccurrences;

			internal int CountSinceServiceStart;
		}

		private struct DeliveryOccurrenceRecord
		{
			internal DeliveryOccurrenceRecord(DateTime timestamp, string mailboxDatabase, string mailboxServer, string messageID, RoutingAddress recipient, ExDateTime recipientStartTime, ulong sessionID, long messageSize, int recipientCount, RoutingAddress sender, DateTime enqueuedTime, MailItemDeliver.DeliveryStage stage)
			{
				this.timestamp = timestamp;
				this.mailboxDatabase = mailboxDatabase;
				this.mailboxServer = mailboxServer;
				this.messageID = messageID;
				this.recipient = recipient;
				this.recipientStartTime = recipientStartTime;
				this.sessionID = sessionID;
				this.messageSize = messageSize;
				this.recipientCount = recipientCount;
				this.sender = sender;
				this.enqueuedTime = enqueuedTime;
				this.stage = stage;
			}

			internal DateTime Timestamp
			{
				get
				{
					return this.timestamp;
				}
			}

			internal XElement GetDiagnosticInfo()
			{
				return new XElement("Occurrence", new object[]
				{
					new XElement("HitUtc", this.timestamp.ToString(CultureInfo.InvariantCulture)),
					new XElement("MDB", this.mailboxDatabase),
					new XElement("MailboxServer", this.mailboxServer),
					new XElement("MessageID", this.messageID),
					new XElement("Recipient", this.recipient),
					new XElement("RecipientStartTime", this.recipientStartTime.ToString(CultureInfo.InvariantCulture)),
					new XElement("SessionID", this.sessionID),
					new XElement("MessageSize", this.messageSize),
					new XElement("RecipientCount", this.recipientCount),
					new XElement("Sender", this.sender),
					new XElement("EnqueuedTime", this.enqueuedTime.ToString(CultureInfo.InvariantCulture))
				});
			}

			private DateTime timestamp;

			private string mailboxDatabase;

			private string mailboxServer;

			private string messageID;

			private RoutingAddress recipient;

			private ExDateTime recipientStartTime;

			private ulong sessionID;

			private long messageSize;

			private int recipientCount;

			private RoutingAddress sender;

			private DateTime enqueuedTime;

			private MailItemDeliver.DeliveryStage stage;
		}
	}
}
