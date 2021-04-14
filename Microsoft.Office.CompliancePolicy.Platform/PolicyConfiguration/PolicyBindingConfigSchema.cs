using System;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	internal sealed class PolicyBindingConfigSchema : PolicyConfigBaseSchema
	{
		public static readonly string PolicyDefinitionConfigId = "PolicyDefinitionConfigId";

		public static readonly string PolicyRuleConfigId = "PolicyRuleConfigId";

		public static readonly string PolicyAssociationConfigId = "PolicyAssociationConfigId";

		public static readonly string Scope = "Scope";

		public static readonly string IsExempt = "IsExempt";

		public static readonly string Mode = "Mode";

		public static readonly string WhenAppliedUTC = "WhenAppliedUTC";

		public static readonly string PolicyApplyStatus = "PolicyApplyStatus";
	}
}
