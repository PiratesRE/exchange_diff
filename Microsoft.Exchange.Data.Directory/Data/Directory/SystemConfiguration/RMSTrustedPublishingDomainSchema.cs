using System;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.RightsManagementServices.Provider;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class RMSTrustedPublishingDomainSchema : ADContainerSchema
	{
		private static object CryptoModeGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[RMSTrustedPublishingDomainSchema.SLCCertChain];
			if (!string.IsNullOrEmpty(text))
			{
				XrmlCertificateChain xrmlCertificateChain = RMUtil.DecompressSLCCertificate(text);
				return xrmlCertificateChain.GetCryptoMode();
			}
			return RMSTrustedPublishingDomainSchema.CryptoMode.DefaultValue;
		}

		private static object IsRMSOnlineGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[RMSTrustedPublishingDomainSchema.SLCCertChain];
			return !string.IsNullOrEmpty(text) && RMSUtil.IsRMSOnline(RMUtil.DecompressSLCCertificate(text).ToStringArray());
		}

		public static readonly ADPropertyDefinition CSPName = new ADPropertyDefinition("CSPName", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchTPDCSPName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CSPType = new ADPropertyDefinition("CSPType", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTPDCSPType", ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition KeyId = new ADPropertyDefinition("KeyId", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchTPDKeyID", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition KeyIdType = new ADPropertyDefinition("KeyIdType", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchTPDKeyIDType", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition KeyContainerName = new ADPropertyDefinition("KeyContainerName", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchTPDKeyContainerName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition KeyNumber = new ADPropertyDefinition("KeyNumber", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTPDKeyNumber", ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IntranetLicensingUrl = new ADPropertyDefinition("IntranetLicensingUrl", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchTPDIntranetLicensingUrl", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition ExtranetLicensingUrl = new ADPropertyDefinition("ExtranetLicensingUrl", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchTPDExtranetLicensingUrl", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition IntranetCertificationUrl = new ADPropertyDefinition("IntranetCertificationUrl", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchTPDIntranetCertificationUrl", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition ExtranetCertificationUrl = new ADPropertyDefinition("ExtranetCertificationUrl", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchTPDExtranetCertificationUrl", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition Flags = new ADPropertyDefinition("TPDFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTPDFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Default = new ADPropertyDefinition("Default", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RMSTrustedPublishingDomainSchema.Flags
		}, null, ADObject.FlagGetterDelegate(RMSTrustedPublishingDomainSchema.Flags, 1), ADObject.FlagSetterDelegate(RMSTrustedPublishingDomainSchema.Flags, 1), null, null);

		public static readonly ADPropertyDefinition PrivateKey = new ADPropertyDefinition("PrivateKey", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchTPDPrivateKey", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 8192)
		}, null, null);

		public static readonly ADPropertyDefinition SLCCertChain = new ADPropertyDefinition("SLCCertChain", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchTPDSLCCertificateChain", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RMSTemplates = new ADPropertyDefinition("RMSTemplates", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchTPDTemplates", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 25600)
		}, null, null);

		public static readonly ADPropertyDefinition CryptoMode = new ADPropertyDefinition("CryptoMode", ExchangeObjectVersion.Exchange2007, typeof(int), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, 1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RMSTrustedPublishingDomainSchema.SLCCertChain
		}, null, new GetterDelegate(RMSTrustedPublishingDomainSchema.CryptoModeGetter), null, null, null);

		public static readonly ADPropertyDefinition IsRMSOnline = new ADPropertyDefinition("IsRMSOnline", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			RMSTrustedPublishingDomainSchema.SLCCertChain
		}, null, new GetterDelegate(RMSTrustedPublishingDomainSchema.IsRMSOnlineGetter), null, null, null);
	}
}
