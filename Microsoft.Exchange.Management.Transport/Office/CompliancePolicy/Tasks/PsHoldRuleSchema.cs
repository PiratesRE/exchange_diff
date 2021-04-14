using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal sealed class PsHoldRuleSchema : PsComplianceRuleBaseSchema
	{
		public static readonly ADPropertyDefinition ContentDateFrom = RuleStorageSchema.ContentDateFrom;

		public static readonly ADPropertyDefinition ContentDateTo = RuleStorageSchema.ContentDateTo;

		public static readonly ADPropertyDefinition HoldContent = RuleStorageSchema.HoldContent;

		public static readonly ADPropertyDefinition HoldDurationDisplayHint = RuleStorageSchema.HoldDurationDisplayHint;
	}
}
