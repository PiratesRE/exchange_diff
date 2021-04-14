using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal sealed class PsDlpComplianceRuleSchema : PsComplianceRuleBaseSchema
	{
		public static readonly ADPropertyDefinition ContentPropertyContainsWords = RuleStorageSchema.ContentPropertyContainsWords;

		public static readonly ADPropertyDefinition ContentContainsSensitiveInformation = RuleStorageSchema.ContentContainsSensitiveInformation;

		public static readonly ADPropertyDefinition AccessScopeIs = RuleStorageSchema.AccessScopeIs;

		public static readonly ADPropertyDefinition BlockAccess = RuleStorageSchema.BlockAccess;
	}
}
