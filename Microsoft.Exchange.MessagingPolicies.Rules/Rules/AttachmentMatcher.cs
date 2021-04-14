using System;
using Microsoft.Filtering;
using Microsoft.Filtering.Results;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class AttachmentMatcher
	{
		internal static bool AttachmentMatches(Value value, RulesEvaluationContext context, AttachmentMatcher.TracingDelegate tracer)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)context;
			MultiMatcher multiMatcher = value.GetValue(transportRulesEvaluationContext) as MultiMatcher;
			if (multiMatcher == null)
			{
				throw new RuleInvalidOperationException(RulesStrings.InvalidValue("MatchAnyIMatch"));
			}
			int num = 0;
			int attachmentTextScanLimit = TransportRulesEvaluationContext.GetAttachmentTextScanLimit(transportRulesEvaluationContext.MailItem);
			try
			{
				foreach (StreamIdentity streamIdentity in transportRulesEvaluationContext.Message.GetSupportedAttachmentStreamIdentities())
				{
					tracer(0L, "Scanning attachment: '{0}'.", new object[]
					{
						streamIdentity.Name
					});
					if (streamIdentity.Content.IsTextAvailable)
					{
						bool flag = multiMatcher.IsMatch(RuleAgentResultUtils.GetSubjectPrependedReader(streamIdentity), "attachmentMatchesRegexPatterns" + '.' + num++, attachmentTextScanLimit, context);
						if (flag)
						{
							return true;
						}
					}
				}
			}
			catch (NotSupportedException ex)
			{
				string text = TransportRulesStrings.AttachmentReadError(string.Format("NotSupportedException. Most likely reason - FIPS not configured properly. Check if TextExtractionHandler is enabled in FIP-FS\\Data\\configuration.xml. Error message {0}", ex.Message));
				tracer(0L, text, new object[0]);
				throw new TransportRulePermanentException(text, ex);
			}
			return false;
		}

		internal delegate void TracingDelegate(long id, string formatString, params object[] args);
	}
}
