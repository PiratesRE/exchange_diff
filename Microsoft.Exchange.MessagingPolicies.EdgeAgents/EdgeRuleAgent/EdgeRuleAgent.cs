using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;

namespace Microsoft.Exchange.MessagingPolicies.EdgeRuleAgent
{
	internal class EdgeRuleAgent : SmtpReceiveAgent
	{
		public EdgeRuleAgent(SmtpServer server, TransportRuleCollection rules, bool shouldDefer)
		{
			base.OnEndOfData += this.EndOfDataHandler;
			this.server = server;
			this.rules = rules;
			this.shouldDefer = shouldDefer;
		}

		public void EndOfDataHandler(ReceiveMessageEventSource source, EndOfDataEventArgs args)
		{
			if (this.shouldDefer)
			{
				AgentLog.Instance.LogRejectMessage(base.Name, base.EventTopic, args, args.SmtpSession, args.MailItem, SmtpResponse.DataTransactionFailed, EdgeRuleAgent.TransientError);
				source.RejectMessage(SmtpResponse.DataTransactionFailed);
				return;
			}
			if (args.MailItem.Recipients.Count == 0)
			{
				return;
			}
			try
			{
				if (this.rules != null)
				{
					ExecutionStatus executionStatus = this.rules.Run(this.server, args.MailItem, source, args.SmtpSession, null);
					if (executionStatus == ExecutionStatus.TransientError)
					{
						AgentLog.Instance.LogRejectMessage(base.Name, base.EventTopic, args, args.SmtpSession, args.MailItem, SmtpResponse.DataTransactionFailed, EdgeRuleAgent.TransientError);
						source.RejectMessage(SmtpResponse.DataTransactionFailed);
					}
				}
			}
			catch (ExchangeDataException ex)
			{
				AgentLog.Instance.LogRejectMessage(base.Name, base.EventTopic, args, args.SmtpSession, args.MailItem, SmtpResponse.InvalidContent, new LogEntry("InvalidContent", ex.Message));
				source.RejectMessage(SmtpResponse.InvalidContent);
			}
		}

		private static readonly LogEntry TransientError = new LogEntry("TransientError", string.Empty);

		private TransportRuleCollection rules;

		private SmtpServer server;

		private bool shouldDefer;
	}
}
