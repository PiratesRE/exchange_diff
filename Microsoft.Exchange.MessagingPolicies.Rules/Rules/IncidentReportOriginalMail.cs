using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public enum IncidentReportOriginalMail
	{
		[LocDescription(TransportRulesStrings.IDs.IncidentReportIncludeOriginalMail)]
		IncludeOriginalMail = 1,
		[LocDescription(TransportRulesStrings.IDs.IncidentReportDoNotIncludeOriginalMail)]
		DoNotIncludeOriginalMail
	}
}
