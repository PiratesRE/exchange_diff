using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.LoggingCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport.Agent.AntiSpam.Common
{
	internal class AgentLog : IAgentLog
	{
		public AgentLog()
		{
			string[] array = new string[20];
			for (int i = 0; i < 20; i++)
			{
				array[i] = ((AgentLogField)i).ToString();
			}
			this.agentLogSchema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Agent Log", array);
			this.log = new Log("AgentLog", new LogHeaderFormatter(this.agentLogSchema), "AgentLogs");
			this.Configure(Components.Configuration.LocalServer);
		}

		public static IAgentLog Instance
		{
			get
			{
				if (AgentLog.instance == null)
				{
					lock (AgentLog.agentLogCreationLock)
					{
						if (AgentLog.instance == null)
						{
							AgentLog agentLog = new AgentLog();
							Thread.MemoryBarrier();
							AgentLog.instance = agentLog;
						}
					}
				}
				return AgentLog.instance;
			}
		}

		private static ProcessTransportRole GetProcessTransportRole()
		{
			ITransportConfiguration transportConfiguration;
			ProcessTransportRole result;
			if (Components.TryGetConfigurationComponent(out transportConfiguration))
			{
				result = transportConfiguration.ProcessTransportRole;
			}
			else
			{
				result = ProcessTransportRole.Hub;
			}
			return result;
		}

		public void LogRejectConnection(string agentName, string eventTopic, ConnectEventArgs eventArgs, SmtpResponse smtpResponse, LogEntry logEntry)
		{
			this.LogAgentAction(agentName, eventTopic, eventArgs, eventArgs.SmtpSession, null, null, AgentAction.RejectConnection, smtpResponse, logEntry);
		}

		public void LogRejectAuthentication(string agentName, string eventTopic, EndOfAuthenticationEventArgs eventArgs, SmtpResponse smtpResponse, LogEntry logEntry)
		{
			this.LogAgentAction(agentName, eventTopic, eventArgs, eventArgs.SmtpSession, null, null, AgentAction.RejectAuthentication, smtpResponse, logEntry);
		}

		public void LogRejectCommand(string agentName, string eventTopic, ReceiveCommandEventArgs eventArgs, SmtpResponse smtpResponse, LogEntry logEntry)
		{
			MailItem mailItem = null;
			if (eventArgs != null)
			{
				DataCommandEventArgs dataCommandEventArgs = eventArgs as DataCommandEventArgs;
				if (dataCommandEventArgs != null)
				{
					mailItem = dataCommandEventArgs.MailItem;
				}
				else
				{
					RcptCommandEventArgs rcptCommandEventArgs = eventArgs as RcptCommandEventArgs;
					if (rcptCommandEventArgs != null)
					{
						mailItem = rcptCommandEventArgs.MailItem;
					}
				}
			}
			this.LogAgentAction(agentName, eventTopic, eventArgs, eventArgs.SmtpSession, mailItem, null, AgentAction.RejectCommand, smtpResponse, logEntry);
		}

		public void LogRejectMessage(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, SmtpResponse smtpResponse, LogEntry logEntry)
		{
			this.LogAgentAction(agentName, eventTopic, eventArgs, smtpSession, mailItem, (mailItem != null) ? mailItem.Recipients : null, AgentAction.RejectMessage, smtpResponse, logEntry);
		}

		public void LogDeleteMessage(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry)
		{
			this.LogAgentAction(agentName, eventTopic, eventArgs, smtpSession, mailItem, (mailItem != null) ? mailItem.Recipients : null, AgentAction.DeleteMessage, SmtpResponse.Empty, logEntry);
		}

		public void LogQuarantineAction(string agentName, string eventTopic, EndOfDataEventArgs eventArgs, AgentAction action, IEnumerable<EnvelopeRecipient> recipients, SmtpResponse smtpResponse, LogEntry logEntry)
		{
			this.LogAgentAction(agentName, eventTopic, eventArgs, eventArgs.SmtpSession, eventArgs.MailItem, recipients, action, smtpResponse, logEntry);
		}

		public void LogDisconnect(string agentName, string eventTopic, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry)
		{
			this.LogAgentAction(agentName, eventTopic, null, smtpSession, mailItem, (mailItem != null) ? mailItem.Recipients : null, AgentAction.Disconnect, SmtpResponse.Empty, logEntry);
		}

		public void LogRejectRecipients(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, IEnumerable<EnvelopeRecipient> recipients, SmtpResponse smtpResponse, LogEntry logEntry)
		{
			this.LogAgentAction(agentName, eventTopic, eventArgs, smtpSession, mailItem, recipients, AgentAction.RejectRecipients, smtpResponse, logEntry);
		}

		public void LogDeleteRecipients(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, IEnumerable<EnvelopeRecipient> recipients, LogEntry logEntry)
		{
			this.LogAgentAction(agentName, eventTopic, eventArgs, smtpSession, mailItem, recipients, AgentAction.DeleteRecipients, SmtpResponse.Empty, logEntry);
		}

		public void LogAcceptMessage(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry)
		{
			this.LogAgentAction(agentName, eventTopic, eventArgs, smtpSession, mailItem, (mailItem != null) ? mailItem.Recipients : null, AgentAction.AcceptMessage, SmtpResponse.Empty, logEntry);
		}

		public void LogModifyHeaders(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry)
		{
			this.LogAgentAction(agentName, eventTopic, eventArgs, smtpSession, mailItem, (mailItem != null) ? mailItem.Recipients : null, AgentAction.ModifyHeaders, SmtpResponse.Empty, logEntry);
		}

		public void LogStampScl(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry)
		{
			this.LogAgentAction(agentName, eventTopic, eventArgs, smtpSession, mailItem, (mailItem != null) ? mailItem.Recipients : null, AgentAction.StampScl, SmtpResponse.Empty, logEntry);
		}

		public void LogAttributionResult(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry)
		{
			this.LogAgentAction(agentName, eventTopic, eventArgs, smtpSession, mailItem, (mailItem != null) ? mailItem.Recipients : null, AgentAction.AttributionResult, SmtpResponse.Empty, logEntry);
		}

		public void LogOnPremiseInboundConnectorInfo(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry)
		{
			this.LogAgentAction(agentName, eventTopic, eventArgs, smtpSession, mailItem, (mailItem != null) ? mailItem.Recipients : null, AgentAction.OnPremiseInboundConnectorInfo, SmtpResponse.Empty, logEntry);
		}

		public void LogInvalidCertificate(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry)
		{
			this.LogAgentAction(agentName, eventTopic, eventArgs, smtpSession, mailItem, (mailItem != null) ? mailItem.Recipients : null, AgentAction.InvalidCertificate, SmtpResponse.Empty, logEntry);
		}

		public void LogNukeAction(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, IEnumerable<EnvelopeRecipient> recipients, SmtpResponse smtpResponse, LogEntry logEntry)
		{
			this.LogAgentAction(agentName, eventTopic, eventArgs, smtpSession, mailItem, recipients, AgentAction.AutoNukeRecipient, smtpResponse, logEntry);
		}

		internal void LogAgentAction(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, IEnumerable<EnvelopeRecipient> recipients, AgentAction action, SmtpResponse smtpResponse, LogEntry logEntry)
		{
			IDictionary<AgentLogField, object> agentLogData = this.GetAgentLogData(smtpSession, mailItem);
			List<RoutingAddress> recipients2 = null;
			if (recipients != null)
			{
				recipients2 = new List<RoutingAddress>(from recipient in recipients
				select recipient.Address);
			}
			if (mailItem == null)
			{
				this.LogAgentAction(agentName, eventTopic, eventArgs, recipients2, action, smtpResponse, logEntry, agentLogData);
				return;
			}
			this.LogAgentAction(agentName, eventTopic, eventArgs, recipients2, action, smtpResponse, logEntry, agentLogData, mailItem.SystemProbeId, mailItem.InternetMessageId);
		}

		public IDictionary<AgentLogField, object> GetAgentLogData(SmtpSession smtpSession, MailItem mailItem)
		{
			Dictionary<AgentLogField, object> dictionary = new Dictionary<AgentLogField, object>();
			if (!this.enabled)
			{
				return dictionary;
			}
			if (smtpSession == null)
			{
				throw new ArgumentNullException("smtpSession");
			}
			dictionary[AgentLogField.SessionId] = smtpSession.SessionId.ToString("X16", CultureInfo.InvariantCulture);
			dictionary[AgentLogField.LocalEndpoint] = smtpSession.LocalEndPoint;
			dictionary[AgentLogField.RemoteEndpoint] = smtpSession.RemoteEndPoint;
			if (smtpSession.IsExternalConnection)
			{
				dictionary[AgentLogField.EnteredOrgFromIP] = smtpSession.RemoteEndPoint.Address;
			}
			else
			{
				dictionary[AgentLogField.EnteredOrgFromIP] = smtpSession.LastExternalIPAddress;
			}
			if (mailItem != null)
			{
				if (mailItem.Message != null)
				{
					dictionary[AgentLogField.MessageId] = mailItem.Message.MessageId;
					dictionary[AgentLogField.P2FromAddresses] = AgentLog.ReplaceNewLinesWithSpace(AgentLog.GetP2From(mailItem));
				}
				dictionary[AgentLogField.P1FromAddress] = AgentLog.ReplaceNewLinesWithSpace(mailItem.FromAddress.ToString());
				if (mailItem.NetworkMessageId != Guid.Empty)
				{
					dictionary[AgentLogField.NetworkMsgID] = mailItem.NetworkMessageId;
				}
				if (mailItem.TenantId != Guid.Empty)
				{
					dictionary[AgentLogField.TenantID] = mailItem.TenantId;
				}
			}
			dictionary[AgentLogField.Directionality] = this.GetDirectionalityString(mailItem);
			return dictionary;
		}

		internal void LogAgentAction(string agentName, string eventTopic, EventArgs eventArgs, IEnumerable<RoutingAddress> recipients, AgentAction action, SmtpResponse smtpResponse, LogEntry logEntry, IDictionary<AgentLogField, object> agentLogData)
		{
			this.LogAgentAction(agentName, eventTopic, eventArgs, recipients, action, smtpResponse, logEntry, agentLogData, Guid.Empty, null);
		}

		public void LogAgentAction(string agentName, string eventTopic, EventArgs eventArgs, IEnumerable<RoutingAddress> recipients, AgentAction action, SmtpResponse smtpResponse, LogEntry logEntry, IDictionary<AgentLogField, object> agentLogData, Guid systemProbeId, string internetMessageId)
		{
			if (!this.enabled)
			{
				return;
			}
			if (agentLogData == null || agentLogData.Count == 0)
			{
				throw new ArgumentNullException("agentLogData");
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.agentLogSchema);
			foreach (KeyValuePair<AgentLogField, object> keyValuePair in agentLogData)
			{
				if (keyValuePair.Value != null)
				{
					logRowFormatter[(int)keyValuePair.Key] = keyValuePair.Value;
				}
			}
			logRowFormatter[11] = eventTopic;
			logRowFormatter[10] = agentName;
			logRowFormatter[12] = action;
			AgentLog.AddRecipientsData(logRowFormatter, recipients);
			AgentLog.AddEventArgsData(logRowFormatter, eventArgs);
			AgentLog.AddSmtpResponseData(logRowFormatter, smtpResponse);
			AgentLog.AddLogEntryData(logRowFormatter, logEntry);
			this.log.Append(logRowFormatter, 0);
			AgentLog.PublishSystemProbeNotification(agentName, action, systemProbeId, internetMessageId);
		}

		private static void AddRecipientsData(LogRowFormatter row, IEnumerable<RoutingAddress> recipients)
		{
			if (recipients == null)
			{
				recipients = new List<RoutingAddress>();
			}
			string[] array = (from recipient in recipients
			select AgentLog.ReplaceNewLinesWithSpace(recipient.ToString())).ToArray<string>();
			row[9] = array.Length;
			row[8] = array;
		}

		private static void AddLogEntryData(LogRowFormatter row, LogEntry logEntry)
		{
			if (logEntry != null)
			{
				if (!string.IsNullOrEmpty(logEntry.Reason))
				{
					string text = AgentLog.ReplaceNewLinesWithSpace(logEntry.Reason);
					row[14] = ((text.Length <= 256) ? text : text.Substring(0, 256));
				}
				if (!string.IsNullOrEmpty(logEntry.ReasonData))
				{
					string text2 = AgentLog.ReplaceNewLinesWithSpace(logEntry.ReasonData);
					row[15] = ((text2.Length <= 256) ? text2 : text2.Substring(0, 256));
				}
				if (!string.IsNullOrEmpty(logEntry.Diagnostics))
				{
					string text3 = AgentLog.ReplaceNewLinesWithSpace(logEntry.Diagnostics);
					row[16] = ((text3.Length <= 256) ? text3 : text3.Substring(0, 256));
				}
			}
		}

		private static void AddSmtpResponseData(LogRowFormatter row, SmtpResponse smtpResponse)
		{
			if (!SmtpResponse.Empty.Equals(smtpResponse))
			{
				string text = AgentLog.ReplaceNewLinesWithSpace(smtpResponse.ToString());
				row[13] = ((text.Length <= 256) ? text : text.Substring(0, 256));
			}
		}

		private static void AddEventArgsData(LogRowFormatter row, EventArgs eventArgs)
		{
			if (eventArgs != null)
			{
				if (eventArgs is MailCommandEventArgs)
				{
					MailCommandEventArgs mailCommandEventArgs = eventArgs as MailCommandEventArgs;
					row[6] = AgentLog.ReplaceNewLinesWithSpace(mailCommandEventArgs.FromAddress.ToString());
					return;
				}
				if (eventArgs is RcptCommandEventArgs)
				{
					RcptCommandEventArgs rcptCommandEventArgs = eventArgs as RcptCommandEventArgs;
					row[9] = 1;
					row[8] = AgentLog.ReplaceNewLinesWithSpace(rcptCommandEventArgs.RecipientAddress.ToString());
					return;
				}
				if (eventArgs is EndOfHeadersEventArgs)
				{
					EndOfHeadersEventArgs endOfHeadersEventArgs = eventArgs as EndOfHeadersEventArgs;
					row[7] = AgentLog.ReplaceNewLinesWithSpace(AgentLog.GetP2From(endOfHeadersEventArgs.Headers));
				}
			}
		}

		private static void PublishSystemProbeNotification(string agentName, AgentAction action, Guid systemProbeId, string internetMessageId)
		{
			if (systemProbeId != Guid.Empty && !string.IsNullOrEmpty(internetMessageId))
			{
				EventNotificationItem eventNotificationItem = new EventNotificationItem(ExchangeComponent.Transport.Name, "MessageTracking", systemProbeId.ToString(), ResultSeverityLevel.Verbose);
				string value = string.Format("Agent={0};Action={1}", agentName, action);
				eventNotificationItem.AddCustomProperty("StateAttribute1", internetMessageId);
				eventNotificationItem.AddCustomProperty("StateAttribute2", "AGENTINFO");
				eventNotificationItem.AddCustomProperty("StateAttribute3", value);
				eventNotificationItem.Publish(false);
			}
		}

		private static string GetP2From(MailItem mailItem)
		{
			if (mailItem.Message.MimeDocument.RootPart == null || mailItem.Message.MimeDocument.RootPart.Headers == null)
			{
				return string.Empty;
			}
			return AgentLog.GetP2From(mailItem.Message.MimeDocument.RootPart.Headers);
		}

		private static string GetP2From(HeaderList headers)
		{
			Header[] array = headers.FindAll(HeaderId.From);
			List<string> list = new List<string>(array.Length);
			foreach (Header header in array)
			{
				foreach (AddressItem addressItem in ((AddressHeader)header))
				{
					MimeRecipient mimeRecipient = addressItem as MimeRecipient;
					if (mimeRecipient != null)
					{
						string email = mimeRecipient.Email;
						if (!string.IsNullOrEmpty(email))
						{
							list.Add(email);
						}
					}
					else
					{
						MimeGroup mimeGroup = addressItem as MimeGroup;
						if (mimeGroup != null)
						{
							foreach (MimeRecipient mimeRecipient2 in mimeGroup)
							{
								string email = mimeRecipient2.Email;
								if (!string.IsNullOrEmpty(email))
								{
									list.Add(email);
								}
							}
						}
					}
				}
				if (list.Count >= 10)
				{
					break;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			while (num < list.Count && num < 10)
			{
				stringBuilder.Append(list[num]);
				stringBuilder.Append(';');
				num++;
			}
			return stringBuilder.ToString();
		}

		private static string ReplaceNewLinesWithSpace(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			StringBuilder stringBuilder = new StringBuilder(s.Length);
			bool flag = false;
			bool flag2 = false;
			foreach (char c in s)
			{
				char c2 = c;
				if (c2 == '\n' || c2 == '\r')
				{
					if (!flag)
					{
						stringBuilder.Append(' ');
					}
					flag = true;
					flag2 = true;
				}
				else
				{
					stringBuilder.Append(c);
					flag = false;
				}
			}
			if (!flag2)
			{
				return s;
			}
			return stringBuilder.ToString();
		}

		private void Configure(TransportServerConfiguration server)
		{
			Server transportServer = server.TransportServer;
			ProcessTransportRole processTransportRole = AgentLog.GetProcessTransportRole();
			if (processTransportRole == ProcessTransportRole.MailboxDelivery)
			{
				this.Configure(transportServer.MailboxDeliveryAgentLogEnabled, transportServer.MailboxDeliveryAgentLogPath, transportServer.MailboxDeliveryAgentLogMaxAge, transportServer.MailboxDeliveryAgentLogMaxDirectorySize, transportServer.MailboxDeliveryAgentLogMaxFileSize);
				return;
			}
			if (processTransportRole == ProcessTransportRole.MailboxSubmission)
			{
				this.Configure(transportServer.MailboxSubmissionAgentLogEnabled, transportServer.MailboxSubmissionAgentLogPath, transportServer.MailboxSubmissionAgentLogMaxAge, transportServer.MailboxSubmissionAgentLogMaxDirectorySize, transportServer.MailboxSubmissionAgentLogMaxFileSize);
				return;
			}
			this.Configure(transportServer.AgentLogEnabled, transportServer.AgentLogPath, transportServer.AgentLogMaxAge, transportServer.AgentLogMaxDirectorySize, transportServer.AgentLogMaxFileSize);
		}

		private void Configure(bool enabled, LocalLongFullPath path, EnhancedTimeSpan maxAge, Unlimited<ByteQuantifiedSize> maxDirectorySize, Unlimited<ByteQuantifiedSize> maxFileSize)
		{
			if (!enabled)
			{
				this.enabled = false;
				return;
			}
			if (path == null || string.IsNullOrEmpty(path.PathName))
			{
				this.enabled = false;
				ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Agent Log path was set to null, Agent Log is disabled");
				Components.EventLogger.LogEvent(AgentsEventLogConstants.Tuple_AgentLogPathIsNull, null, new object[0]);
				return;
			}
			this.log.Configure(path.PathName, maxAge, (long)(maxDirectorySize.IsUnlimited ? 0UL : maxDirectorySize.Value.ToBytes()), (long)(maxFileSize.IsUnlimited ? 0UL : maxFileSize.Value.ToBytes()));
			this.enabled = true;
		}

		private string GetDirectionalityString(MailItem item)
		{
			TransportMailItemWrapper transportMailItemWrapper = item as TransportMailItemWrapper;
			if (transportMailItemWrapper != null && transportMailItemWrapper.TransportMailItem != null)
			{
				return transportMailItemWrapper.TransportMailItem.Directionality.ToString();
			}
			return MailDirectionality.Undefined.ToString();
		}

		private const int MaxFromHeaders = 10;

		private const int MaxCustomStringLength = 256;

		private const string LogComponentName = "AgentLogs";

		private static readonly object agentLogCreationLock = new object();

		private static readonly TimeSpan DefaultMaxAge = TimeSpan.FromDays(30.0);

		private static IAgentLog instance;

		private Log log;

		private LogSchema agentLogSchema;

		private bool enabled;
	}
}
