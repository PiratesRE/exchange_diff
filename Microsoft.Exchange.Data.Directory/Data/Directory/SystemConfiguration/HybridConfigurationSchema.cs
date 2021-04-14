using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class HybridConfigurationSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition ClientAccessServers = new ADPropertyDefinition("ClientAccessServers", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchCoexistenceServers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SendingTransportServers = new ADPropertyDefinition("SendingTransportServers", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchCoexistenceTransportServers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReceivingTransportServers = new ADPropertyDefinition("ReceivingTransportServers", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchCoexistenceFrontendTransportServers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EdgeTransportServers = new ADPropertyDefinition("EdgeTransportServers", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchCoexistenceEdgeTransportServers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TlsCertificateName = new ADPropertyDefinition("TlsCertificateName", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchCoexistenceSecureMailCertificateThumbprint", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OnPremisesSmartHost = new ADPropertyDefinition("OnPremisesSmartHost", ExchangeObjectVersion.Exchange2010, typeof(SmtpDomain), "msExchCoexistenceOnPremisesSmartHost", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Domains = new ADPropertyDefinition("Domains", ExchangeObjectVersion.Exchange2010, typeof(AutoDiscoverSmtpDomain), "msExchCoexistenceDomains", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalIPAddresses = new ADPropertyDefinition("ExternalIPAddresses", ExchangeObjectVersion.Exchange2010, typeof(IPRange), "msExchCoexistenceExternalIPAddresses", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Flags = new ADPropertyDefinition("CoexistenceFeatureFlags", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchCoexistenceFeatureFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FreeBusySharingEnabled = new ADPropertyDefinition("FreeBusySharingEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HybridConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(HybridConfigurationSchema.Flags, 1), ADObject.FlagSetterDelegate(HybridConfigurationSchema.Flags, 1), null, null);

		public static readonly ADPropertyDefinition MoveMailboxEnabled = new ADPropertyDefinition("MoveMailboxEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HybridConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(HybridConfigurationSchema.Flags, 2), ADObject.FlagSetterDelegate(HybridConfigurationSchema.Flags, 2), null, null);

		public static readonly ADPropertyDefinition MailtipsEnabled = new ADPropertyDefinition("MailtipsEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HybridConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(HybridConfigurationSchema.Flags, 4), ADObject.FlagSetterDelegate(HybridConfigurationSchema.Flags, 4), null, null);

		public static readonly ADPropertyDefinition MessageTrackingEnabled = new ADPropertyDefinition("MessageTrackingEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HybridConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(HybridConfigurationSchema.Flags, 8), ADObject.FlagSetterDelegate(HybridConfigurationSchema.Flags, 8), null, null);

		public static readonly ADPropertyDefinition OwaRedirectionEnabled = new ADPropertyDefinition("OwaRedirectionEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HybridConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(HybridConfigurationSchema.Flags, 16), ADObject.FlagSetterDelegate(HybridConfigurationSchema.Flags, 16), null, null);

		public static readonly ADPropertyDefinition OnlineArchiveEnabled = new ADPropertyDefinition("OnlineArchiveEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HybridConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(HybridConfigurationSchema.Flags, 32), ADObject.FlagSetterDelegate(HybridConfigurationSchema.Flags, 32), null, null);

		public static readonly ADPropertyDefinition SecureMailEnabled = new ADPropertyDefinition("SecureMailEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HybridConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(HybridConfigurationSchema.Flags, 64), ADObject.FlagSetterDelegate(HybridConfigurationSchema.Flags, 64), null, null);

		public static readonly ADPropertyDefinition CentralizedTransportOnPremEnabled = new ADPropertyDefinition("CentralizedTransportOnPremEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HybridConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(HybridConfigurationSchema.Flags, 128), ADObject.FlagSetterDelegate(HybridConfigurationSchema.Flags, 128), null, null);

		public static readonly ADPropertyDefinition CentralizedTransportInCloudEnabled = new ADPropertyDefinition("CentralizedTransportInCloudEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HybridConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(HybridConfigurationSchema.Flags, 256), ADObject.FlagSetterDelegate(HybridConfigurationSchema.Flags, 256), null, null);

		public static readonly ADPropertyDefinition Features = new ADPropertyDefinition("Features", ExchangeObjectVersion.Exchange2010, typeof(HybridFeature), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HybridConfigurationSchema.Flags
		}, null, new GetterDelegate(HybridConfiguration.FeaturesGetter), new SetterDelegate(HybridConfiguration.FeaturesSetter), null, null);

		public static readonly ADPropertyDefinition PhotosEnabled = new ADPropertyDefinition("PhotosEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			HybridConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(HybridConfigurationSchema.Flags, 512), ADObject.FlagSetterDelegate(HybridConfigurationSchema.Flags, 512), null, null);

		public static readonly ADPropertyDefinition ServiceInstanceFlags = new ADPropertyDefinition("ServiceInstanceFlags", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchManagementSettings", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ServiceInstance = ADObject.BitfieldProperty("ServiceInstance", 0, 5, HybridConfigurationSchema.ServiceInstanceFlags);
	}
}
