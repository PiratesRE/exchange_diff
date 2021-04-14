using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal static class RoutingActionUtils
	{
		internal static void ProcessRecipients(TransportRulesEvaluationContext context, Action<EnvelopeRecipient> onRecipient)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (onRecipient == null)
			{
				throw new ArgumentNullException("onRecipient");
			}
			IEnumerable<EnvelopeRecipient> enumerable;
			if (context.MatchedRecipients == null)
			{
				enumerable = context.MailItem.Recipients;
			}
			else
			{
				enumerable = context.MatchedRecipients;
			}
			foreach (EnvelopeRecipient obj in enumerable)
			{
				onRecipient(obj);
			}
		}
	}
}
