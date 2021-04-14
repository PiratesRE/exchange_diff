using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UnifiedPolicyRuleClassificationSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = UnifiedPolicyCommonSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition ObjectIdProperty = UnifiedPolicyCommonSchema.ObjectIdProperty;

		internal static readonly HygienePropertyDefinition DataSourceProperty = UnifiedPolicyCommonSchema.DataSourceProperty;

		internal static readonly HygienePropertyDefinition RuleIdProperty = UnifiedPolicyRuleSchema.RuleIdProperty;

		internal static readonly HygienePropertyDefinition ClassificationIdProperty = new HygienePropertyDefinition("ClassificationId", typeof(Guid));

		internal static readonly HygienePropertyDefinition CountProperty = new HygienePropertyDefinition("Count", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ConfidenceProperty = new HygienePropertyDefinition("Confidence", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
