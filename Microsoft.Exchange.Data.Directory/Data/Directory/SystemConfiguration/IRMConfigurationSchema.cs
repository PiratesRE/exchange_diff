using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class IRMConfigurationSchema : ADContainerSchema
	{
		internal const int ServerCertificatesVersionShift = 24;

		internal const int ServerCertificatesVersionLength = 8;

		public static readonly ADPropertyDefinition ServiceLocation = new ADPropertyDefinition("ServiceLocation", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchRMSServiceLocationUrl", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition PublishingLocation = new ADPropertyDefinition("PublishingLocation", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchRMSPublishingLocationUrl", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition LicensingLocation = new ADPropertyDefinition("LicensingLocation", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchRMSLicensingLocationUrl", ADPropertyDefinitionFlags.MultiValued, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition Flags = new ADPropertyDefinition("ControlPointFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchControlPointFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 228, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition JournalReportDecryptionEnabled = new ADPropertyDefinition("JournalReportDecryptionEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IRMConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(IRMConfigurationSchema.Flags, 4), ADObject.FlagSetterDelegate(IRMConfigurationSchema.Flags, 4), null, null);

		public static readonly ADPropertyDefinition TransportDecryptionOptional = new ADPropertyDefinition("TransportDecryptionOptional", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IRMConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(IRMConfigurationSchema.Flags, 128), ADObject.FlagSetterDelegate(IRMConfigurationSchema.Flags, 128), null, null);

		public static readonly ADPropertyDefinition TransportDecryptionMandatory = new ADPropertyDefinition("TransportDecryptionMandatory", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IRMConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(IRMConfigurationSchema.Flags, 256), ADObject.FlagSetterDelegate(IRMConfigurationSchema.Flags, 256), null, null);

		public static readonly ADPropertyDefinition ExternalLicensingEnabled = new ADPropertyDefinition("ExternalLicensingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IRMConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(IRMConfigurationSchema.Flags, 16), ADObject.FlagSetterDelegate(IRMConfigurationSchema.Flags, 16), null, null);

		public static readonly ADPropertyDefinition InternalLicensingEnabled = new ADPropertyDefinition("InternalLicensingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IRMConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(IRMConfigurationSchema.Flags, 512), ADObject.FlagSetterDelegate(IRMConfigurationSchema.Flags, 512), null, null);

		public static readonly ADPropertyDefinition SearchEnabled = new ADPropertyDefinition("SearchEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IRMConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(IRMConfigurationSchema.Flags, 32), ADObject.FlagSetterDelegate(IRMConfigurationSchema.Flags, 32), null, null);

		public static readonly ADPropertyDefinition ClientAccessServerEnabled = new ADPropertyDefinition("ClientAccessServerEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IRMConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(IRMConfigurationSchema.Flags, 64), ADObject.FlagSetterDelegate(IRMConfigurationSchema.Flags, 64), null, null);

		public static readonly ADPropertyDefinition InternetConfidentialEnabled = new ADPropertyDefinition("InternetConfidentialEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IRMConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(IRMConfigurationSchema.Flags, 1024), ADObject.FlagSetterDelegate(IRMConfigurationSchema.Flags, 1024), null, null);

		public static readonly ADPropertyDefinition EDiscoverySuperUserDisabled = new ADPropertyDefinition("EDiscoverySuperUserDisabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IRMConfigurationSchema.Flags
		}, null, ADObject.FlagGetterDelegate(IRMConfigurationSchema.Flags, 2048), ADObject.FlagSetterDelegate(IRMConfigurationSchema.Flags, 2048), null, null);

		public static readonly ADPropertyDefinition RMSOnlineKeySharingLocation = new ADPropertyDefinition("RMSOnlineKeySharingLocation", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchRMSOnlineKeySharingLocationUrl", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute),
			new UriSchemeConstraint(new List<string>
			{
				Uri.UriSchemeHttps
			})
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute),
			new UriSchemeConstraint(new List<string>
			{
				Uri.UriSchemeHttps
			})
		}, null, null);

		public static readonly ADPropertyDefinition ServerCertificatesVersion = ADObject.BitfieldProperty("ServerCertificatesVersion", 24, 8, IRMConfigurationSchema.Flags);

		public static readonly ADPropertyDefinition SharedServerBoxRacIdentity = new ADPropertyDefinition("SharedServerBoxRacIdentity", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchSharedIdentityServerBoxRAC", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RMSOnlineVersion = new ADPropertyDefinition("RMSOnlineVersion", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchRMSOnlineCertificationLocationUrl", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
