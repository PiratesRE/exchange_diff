using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Data.Transport.Storage;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal class InterceptorBootScannerAgent : StorageAgent
	{
		public InterceptorBootScannerAgent(FilteredRuleCache filteredRuleCache)
		{
			this.filteredRuleCache = filteredRuleCache;
			base.OnLoadedMessage += this.OnLoadedMessageHandler;
		}

		private void OnLoadedMessageHandler(StorageEventSource source, StorageEventArgs args)
		{
			InterceptorAgentRule interceptorAgentRule = InterceptorAgentRuleEvaluator.Evaluate(this.filteredRuleCache.Rules, args);
			if (interceptorAgentRule == null)
			{
				return;
			}
			string context = interceptorAgentRule.GetSourceContext(base.Name, InterceptorAgentEvent.OnLoadedMessage, false);
			ExTraceGlobals.InterceptorAgentTracer.TraceDebug(0, (long)this.GetHashCode(), context);
			interceptorAgentRule.PerformAction(args.MailItem, delegate
			{
				source.Delete(context);
			}, delegate(SmtpResponse response)
			{
				source.DeleteWithNdr(response, context);
			}, null);
		}

		private readonly FilteredRuleCache filteredRuleCache;
	}
}
