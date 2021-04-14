using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UnifiedPolicyRuleActionSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = UnifiedPolicyCommonSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition ObjectIdProperty = UnifiedPolicyCommonSchema.ObjectIdProperty;

		internal static readonly HygienePropertyDefinition DataSourceProperty = UnifiedPolicyCommonSchema.DataSourceProperty;

		internal static readonly HygienePropertyDefinition RuleIdProperty = UnifiedPolicyRuleSchema.RuleIdProperty;

		internal static readonly HygienePropertyDefinition ActionProperty = new HygienePropertyDefinition("Action", typeof(string));
	}
}
