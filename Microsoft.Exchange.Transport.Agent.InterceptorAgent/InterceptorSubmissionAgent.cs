using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal sealed class InterceptorSubmissionAgent : SubmissionAgent
	{
		public InterceptorSubmissionAgent(FilteredRuleCache filteredRuleCache)
		{
			this.filteredRuleCache = filteredRuleCache;
			base.OnDemotedMessage += this.DemotedMessageEventHandler;
		}

		private void DemotedMessageEventHandler(StoreDriverEventSource source, StoreDriverSubmissionEventArgs e)
		{
			this.HandleMessage(source, e);
		}

		private void HandleMessage(StoreDriverEventSource source, StoreDriverSubmissionEventArgs e)
		{
			InterceptorAgentRule interceptorAgentRule = InterceptorAgentRuleEvaluator.Evaluate(this.filteredRuleCache.Rules, e);
			if (interceptorAgentRule == null)
			{
				return;
			}
			interceptorAgentRule.PerformAction(e.MailItem, delegate
			{
				throw new SmtpResponseException(new SmtpResponse("250", "2.7.0", new string[]
				{
					"STOREDRV.Submit; message deleted by administrative rule"
				}), "Interceptor Submission Agent");
			}, delegate(SmtpResponse response)
			{
				throw new SmtpResponseException(response);
			}, null);
		}

		private readonly FilteredRuleCache filteredRuleCache;
	}
}
