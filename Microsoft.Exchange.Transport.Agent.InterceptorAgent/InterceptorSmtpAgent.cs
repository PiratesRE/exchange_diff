using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal sealed class InterceptorSmtpAgent : SmtpReceiveAgent
	{
		public InterceptorSmtpAgent(FilteredRuleCache filteredRuleCache)
		{
			this.filteredRuleCache = filteredRuleCache;
			base.OnEndOfData += this.EodHandler;
			base.OnEndOfHeaders += this.EohHandler;
			base.OnRcptCommand += this.RcptHandler;
			base.OnMailCommand += this.MailHandler;
		}

		private void MailHandler(ReceiveCommandEventSource source, MailCommandEventArgs mailArgs)
		{
			InterceptorAgentRule rule = InterceptorAgentRuleEvaluator.Evaluate(this.filteredRuleCache.Rules, mailArgs);
			this.Run(rule, null, new InterceptorSmtpAgent.RejectCommand(source.RejectCommand), null, source.SmtpSession, mailArgs, InterceptorAgentEvent.OnMailFrom, true);
		}

		private void RcptHandler(ReceiveCommandEventSource source, RcptCommandEventArgs rcptArgs)
		{
			InterceptorAgentRule rule = InterceptorAgentRuleEvaluator.Evaluate(this.filteredRuleCache.Rules, rcptArgs);
			this.Run(rule, null, new InterceptorSmtpAgent.RejectCommand(source.RejectCommand), null, source.SmtpSession, rcptArgs, InterceptorAgentEvent.OnRcptTo, true);
		}

		private void EohHandler(ReceiveMessageEventSource source, EndOfHeadersEventArgs eohArgs)
		{
			InterceptorAgentRule rule = InterceptorAgentRuleEvaluator.Evaluate(this.filteredRuleCache.Rules, eohArgs);
			this.Run(rule, eohArgs.MailItem, new InterceptorSmtpAgent.RejectCommand(source.RejectMessage), new InterceptorSmtpAgent.DiscardMessage(source.DiscardMessage), source.SmtpSession, eohArgs, InterceptorAgentEvent.OnEndOfHeaders, false);
		}

		private void EodHandler(ReceiveMessageEventSource source, EndOfDataEventArgs eodArgs)
		{
			InterceptorAgentRule rule = InterceptorAgentRuleEvaluator.Evaluate(this.filteredRuleCache.Rules, eodArgs);
			this.Run(rule, eodArgs.MailItem, new InterceptorSmtpAgent.RejectCommand(source.RejectMessage), new InterceptorSmtpAgent.DiscardMessage(source.DiscardMessage), source.SmtpSession, eodArgs, InterceptorAgentEvent.OnEndOfData, false);
		}

		private void Run(InterceptorAgentRule rule, MailItem mail, InterceptorSmtpAgent.RejectCommand rejectCmd, InterceptorSmtpAgent.DiscardMessage discardMessage, SmtpSession session, EventArgs args, InterceptorAgentEvent evt, bool logCommand)
		{
			if (rule == null)
			{
				return;
			}
			string sourceContext = rule.GetSourceContext(base.Name, evt, false);
			rule.PerformAction(mail, delegate
			{
				discardMessage(SmtpResponse.SuccessfulConnection, sourceContext);
			}, delegate(SmtpResponse response)
			{
				if (logCommand)
				{
					AgentLog.Instance.LogRejectCommand(this.Name, this.EventTopic, (ReceiveCommandEventArgs)args, response, new LogEntry(string.Empty, sourceContext));
				}
				else
				{
					AgentLog.Instance.LogRejectMessage(this.Name, this.EventTopic, args, session, mail, response, new LogEntry(string.Empty, sourceContext));
				}
				rejectCmd(response, sourceContext);
			}, null);
		}

		private readonly FilteredRuleCache filteredRuleCache;

		private delegate void RejectCommand(SmtpResponse resp, string context);

		private delegate void DiscardMessage(SmtpResponse response, string context);
	}
}
