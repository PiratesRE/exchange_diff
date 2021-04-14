using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADComplianceProgramSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition TransportRulesXml = new ADPropertyDefinition("TransportRulesXml", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchMailflowPolicyTransportRulesTemplateXml", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PublisherName = new ADPropertyDefinition("PublisherName", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchMailflowPolicyPublisherName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Version = new ADPropertyDefinition("Version", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchMailflowPolicyVersion", ADPropertyDefinitionFlags.PersistDefaultValue, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition State = new ADPropertyDefinition("State", ExchangeObjectVersion.Exchange2010, typeof(DlpPolicyState), "msExchTransportRuleState", ADPropertyDefinitionFlags.PersistDefaultValue, DlpPolicyState.Disabled_Audit, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Description = new ADPropertyDefinition("Description", ExchangeObjectVersion.Exchange2010, typeof(string), "Description", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, null, null);

		public static readonly ADPropertyDefinition Countries = new ADPropertyDefinition("Countries", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchMailflowPolicyCountries", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Keywords = new ADPropertyDefinition("Keywords", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchMailflowPolicyKeywords", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition ImmutableId = new ADPropertyDefinition("ImmutableId", ExchangeObjectVersion.Exchange2010, typeof(Guid), "msExchImmutableid", ADPropertyDefinitionFlags.WriteOnce, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
