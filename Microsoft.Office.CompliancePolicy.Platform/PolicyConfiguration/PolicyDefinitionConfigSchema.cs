using System;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	internal sealed class PolicyDefinitionConfigSchema : PolicyConfigBaseSchema
	{
		public static readonly string Description = "Description";

		public static readonly string Comment = "Comment";

		public static readonly string DefaultPolicyRuleConfigId = "DefaultPolicyRuleConfigId";

		public static readonly string Mode = "Mode";

		public static readonly string Scenario = "Scenario";

		public static readonly string Enabled = "Enabled";

		public static readonly string CreatedBy = "CreatedBy";

		public static readonly string LastModifiedBy = "LastModifiedBy";
	}
}
