using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.MessagingPolicies.HygieneRules
{
	internal static class HygieneRuleUtils
	{
		public static bool TryRunRuleCollection(RuleCollection rules, SmtpServer server, MailItem mailItem, QueuedMessageEventSource eventSource, out Exception e)
		{
			e = null;
			HygieneTransportRulesEvaluationContext context = new HygieneTransportRulesEvaluationContext(rules, server, eventSource, mailItem);
			try
			{
				HygieneTransportRulesEvaluator hygieneTransportRulesEvaluator = new HygieneTransportRulesEvaluator(context);
				hygieneTransportRulesEvaluator.Run();
			}
			catch (Exception ex)
			{
				e = ex;
				return false;
			}
			return true;
		}

		private static int CompareTransportRule(TransportRule x, TransportRule y)
		{
			return x.Priority.CompareTo(y.Priority);
		}

		public const int FixedObjectOverhead = 18;
	}
}
