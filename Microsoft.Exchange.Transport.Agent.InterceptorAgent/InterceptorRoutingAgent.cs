using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal sealed class InterceptorRoutingAgent : RoutingAgent
	{
		public InterceptorRoutingAgent(FilteredRuleCache filteredRuleCache)
		{
			this.filteredRuleCache = filteredRuleCache;
			base.OnSubmittedMessage += this.OnSubmittedHandler;
			base.OnResolvedMessage += this.OnResolvedHandler;
			base.OnRoutedMessage += this.OnRoutedHandler;
			base.OnCategorizedMessage += this.OnCategorizedHandler;
		}

		private void OnSubmittedHandler(SubmittedMessageEventSource source, QueuedMessageEventArgs args)
		{
			this.HandleMessage(source, args, InterceptorAgentEvent.OnSubmittedMessage);
		}

		private void OnResolvedHandler(ResolvedMessageEventSource source, QueuedMessageEventArgs args)
		{
			this.HandleMessage(source, args, InterceptorAgentEvent.OnResolvedMessage);
		}

		private void OnRoutedHandler(RoutedMessageEventSource source, QueuedMessageEventArgs args)
		{
			this.HandleMessage(source, args, InterceptorAgentEvent.OnRoutedMessage);
		}

		private void OnCategorizedHandler(CategorizedMessageEventSource source, QueuedMessageEventArgs args)
		{
			this.HandleMessage(source, args, InterceptorAgentEvent.OnCategorizedMessage);
		}

		private void HandleMessage(QueuedMessageEventSource source, QueuedMessageEventArgs args, InterceptorAgentEvent evt)
		{
			InterceptorAgentRule rule = InterceptorAgentRuleEvaluator.Evaluate(this.filteredRuleCache.Rules, args, evt);
			if (rule == null)
			{
				return;
			}
			rule.PerformAction(args.MailItem, delegate
			{
				this.DeleteMessage(args.MailItem, source, rule, evt);
			}, delegate(SmtpResponse response)
			{
				this.NdrMessage(args.MailItem, response, source, rule, evt);
			}, delegate(TimeSpan deferTime)
			{
				this.DeferMessage(args.MailItem, deferTime, source, rule, evt);
			});
		}

		private void NdrMessage(MailItem mailItem, SmtpResponse response, QueuedMessageEventSource source, InterceptorAgentRule rule, InterceptorAgentEvent evt)
		{
			EnvelopeRecipientCollection recipients = mailItem.Recipients;
			if (recipients == null || recipients.Count == 0)
			{
				ExTraceGlobals.InterceptorAgentTracer.TraceInformation<string>(0, (long)this.GetHashCode(), "Interceptor routing agent: message with subject '{0} has no recipients to NDR", mailItem.Message.Subject);
				return;
			}
			for (int i = recipients.Count - 1; i >= 0; i--)
			{
				ExTraceGlobals.InterceptorAgentTracer.TraceInformation<string, string>(0, (long)this.GetHashCode(), "Interceptor routing agent dropping message with subject '{0} for recipient '{1}'", mailItem.Message.Subject, recipients[i].Address.ToString());
				string sourceContext = rule.GetSourceContext(base.Name, evt, false);
				mailItem.Recipients.Remove(recipients[i], DsnType.Failure, response, sourceContext);
			}
		}

		private void DeferMessage(MailItem mailItem, TimeSpan deferTime, QueuedMessageEventSource source, InterceptorAgentRule rule, InterceptorAgentEvent evt)
		{
			source.Defer(deferTime, rule.GetSourceContext(base.Name, evt, true));
		}

		private void DeleteMessage(MailItem mailItem, QueuedMessageEventSource source, InterceptorAgentRule rule, InterceptorAgentEvent evt)
		{
			source.Delete(rule.GetSourceContext(base.Name, evt, true));
		}

		private readonly FilteredRuleCache filteredRuleCache;
	}
}
