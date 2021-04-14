using System;
using System.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UnifiedPolicyDataSetSchema
	{
		internal static readonly HygienePropertyDefinition UnifiedPolicyObjectTableProperty = new HygienePropertyDefinition("tvp_UnifiedPolicyObjects", typeof(DataTable));

		internal static readonly HygienePropertyDefinition UnifiedPolicyRuleTableProperty = new HygienePropertyDefinition("tvp_UnifiedPolicyRules", typeof(DataTable));

		internal static readonly HygienePropertyDefinition UnifiedPolicyRuleActionTableProperty = new HygienePropertyDefinition("tvp_UnifiedPolicyRuleActions", typeof(DataTable));

		internal static readonly HygienePropertyDefinition UnifiedPolicyRuleClassificationTableProperty = new HygienePropertyDefinition("tvp_UnifiedPolicyRuleClassifications", typeof(DataTable));
	}
}
