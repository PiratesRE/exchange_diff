using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public enum IncidentReportContent
	{
		[LocDescription(TransportRulesStrings.IDs.IncidentReportSender)]
		Sender = 1,
		[LocDescription(TransportRulesStrings.IDs.IncidentReportRecipients)]
		Recipients,
		[LocDescription(TransportRulesStrings.IDs.IncidentReportSubject)]
		Subject,
		[LocDescription(TransportRulesStrings.IDs.IncidentReportCc)]
		Cc,
		[LocDescription(TransportRulesStrings.IDs.IncidentReportBcc)]
		Bcc,
		[LocDescription(TransportRulesStrings.IDs.IncidentReportSeverity)]
		Severity,
		[LocDescription(TransportRulesStrings.IDs.IncidentReportOverride)]
		Override,
		[LocDescription(TransportRulesStrings.IDs.IncidentReportRuleDetections)]
		RuleDetections,
		[LocDescription(TransportRulesStrings.IDs.IncidentReportFalsePositive)]
		FalsePositive,
		[LocDescription(TransportRulesStrings.IDs.IncidentReportDataClassifications)]
		DataClassifications,
		[LocDescription(TransportRulesStrings.IDs.IncidentReportIdMatch)]
		IdMatch,
		[LocDescription(TransportRulesStrings.IDs.IncidentReportAttachOriginalMail)]
		AttachOriginalMail
	}
}
