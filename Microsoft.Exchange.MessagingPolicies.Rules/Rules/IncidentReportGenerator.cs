using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class IncidentReportGenerator
	{
		internal static string GenerateIncidentReport(EmailMessage message, TransportRulesEvaluationContext context, IEnumerable<IncidentReportContent> contentItems)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(TransportRulesStrings.IncidentReportDisclaimer);
			stringBuilder.Append("\r\n");
			stringBuilder.Append(TransportRulesStrings.IncidentReportMessageIdLine + ": ");
			stringBuilder.Append(message.MessageId);
			stringBuilder.Append("\r\n");
			if (contentItems.Contains(IncidentReportContent.Sender))
			{
				IncidentReportGenerator.AddSenderLine(message, stringBuilder);
			}
			if (contentItems.Contains(IncidentReportContent.Subject))
			{
				stringBuilder.Append(TransportRulesStrings.IncidentReportSubjectLine + ": ");
				stringBuilder.Append(message.Subject);
				stringBuilder.Append("\r\n");
			}
			if (contentItems.Contains(IncidentReportContent.Recipients))
			{
				IncidentReportGenerator.AddRecipientLines(TransportRulesStrings.IncidentReportToLine + ": ", message.To, 10, stringBuilder);
			}
			if (contentItems.Contains(IncidentReportContent.Cc))
			{
				IncidentReportGenerator.AddRecipientLines(TransportRulesStrings.IncidentReportCcLine + ": ", message.Cc, 10, stringBuilder);
			}
			if (contentItems.Contains(IncidentReportContent.Bcc))
			{
				IncidentReportGenerator.AddRecipientLines(TransportRulesStrings.IncidentReportBccLine + ": ", message.BccFromOrgHeader, 10, stringBuilder);
			}
			if (contentItems.Contains(IncidentReportContent.Severity))
			{
				stringBuilder.Append(TransportRulesStrings.IncidentReportSeverityLine + ": ");
				stringBuilder.Append(context.CurrentAuditSeverityLevel.ToString());
				stringBuilder.Append("\r\n");
			}
			if (contentItems.Contains(IncidentReportContent.Override))
			{
				stringBuilder.Append(TransportRulesStrings.IncidentReportOverrideLine + ": ");
				if (context.SenderOverridden)
				{
					stringBuilder.Append(TransportRulesStrings.Yes);
					if (!string.IsNullOrEmpty(context.SenderOverrideJustification))
					{
						stringBuilder.Append(", ");
						stringBuilder.Append(TransportRulesStrings.IncidentReportJustificationLine + ": ");
						stringBuilder.Append(context.SenderOverrideJustification);
					}
				}
				else
				{
					stringBuilder.Append(TransportRulesStrings.No);
				}
				stringBuilder.Append("\r\n");
			}
			if (contentItems.Contains(IncidentReportContent.FalsePositive))
			{
				stringBuilder.Append(TransportRulesStrings.IncidentReportFalsePositiveLine + ": ");
				stringBuilder.Append(SenderNotify.IsFpHeaderSet(context) ? TransportRulesStrings.Yes : TransportRulesStrings.No);
				stringBuilder.Append("\r\n");
			}
			IEnumerable<DiscoveredDataClassification> ruleMatchingClassifications = IncidentReportGenerator.GetRuleMatchingClassifications(context.RulesEvaluationHistory, context.DataClassifications);
			if (contentItems.Contains(IncidentReportContent.DataClassifications))
			{
				IncidentReportGenerator.AddDataClassificationsLine(ruleMatchingClassifications, 20, stringBuilder);
			}
			if (contentItems.Contains(IncidentReportContent.RuleDetections))
			{
				IncidentReportGenerator.AddRuleHitLines(context, 5, stringBuilder);
			}
			if (contentItems.Contains(IncidentReportContent.IdMatch))
			{
				IncidentReportGenerator.AddDataClassificationIdMatchLines(context.Message, ruleMatchingClassifications, 150, 20, 20, stringBuilder);
			}
			return stringBuilder.ToString();
		}

		internal static void AddSenderLine(EmailMessage message, StringBuilder sb)
		{
			sb.Append(TransportRulesStrings.IncidentReportSenderLine + ": ");
			if (message.Sender != null && !string.IsNullOrEmpty(message.Sender.DisplayName))
			{
				sb.Append(message.Sender.DisplayName);
				sb.Append(", ");
			}
			else if (message.From != null && !string.IsNullOrEmpty(message.From.DisplayName))
			{
				sb.Append(message.From.DisplayName);
				sb.Append(", ");
			}
			sb.Append((message.Sender != null) ? message.Sender.SmtpAddress : "<>");
			sb.Append("\r\n");
		}

		internal static void AddRecipientLine(string lineTitle, EmailRecipient recipient, StringBuilder sb)
		{
			sb.Append(lineTitle);
			if (!string.IsNullOrEmpty(recipient.DisplayName) && !recipient.DisplayName.Equals(recipient.SmtpAddress))
			{
				sb.Append(recipient.DisplayName);
				sb.Append(", ");
			}
			sb.Append(recipient.SmtpAddress);
			sb.Append("\r\n");
		}

		internal static void AddRecipientLines(string lineTitle, EmailRecipientCollection recipients, int maxLineCount, StringBuilder sb)
		{
			int num = recipients.Count - maxLineCount;
			foreach (EmailRecipient recipient in recipients)
			{
				IncidentReportGenerator.AddRecipientLine(lineTitle, recipient, sb);
				if (--maxLineCount == 0)
				{
					if (num > 0)
					{
						sb.Append(lineTitle);
						sb.Append(num.ToString() + " " + TransportRulesStrings.IncidentReportMoreRecipients);
						sb.Append("\r\n");
					}
					break;
				}
			}
		}

		internal static void AddDataClassificationsLine(IEnumerable<DiscoveredDataClassification> classifications, int maxLineCount, StringBuilder sb)
		{
			foreach (DiscoveredDataClassification discoveredDataClassification in classifications)
			{
				sb.Append(TransportRulesStrings.IncidentReportDataClassificationLine);
				sb.Append(": ");
				sb.Append(discoveredDataClassification.ClassificationName);
				sb.Append(", ");
				sb.Append(TransportRulesStrings.IncidentReportCountLine);
				sb.Append(": ");
				sb.Append(discoveredDataClassification.TotalCount);
				sb.Append(", ");
				sb.Append(TransportRulesStrings.IncidentReportConfidenceLine);
				sb.Append(": ");
				sb.Append(discoveredDataClassification.MaxConfidenceLevel);
				sb.Append(", ");
				sb.Append(TransportRulesStrings.IncidentReportRecommendedMinimumConfidenceLine);
				sb.Append(": ");
				sb.Append(discoveredDataClassification.RecommendedMinimumConfidence);
				sb.Append("\r\n");
				if (--maxLineCount == 0)
				{
					break;
				}
			}
		}

		internal static void AddDataClassificationIdMatchLines(MailMessage message, IEnumerable<DiscoveredDataClassification> classifications, int matchSurroundLength, int maxDataClassificationCount, int maxLineCountPerDataClassification, StringBuilder sb)
		{
			foreach (DiscoveredDataClassification discoveredDataClassification in classifications)
			{
				int num = maxLineCountPerDataClassification;
				foreach (DataClassificationMatchLocation dataClassificationMatchLocation in discoveredDataClassification.Locations)
				{
					Tuple<string, string> tuple = null;
					try
					{
						tuple = dataClassificationMatchLocation.GetMatchData(message, matchSurroundLength);
					}
					catch (ArgumentOutOfRangeException)
					{
						ExTraceGlobals.TransportRulesEngineTracer.TraceError(0L, "Match data was corrupted");
					}
					if (tuple != null)
					{
						sb.Append(TransportRulesStrings.IncidentReportIdMatchLine);
						sb.Append(": ");
						sb.Append(discoveredDataClassification.ClassificationName);
						sb.Append(", ");
						sb.Append(TransportRulesStrings.IncidentReportIdMatchValue);
						sb.Append(": ");
						sb.Append(tuple.Item1);
						sb.Append(", ");
						sb.Append("\r\n");
						sb.Append(TransportRulesStrings.IncidentReportIdContext);
						sb.Append(": ");
						sb.Append(tuple.Item2);
						sb.Append("\r\n");
						if (--num == 0)
						{
							break;
						}
					}
				}
				sb.Append("\r\n");
				if (--maxDataClassificationCount == 0)
				{
					break;
				}
			}
		}

		internal static void AddRuleHitLine(TransportRule rule, int maxLineCount, StringBuilder sb)
		{
			sb.Append(TransportRulesStrings.IncidentReportRuleHitLine + ": ");
			sb.Append(rule.Name);
			sb.Append(", ");
			Tuple<Guid, string> tuple;
			if (rule.TryGetDlpPolicy(out tuple))
			{
				sb.Append(TransportRulesStrings.IncidentReportDlpPolicyLine + ": ");
				sb.Append(tuple.Item2);
				sb.Append(", ");
				sb.Append(tuple.Item1.ToString("D"));
				sb.Append(", ");
			}
			int num = rule.Actions.Count;
			if (num > 0)
			{
				sb.Append(TransportRulesStrings.IncidentReportActionLine + ": ");
				foreach (Action action in rule.Actions)
				{
					sb.Append(action.Name);
					if (--maxLineCount == 0)
					{
						break;
					}
					if (--num > 0)
					{
						sb.Append(", ");
					}
				}
			}
			sb.Append("\r\n");
		}

		internal static void AddRuleHitLines(TransportRulesEvaluationContext context, int maxLineCount, StringBuilder sb)
		{
			RulesEvaluationHistory rulesEvaluationHistory = context.RulesEvaluationHistory;
			IEnumerable<Guid> source = from result in rulesEvaluationHistory.History
			where result.Value.IsMatch
			select result into r
			select r.Key;
			IEnumerable<TransportRule> enumerable = from ruleEvaluationResult in source
			select (TransportRule)(from p in context.Rules
			where p.ImmutableId == ruleEvaluationResult
			select p).First<Rule>();
			if (!enumerable.Any<TransportRule>())
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceError(0L, "No matching rules found in rule execution history");
				return;
			}
			foreach (TransportRule rule in enumerable)
			{
				IncidentReportGenerator.AddRuleHitLine(rule, maxLineCount, sb);
			}
		}

		internal static IEnumerable<DiscoveredDataClassification> GetRuleMatchingClassifications(RulesEvaluationHistory ruleHistory, IEnumerable<DiscoveredDataClassification> discoveredDataClassifications)
		{
			HashSet<string> matchingClassificationNames = new HashSet<string>();
			foreach (KeyValuePair<Guid, RuleEvaluationResult> keyValuePair in from h in ruleHistory.History
			where h.Value.IsMatch
			select h)
			{
				IList<PredicateEvaluationResult> predicateEvaluationResult = RuleEvaluationResult.GetPredicateEvaluationResult(typeof(ContainsDataClassificationPredicate), keyValuePair.Value.Predicates);
				IEnumerable<PredicateEvaluationResult> source = predicateEvaluationResult.Where((PredicateEvaluationResult m) => m.SupplementalInfo == 0);
				foreach (string item in source.SelectMany((PredicateEvaluationResult predicateResult) => predicateResult.MatchResults))
				{
					matchingClassificationNames.Add(item);
				}
			}
			return from c in discoveredDataClassifications
			where matchingClassificationNames.Contains(c.ClassificationName)
			select c;
		}

		private const string TitleSeparator = ": ";

		private const string LineTerminator = "\r\n";

		private const string ItemSeparator = ", ";

		private const int MaxRecipientLineCount = 10;

		private const int MaxRuleHitLineCount = 5;

		private const int MaxClassificationMatchLineCount = 20;

		private const int MaxClassificationIdMatchLineCount = 20;

		private const int MatchDataSurroundLength = 150;
	}
}
