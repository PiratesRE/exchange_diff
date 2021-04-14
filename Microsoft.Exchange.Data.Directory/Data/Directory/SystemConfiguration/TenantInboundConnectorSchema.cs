using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class TenantInboundConnectorSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition Enabled = new ADPropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchSmtpReceiveEnabled", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConnectorType = new ADPropertyDefinition("ConnectorType", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpReceiveType", ADPropertyDefinitionFlags.PersistDefaultValue, 2, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Comment = new ADPropertyDefinition("Comment", ExchangeObjectVersion.Exchange2007, typeof(string), "AdminDescription", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, null, null);

		public static readonly ADPropertyDefinition RemoteIPRanges = new ADPropertyDefinition("RemoteIPRanges", ExchangeObjectVersion.Exchange2007, typeof(IPRange), "msExchSmtpReceiveRemoteIPRanges", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SenderDomainString = new ADPropertyDefinition("SenderDomainString", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchSMTPReceiveSenderDomain", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AssociatedAcceptedDomains = new ADPropertyDefinition("AssociatedAcceptedDomains", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchAssociatedAcceptedDomainLink", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SenderDomains = new ADPropertyDefinition("SenderDomains", ExchangeObjectVersion.Exchange2003, typeof(AddressSpace), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TenantInboundConnectorSchema.SenderDomainString
		}, null, new GetterDelegate(TenantInboundConnector.SenderDomainsGetter), new SetterDelegate(TenantInboundConnector.SenderDomainsSetter), null, null);

		public static readonly ADPropertyDefinition Flags = new ADPropertyDefinition("TenantInboundConnectorFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSMTPReceiveInboundSecurityFlag", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RequireTls = new ADPropertyDefinition("RequireTls", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TenantInboundConnectorSchema.Flags
		}, null, ADObject.FlagGetterDelegate(TenantInboundConnectorSchema.Flags, 32), ADObject.FlagSetterDelegate(TenantInboundConnectorSchema.Flags, 32), null, null);

		public static readonly ADPropertyDefinition RestrictDomainsToIPAddresses = new ADPropertyDefinition("RestrictDomainsToIps", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TenantInboundConnectorSchema.Flags
		}, null, ADObject.FlagGetterDelegate(TenantInboundConnectorSchema.Flags, 16), ADObject.FlagSetterDelegate(TenantInboundConnectorSchema.Flags, 16), null, null);

		public static readonly ADPropertyDefinition RestrictDomainsToCertificate = new ADPropertyDefinition("RestrictDomainsToCertificate", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TenantInboundConnectorSchema.Flags
		}, null, ADObject.FlagGetterDelegate(TenantInboundConnectorSchema.Flags, 64), ADObject.FlagSetterDelegate(TenantInboundConnectorSchema.Flags, 64), null, null);

		public static readonly ADPropertyDefinition CloudServicesMailEnabled = new ADPropertyDefinition("CloudServicesMailEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			TenantInboundConnectorSchema.Flags
		}, null, ADObject.FlagGetterDelegate(TenantInboundConnectorSchema.Flags, 128), ADObject.FlagSetterDelegate(TenantInboundConnectorSchema.Flags, 128), null, null);

		public static readonly ADPropertyDefinition TlsSenderCertificateName = new ADPropertyDefinition("TlsCertificateName", ExchangeObjectVersion.Exchange2007, typeof(TlsCertificate), "msExchSmtpReceiveTlsCertificateName", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new DisallowStarSmtpDomainWithSubdomainsConstraint()
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OnPremisesOrganizationBackLink = new ADPropertyDefinition("OnPremisesOrganizationBackLink", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchOnPremisesInboundConnectorBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConnectorSourceFlags = new ADPropertyDefinition("TenantConnectorSourceFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportInboundSettings", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
