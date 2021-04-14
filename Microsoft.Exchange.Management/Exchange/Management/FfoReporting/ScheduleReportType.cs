using System;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Flags]
	public enum ScheduleReportType
	{
		SpamMailSummary = 1,
		SpamMailDetail = 2,
		MalwareMailSummary = 4,
		MalwareMailDetail = 8,
		RuleMailSummary = 16,
		RuleMailDetail = 32,
		DLPMailSummary = 64,
		DLPMailDetail = 256,
		DLPUnifiedSummary = 512,
		DLPUnifiedDetail = 1024,
		TopDLPMailHits = 4096,
		TopTransportRuleHits = 8192,
		DLPPolicyRuleHits = 16384,
		TopSpamRecipient = 65536,
		TopMailSender = 131072,
		TopMailRecipient = 262144,
		TopMalwareRecipient = 1048576,
		TopMalware = 2097152
	}
}
