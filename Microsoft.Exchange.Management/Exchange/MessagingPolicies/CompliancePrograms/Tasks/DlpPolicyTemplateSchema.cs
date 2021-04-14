using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	internal class DlpPolicyTemplateSchema : DlpPolicySchemaBase
	{
		public static readonly ADPropertyDefinition LocalizedName = new ADPropertyDefinition("LocalizedName", ExchangeObjectVersion.Exchange2012, typeof(string), "localizedName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RuleParameters = new ADPropertyDefinition("RuleParameters", ExchangeObjectVersion.Exchange2012, typeof(string), "ruleParameters", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
