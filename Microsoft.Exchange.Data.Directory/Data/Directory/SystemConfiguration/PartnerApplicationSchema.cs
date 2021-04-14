using System;
using System.Linq;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class PartnerApplicationSchema : ADConfigurationObjectSchema
	{
		internal const int EnabledBitShift = 0;

		internal const int UseAuthServerBitShift = 1;

		internal const int AcceptSecurityIdentifierInformationShift = 2;

		public static readonly ADPropertyDefinition ApplicationIdentifier = new ADPropertyDefinition("ApplicationIdentifier", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAuthApplicationIdentifier", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AuthMetadataUrl = new ADPropertyDefinition("AuthMetadataUrl", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAuthMetadataUrl", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Realm = new ADPropertyDefinition("Realm", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAuthRealm", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Flags = new ADPropertyDefinition("Flags", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchAuthFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CertificateDataRaw = new ADPropertyDefinition("CertificateDataRaw", ExchangeObjectVersion.Exchange2010, typeof(byte[]), "msExchAuthCertificateData", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CertificateDataString = new ADPropertyDefinition("CertificateDataString", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			PartnerApplicationSchema.CertificateDataRaw
		}, null, delegate(IPropertyBag propertyBag)
		{
			MultiValuedProperty<byte[]> source = (MultiValuedProperty<byte[]>)propertyBag[PartnerApplicationSchema.CertificateDataRaw];
			return new MultiValuedProperty<string>((from d in source
			select Convert.ToBase64String(d)).ToArray<string>());
		}, null, null, null);

		public static readonly ADPropertyDefinition UseAuthServer = ADObject.BitfieldProperty("UseAuthServer", 1, PartnerApplicationSchema.Flags);

		public static readonly ADPropertyDefinition Enabled = ADObject.BitfieldProperty("Enabled", 0, PartnerApplicationSchema.Flags);

		public static readonly ADPropertyDefinition AcceptSecurityIdentifierInformation = ADObject.BitfieldProperty("AcceptSecurityIdentifierInformation", 2, PartnerApplicationSchema.Flags);

		public static readonly ADPropertyDefinition ThrottlingPolicy = new ADPropertyDefinition("ThrottlingPolicy", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchThrottlingPolicyDN", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LinkedAccount = new ADPropertyDefinition("LinkedAccount", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchAuthLinkedAccount", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConfigurationXMLRaw = XMLSerializableBase.ConfigurationXmlRawProperty();

		public static readonly ADPropertyDefinition ConfigurationXML = XMLSerializableBase.ConfigurationXmlProperty<PartnerApplicationConfigXML>(PartnerApplicationSchema.ConfigurationXMLRaw);

		public static readonly ADPropertyDefinition IssuerIdentifier = XMLSerializableBase.ConfigXmlProperty<PartnerApplicationConfigXML, string>("IssuerIdentifier", ExchangeObjectVersion.Exchange2010, PartnerApplicationSchema.ConfigurationXML, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, (PartnerApplicationConfigXML configXml) => configXml.IssuerIdentifier, delegate(PartnerApplicationConfigXML configXml, string value)
		{
			configXml.IssuerIdentifier = value;
		}, null, null);

		public static readonly ADPropertyDefinition ActAsPermissions = XMLSerializableBase.ConfigXmlProperty<PartnerApplicationConfigXML, string[]>("ActAsPermissions", ExchangeObjectVersion.Exchange2010, PartnerApplicationSchema.ConfigurationXML, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, (PartnerApplicationConfigXML configXml) => configXml.ActAsPermissions, delegate(PartnerApplicationConfigXML configXml, string[] value)
		{
			configXml.ActAsPermissions = value;
		}, null, null);

		public static readonly ADPropertyDefinition AppOnlyPermissions = XMLSerializableBase.ConfigXmlProperty<PartnerApplicationConfigXML, string[]>("AppOnlyPermissions", ExchangeObjectVersion.Exchange2010, PartnerApplicationSchema.ConfigurationXML, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, (PartnerApplicationConfigXML configXml) => configXml.AppOnlyPermissions, delegate(PartnerApplicationConfigXML configXml, string[] value)
		{
			configXml.AppOnlyPermissions = value;
		}, null, null);
	}
}
