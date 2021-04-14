using System;
using System.Linq;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class AuthServerSchema : ADConfigurationObjectSchema
	{
		internal const int EnabledBitShift = 0;

		internal const int IsDefaultAuthorizationEndpointBitShift = 1;

		public static readonly ADPropertyDefinition IssuerIdentifier = new ADPropertyDefinition("IssuerIdentifier", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAuthIssuerName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TokenIssuingEndpoint = new ADPropertyDefinition("TokenIssuingEndpoint", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAuthIssuingUrl", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AuthorizationEndpoint = new ADPropertyDefinition("AuthorizationEndpoint", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAuthAuthorizationUrl", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ApplicationIdentifier = new ADPropertyDefinition("ApplicationIdentifier", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAuthApplicationIdentifier", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AuthMetadataUrl = new ADPropertyDefinition("AuthMetadataUrl", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAuthMetadataUrl", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Realm = new ADPropertyDefinition("Realm", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAuthRealm", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CertificateDataRaw = new ADPropertyDefinition("CertificateDataRaw", ExchangeObjectVersion.Exchange2010, typeof(byte[]), "msExchAuthCertificateData", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CertificateDataString = new ADPropertyDefinition("CertificateDataString", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AuthServerSchema.CertificateDataRaw
		}, null, delegate(IPropertyBag propertyBag)
		{
			MultiValuedProperty<byte[]> source = (MultiValuedProperty<byte[]>)propertyBag[AuthServerSchema.CertificateDataRaw];
			return new MultiValuedProperty<string>((from d in source
			select Convert.ToBase64String(d)).ToArray<string>());
		}, null, null, null);

		public static readonly ADPropertyDefinition AppSecretRaw = new ADPropertyDefinition("AppSecretRaw", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchAuthAppSecret", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CurrentEncryptedAppSecret = new ADPropertyDefinition("CurrentEncryptedAppSecret", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AuthServerSchema.AppSecretRaw
		}, null, (IPropertyBag b) => AuthConfigSchema.MultiValuedStringKeyGetter(b, AuthServerSchema.AppSecretRaw, 0), delegate(object v, IPropertyBag b)
		{
			AuthConfigSchema.MultiValuedStringKeySetter(v, b, AuthServerSchema.AppSecretRaw, 0);
		}, null, null);

		public static readonly ADPropertyDefinition PreviousEncryptedAppSecret = new ADPropertyDefinition("PreviousEncryptedAppSecret", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AuthServerSchema.AppSecretRaw
		}, null, (IPropertyBag b) => AuthConfigSchema.MultiValuedStringKeyGetter(b, AuthServerSchema.AppSecretRaw, 1), delegate(object v, IPropertyBag b)
		{
			AuthConfigSchema.MultiValuedStringKeySetter(v, b, AuthServerSchema.AppSecretRaw, 1);
		}, null, null);

		public static readonly ADPropertyDefinition Type = new ADPropertyDefinition("Type", ExchangeObjectVersion.Exchange2010, typeof(AuthServerType), "msExchAuthAuthServerType", ADPropertyDefinitionFlags.PersistDefaultValue, AuthServerType.Unknown, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Flags = new ADPropertyDefinition("Flags", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchAuthFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Enabled = ADObject.BitfieldProperty("Enabled", 0, AuthServerSchema.Flags);

		public static readonly ADPropertyDefinition IsDefaultAuthorizationEndpoint = ADObject.BitfieldProperty("IsDefaultAuthorizationEndpoint", 1, AuthServerSchema.Flags);
	}
}
