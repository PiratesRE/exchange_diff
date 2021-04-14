using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UnifiedPolicyRuleSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = UnifiedPolicyCommonSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition ObjectIdProperty = UnifiedPolicyCommonSchema.ObjectIdProperty;

		internal static readonly HygienePropertyDefinition DataSourceProperty = UnifiedPolicyCommonSchema.DataSourceProperty;

		internal static readonly HygienePropertyDefinition RuleIdProperty = new HygienePropertyDefinition("RuleId", typeof(Guid));

		internal static readonly HygienePropertyDefinition DLPIdProperty = new HygienePropertyDefinition("DLPId", typeof(Guid?));

		internal static readonly HygienePropertyDefinition ModeProperty = new HygienePropertyDefinition("Mode", typeof(string));

		internal static readonly HygienePropertyDefinition SeverityProperty = new HygienePropertyDefinition("Severity", typeof(string));

		internal static readonly HygienePropertyDefinition OverrideTypeProperty = new HygienePropertyDefinition("OverrideType", typeof(string));

		internal static readonly HygienePropertyDefinition OverrideJustificationProperty = new HygienePropertyDefinition("OverrideJustification", typeof(string));
	}
}
