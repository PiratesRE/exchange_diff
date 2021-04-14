using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	internal class DlpPolicySchemaBase : ObjectSchema
	{
		public static readonly ADPropertyDefinition Name = ADObjectSchema.Name;

		public static readonly ADPropertyDefinition Version = new ADPropertyDefinition("Version", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchMailflowPolicyVersion", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition State = new ADPropertyDefinition("State", ExchangeObjectVersion.Exchange2012, typeof(RuleState), "state", ADPropertyDefinitionFlags.None, RuleState.Enabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Mode = new ADPropertyDefinition("Mode", ExchangeObjectVersion.Exchange2012, typeof(RuleMode), "mode", ADPropertyDefinitionFlags.None, RuleMode.Enforce, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ContentVersion = new ADPropertyDefinition("ContentVersion", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchMailflowPolicyVersion", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Description = new ADPropertyDefinition("Description", ExchangeObjectVersion.Exchange2012, typeof(string), "Description", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PublisherName = new ADPropertyDefinition("PublisherName", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchMailflowPolicyPublisherName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Keywords = new ADPropertyDefinition("Keywords", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchMailflowPolicyKeywords", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Guid = ADObjectSchema.Guid;

		public static readonly ADPropertyDefinition DistinguishedName = ADObjectSchema.DistinguishedName;

		public static readonly ADPropertyDefinition ImmutableId = TransportRuleSchema.ImmutableId;
	}
}
