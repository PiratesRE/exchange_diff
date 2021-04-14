using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal class JournalLogContext : IDisposable
	{
		public JournalLogContext(string source, string eventTopic, string messageId, MailItem mailItem)
		{
			this.source = source;
			this.eventTopic = eventTopic;
			this.Status = "Processing";
			this.messageId = (string.IsNullOrEmpty(messageId) ? string.Empty : messageId);
			this.internalMesageId = 0L;
			this.externalOrganizationId = Guid.Empty;
			this.logData = new List<KeyValuePair<string, object>>();
			try
			{
				if (mailItem != null)
				{
					TransportMailItem transportMailItem = ((ITransportMailItemWrapperFacade)mailItem).TransportMailItem as TransportMailItem;
					if (transportMailItem != null && !transportMailItem.IsRowDeleted)
					{
						this.internalMesageId = mailItem.InternalMessageId;
						this.externalOrganizationId = transportMailItem.ExternalOrganizationId;
					}
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.JournalingTracer.TraceError((long)this.GetHashCode(), "JournalLogContext failed with exception: " + ex);
				JournalLogContext.JournalLog.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_JournalingLogException, null, new object[]
				{
					"JournalLogContext",
					ex.ToString()
				});
			}
		}

		private string Status { get; set; }

		public void AddParameter(string key, params object[] objects)
		{
			if (!string.IsNullOrEmpty(key))
			{
				string text = string.Empty;
				try
				{
					if (objects != null && objects.Any<object>())
					{
						List<object> list = new List<object>();
						foreach (object obj in objects)
						{
							if (obj != null)
							{
								IEnumerable<object> enumerable = obj as IEnumerable<object>;
								if (!(obj is string) && enumerable != null)
								{
									list.AddRange(enumerable);
								}
								else
								{
									list.Add(obj);
								}
							}
						}
						text = string.Join<object>("|", list);
					}
					this.logData.Add(new KeyValuePair<string, object>(key, (text.Length > 256) ? text.Substring(0, 256) : text));
				}
				catch (Exception ex)
				{
					ExTraceGlobals.JournalingTracer.TraceError((long)this.GetHashCode(), "JournalLog AddParameter failed with exception: " + ex);
					JournalLogContext.JournalLog.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_JournalingLogException, null, new object[]
					{
						"AddParameter",
						ex.ToString()
					});
				}
			}
		}

		public void LogAsSkipped(string key, params object[] objects)
		{
			this.Status = "Skipped";
			this.AddParameter(key, objects);
		}

		public void LogAsRetriableError(params object[] objects)
		{
			this.Status = "RetriableError";
			this.AddParameter("RetriableError", objects);
		}

		public void LogAsFatalError(params object[] objects)
		{
			this.Status = "FatalError";
			this.AddParameter("FatalError", objects);
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				if (string.IsNullOrEmpty(this.Status) || (!this.Status.Equals("Skipped", StringComparison.CurrentCultureIgnoreCase) && !this.Status.Equals("FatalError", StringComparison.CurrentCultureIgnoreCase) && !this.Status.Equals("RetriableError", StringComparison.CurrentCultureIgnoreCase)))
				{
					this.Status = "Processed";
				}
				JournalLogContext.JournalLog.Instance.LogEvent(this.source, this.externalOrganizationId, this.eventTopic, this.Status, this.internalMesageId, this.messageId, this.logData);
			}
		}

		private const int MaxCustomStringLength = 256;

		private readonly string source;

		private readonly string eventTopic;

		private readonly string messageId;

		private readonly long internalMesageId;

		private readonly Guid externalOrganizationId;

		private List<KeyValuePair<string, object>> logData;

		private bool isDisposed;

		internal class Source
		{
			public const string JournalAgent = "JA";

			public const string JournalFilterAgent = "JF";

			public const string UnwrapJournalAgent = "UJA";
		}

		internal class StatusCode
		{
			public const string Processing = "Processing";

			public const string Processed = "Processed";

			public const string Skipped = "Skipped";

			public const string RetriableError = "RetriableError";

			public const string FatalError = "FatalError";
		}

		internal class Tag
		{
			internal const string Configure = "Cfg";

			internal const string RuleId = "RId";

			internal const string JournalReportItem = "JRItem";

			internal const string JournalRecipients = "JRRec";

			internal const string MessageCheck = "MC";

			internal const string ProcessingStatus = "PSt";

			internal const string GccRule = "GR";

			internal const string AlreadyProcessed = "APed";

			internal const string UnwrapJournalTargetRecipients = "UnRec";
		}

		private class JournalLog
		{
			public static JournalLogContext.JournalLog Instance
			{
				get
				{
					if (JournalLogContext.JournalLog.instance == null)
					{
						lock (JournalLogContext.JournalLog.initLock)
						{
							if (JournalLogContext.JournalLog.instance == null)
							{
								JournalLogContext.JournalLog journalLog = new JournalLogContext.JournalLog();
								journalLog.Configure();
								JournalLogContext.JournalLog.instance = journalLog;
							}
						}
					}
					return JournalLogContext.JournalLog.instance;
				}
			}

			public static ExEventLog Logger
			{
				get
				{
					return JournalLogContext.JournalLog.logger;
				}
			}

			public void LogEvent(string source, Guid externalOrganizationId, string eventTopic, string status, long internalMessageId, string messageId, IEnumerable<KeyValuePair<string, object>> customData)
			{
				if (!this.initialized)
				{
					return;
				}
				try
				{
					LogRowFormatter logRowFormatter = new LogRowFormatter(JournalLogContext.JournalLog.journalLogSchema);
					logRowFormatter[1] = source;
					logRowFormatter[2] = ((externalOrganizationId == Guid.Empty) ? null : externalOrganizationId);
					logRowFormatter[5] = eventTopic;
					logRowFormatter[6] = status;
					logRowFormatter[3] = internalMessageId;
					logRowFormatter[4] = messageId;
					logRowFormatter[7] = customData;
					this.log.Append(logRowFormatter, 0);
				}
				catch (Exception ex)
				{
					ExTraceGlobals.JournalingTracer.TraceError((long)this.GetHashCode(), "JournalLog LogEvent with exception: " + ex);
					JournalLogContext.JournalLog.logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_JournalingLogException, null, new object[]
					{
						"LogEvent",
						ex.ToString()
					});
				}
			}

			private void Configure()
			{
				Server transportServer = Components.Configuration.LocalServer.TransportServer;
				if (transportServer.JournalLogEnabled)
				{
					if (transportServer.JournalLogPath != null && !string.IsNullOrEmpty(transportServer.JournalLogPath.PathName))
					{
						this.log = new AsyncLog("JournalLog", new LogHeaderFormatter(JournalLogContext.JournalLog.journalLogSchema), "JournalLog");
						this.log.Configure(transportServer.JournalLogPath.PathName, transportServer.JournalLogMaxAge, (long)(transportServer.JournalLogMaxDirectorySize.IsUnlimited ? 0UL : transportServer.JournalLogMaxDirectorySize.Value.ToBytes()), (long)(transportServer.JournalLogMaxFileSize.IsUnlimited ? 0UL : transportServer.JournalLogMaxDirectorySize.Value.ToBytes()), 65536, TimeSpan.FromSeconds(60.0), TimeSpan.FromSeconds(15.0));
						this.initialized = true;
						return;
					}
					ExTraceGlobals.JournalingTracer.TraceError((long)this.GetHashCode(), "JournalLog configure failed with empty logpath");
					JournalLogContext.JournalLog.logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_JournalingLogConfigureError, null, new object[0]);
				}
			}

			internal const string LogName = "JournalLog";

			internal const string LogType = "Journal Log";

			internal const string LogComponent = "JournalLog";

			private static LogSchema journalLogSchema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Journal Log", Enum.GetNames(typeof(JournalLogContext.JournalLog.JournalLogFields)));

			private static ExEventLog logger = new ExEventLog(new Guid("7D2A0005-2C75-42ac-B495-8FE62F3B4FCF"), "MSExchange Messaging Policies");

			private static object initLock = new object();

			private static JournalLogContext.JournalLog instance;

			private bool initialized;

			private AsyncLog log;

			private enum JournalLogFields
			{
				TimeStamp,
				Source,
				ExternalOrganizationId,
				InternalMessageId,
				MessageId,
				Event,
				Status,
				CustomData
			}
		}
	}
}
