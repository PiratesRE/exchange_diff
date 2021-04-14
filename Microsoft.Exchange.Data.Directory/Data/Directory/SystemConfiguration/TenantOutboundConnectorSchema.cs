using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class TenantOutboundConnectorSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition Enabled = new ADPropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchSmtpSendEnabled", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SmartHostTypeFlag = new ADPropertyDefinition("SmartHostType", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpSmartHostType", ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Comment = new ADPropertyDefinition("Comment", ExchangeObjectVersion.Exchange2007, typeof(string), "AdminDescription", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, null, null);

		public static readonly ADPropertyDefinition UseMxRecord = new ADPropertyDefinition("UseMx", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TenantOutboundConnectorSchema.SmartHostTypeFlag
		}, null, ADObject.FlagGetterDelegate(TenantOutboundConnectorSchema.SmartHostTypeFlag, 1), ADObject.FlagSetterDelegate(TenantOutboundConnectorSchema.SmartHostTypeFlag, 1), null, null);

		public static readonly ADPropertyDefinition ConnectorType = new ADPropertyDefinition("ConnectorType", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpSendType", ADPropertyDefinitionFlags.PersistDefaultValue, 2, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConnectorSourceFlags = new ADPropertyDefinition("TenantConnectorSourceFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportOutboundSettings", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition RecipientDomains = new ADPropertyDefinition("RecipientDomains", ExchangeObjectVersion.Exchange2003, typeof(string), "routingList", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RecipientDomainsEx = new ADPropertyDefinition("RecipientDomainsEx", ExchangeObjectVersion.Exchange2003, typeof(SmtpDomainWithSubdomains), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TenantOutboundConnectorSchema.RecipientDomains
		}, null, new GetterDelegate(TenantOutboundConnector.RecipientDomainsGetter), new SetterDelegate(TenantOutboundConnector.RecipientDomainsSetter), null, null);

		public static readonly ADPropertyDefinition SmartHostsString = new ADPropertyDefinition("SmartHostsString", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchSmtpSmartHost", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SmartHosts = new ADPropertyDefinition("SmartHosts", ExchangeObjectVersion.Exchange2003, typeof(SmartHost), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TenantOutboundConnectorSchema.SmartHostsString
		}, null, new GetterDelegate(TenantOutboundConnector.SmartHostsGetter), new SetterDelegate(TenantOutboundConnector.SmartHostsSetter), null, null);

		public static readonly ADPropertyDefinition OutboundConnectorFlags = new ADPropertyDefinition("SendConnectorFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpSendFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TlsSettings = new ADPropertyDefinition("TlsSettings", ExchangeObjectVersion.Exchange2007, typeof(TlsAuthLevel?), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TenantOutboundConnectorSchema.OutboundConnectorFlags
		}, null, new GetterDelegate(TenantOutboundConnector.TlsAuthLevelGetter), new SetterDelegate(TenantOutboundConnector.TlsAuthLevelSetter), null, null);

		public static readonly ADPropertyDefinition TlsDomain = new ADPropertyDefinition("TlsDomain", ExchangeObjectVersion.Exchange2007, typeof(SmtpDomainWithSubdomains), "msExchSmtpSendTlsDomain", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new DisallowStarSmtpDomainWithSubdomainsConstraint()
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsTransportRuleScoped = new ADPropertyDefinition("IsTransportRuleScoped", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TenantOutboundConnectorSchema.OutboundConnectorFlags
		}, null, ADObject.FlagGetterDelegate(TenantOutboundConnectorSchema.OutboundConnectorFlags, 4096), ADObject.FlagSetterDelegate(TenantOutboundConnectorSchema.OutboundConnectorFlags, 4096), null, null);

		public static readonly ADPropertyDefinition OnPremisesOrganizationBackLink = new ADPropertyDefinition("OnPremisesOrganizationBackLink", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchOnPremisesOutboundConnectorBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RouteAllMessagesViaOnPremises = new ADPropertyDefinition("RouteAllMessagesViaOnPremises", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TenantOutboundConnectorSchema.OutboundConnectorFlags
		}, null, ADObject.FlagGetterDelegate(TenantOutboundConnectorSchema.OutboundConnectorFlags, 1), ADObject.FlagSetterDelegate(TenantOutboundConnectorSchema.OutboundConnectorFlags, 1), null, null);

		public static readonly ADPropertyDefinition CloudServicesMailEnabled = new ADPropertyDefinition("CloudServicesMailEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TenantOutboundConnectorSchema.OutboundConnectorFlags
		}, null, ADObject.FlagGetterDelegate(TenantOutboundConnectorSchema.OutboundConnectorFlags, 8192), ADObject.FlagSetterDelegate(TenantOutboundConnectorSchema.OutboundConnectorFlags, 8192), null, null);

		public static readonly ADPropertyDefinition AllAcceptedDomains = new ADPropertyDefinition("AllAcceptedDomains", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TenantOutboundConnectorSchema.OutboundConnectorFlags
		}, null, ADObject.FlagGetterDelegate(TenantOutboundConnectorSchema.OutboundConnectorFlags, 16384), ADObject.FlagSetterDelegate(TenantOutboundConnectorSchema.OutboundConnectorFlags, 16384), null, null);
	}
}
