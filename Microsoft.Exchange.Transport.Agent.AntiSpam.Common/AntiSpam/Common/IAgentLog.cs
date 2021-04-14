using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Transport.Agent.AntiSpam.Common
{
	internal interface IAgentLog
	{
		void LogRejectConnection(string agentName, string eventTopic, ConnectEventArgs eventArgs, SmtpResponse smtpResponse, LogEntry logEntry);

		void LogRejectAuthentication(string agentName, string eventTopic, EndOfAuthenticationEventArgs eventArgs, SmtpResponse smtpResponse, LogEntry logEntry);

		void LogRejectCommand(string agentName, string eventTopic, ReceiveCommandEventArgs eventArgs, SmtpResponse smtpResponse, LogEntry logEntry);

		void LogRejectMessage(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, SmtpResponse smtpResponse, LogEntry logEntry);

		void LogDeleteMessage(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry);

		void LogQuarantineAction(string agentName, string eventTopic, EndOfDataEventArgs eventArgs, AgentAction action, IEnumerable<EnvelopeRecipient> recipients, SmtpResponse smtpResponse, LogEntry logEntry);

		void LogDisconnect(string agentName, string eventTopic, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry);

		void LogRejectRecipients(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, IEnumerable<EnvelopeRecipient> recipients, SmtpResponse smtpResponse, LogEntry logEntry);

		void LogDeleteRecipients(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, IEnumerable<EnvelopeRecipient> recipients, LogEntry logEntry);

		void LogAcceptMessage(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry);

		void LogModifyHeaders(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry);

		void LogStampScl(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry);

		void LogAttributionResult(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry);

		void LogOnPremiseInboundConnectorInfo(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry);

		void LogInvalidCertificate(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, LogEntry logEntry);

		void LogNukeAction(string agentName, string eventTopic, EventArgs eventArgs, SmtpSession smtpSession, MailItem mailItem, IEnumerable<EnvelopeRecipient> recipients, SmtpResponse smtpResponse, LogEntry logEntry);

		void LogAgentAction(string agentName, string eventTopic, EventArgs eventArgs, IEnumerable<RoutingAddress> recipients, AgentAction action, SmtpResponse smtpResponse, LogEntry logEntry, IDictionary<AgentLogField, object> agentLogData, Guid systemProbeId, string internetMessageId);

		IDictionary<AgentLogField, object> GetAgentLogData(SmtpSession smtpSession, MailItem mailItem);
	}
}
