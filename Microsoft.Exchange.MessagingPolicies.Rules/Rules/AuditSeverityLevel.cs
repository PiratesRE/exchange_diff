using System;
using Microsoft.Exchange.Core;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal enum AuditSeverityLevel
	{
		[LocDescription(CoreStrings.IDs.AuditSeverityLevelLow)]
		Low = 1,
		[LocDescription(CoreStrings.IDs.AuditSeverityLevelMedium)]
		Medium,
		[LocDescription(CoreStrings.IDs.AuditSeverityLevelHigh)]
		High,
		[LocDescription(CoreStrings.IDs.AuditSeverityLevelDoNotAudit)]
		DoNotAudit
	}
}
