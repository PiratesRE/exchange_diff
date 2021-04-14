using System;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	internal sealed class PolicyAssociationConfigSchema : PolicyConfigBaseSchema
	{
		public static readonly string Description = "Description";

		public static readonly string Comment = "Comment";

		public static readonly string PolicyDefinitionConfigIds = "PolicyDefinitionConfigIds";

		public static readonly string DefaultPolicyDefinitionConfigId = "DefaultPolicyDefinitionConfigId";

		public static readonly string AllowOverride = "AllowOverride";

		public static readonly string Scope = "Scope";

		public static readonly string WhenAppliedUTC = "WhenAppliedUTC";

		public static readonly string PolicyApplyStatus = "PolicyApplyStatus";
	}
}
