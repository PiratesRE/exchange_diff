using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class FederationTrustSchema : ADConfigurationObjectSchema
	{
		internal static GetterDelegate CertificateGetterDelegate(ProviderPropertyDefinition propertyDefinition)
		{
			return delegate(IPropertyBag propertyBag)
			{
				byte[] array = propertyBag[propertyDefinition] as byte[];
				if (array == null || array.Length == 0)
				{
					return null;
				}
				object result;
				try
				{
					result = new X509Certificate2(array);
				}
				catch (CryptographicException ex)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(propertyDefinition.Name, ex.Message), propertyDefinition, propertyBag[ADObjectSchema.Id]), ex);
				}
				return result;
			};
		}

		internal static SetterDelegate CertificateSetterDelegate(ProviderPropertyDefinition propertyDefinition)
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				if (value == null)
				{
					propertyBag[propertyDefinition] = value;
					return;
				}
				X509Certificate2 x509Certificate = value as X509Certificate2;
				if (x509Certificate == null)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ExArgumentOutOfRangeException(propertyDefinition.Name, value.ToString()), propertyDefinition, propertyBag[ADObjectSchema.Id]), null);
				}
				propertyBag[propertyDefinition] = x509Certificate.Export(X509ContentType.SerializedCert);
			};
		}

		internal static object PartnerTypeGetter(IPropertyBag propertyBag)
		{
			string text = propertyBag[FederationTrustSchema.RawTokenIssuerType] as string;
			if (string.IsNullOrEmpty(text))
			{
				return FederationTrust.PartnerSTSType.LiveId;
			}
			int num = Array.IndexOf<string>(FederationTrustSchema.partnerSTSTypeProgIds, text.Trim().ToUpper());
			if (0 > num)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(FederationTrustSchema.RawTokenIssuerType.Name, text), FederationTrustSchema.RawTokenIssuerType, propertyBag[ADObjectSchema.Id]));
			}
			return (FederationTrust.PartnerSTSType)num;
		}

		internal static void PartnerTypeSetter(object value, IPropertyBag propertyBag)
		{
			if (value == null)
			{
				propertyBag[FederationTrustSchema.RawTokenIssuerType] = value;
				return;
			}
			try
			{
				propertyBag[FederationTrustSchema.RawTokenIssuerType] = FederationTrustSchema.partnerSTSTypeProgIds[(int)value];
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ExArgumentOutOfRangeException(FederationTrustSchema.RawTokenIssuerType.Name, value.ToString()), FederationTrustSchema.RawTokenIssuerType, propertyBag[ADObjectSchema.Id]), null);
			}
		}

		internal static object NamespaceProvisionerTypeGetter(IPropertyBag propertyBag)
		{
			string text = propertyBag[FederationTrustSchema.RawAdminDescription] as string;
			string namespaceProvisioner = FederationTrustProvisioningControl.GetNamespaceProvisioner(text);
			if (string.IsNullOrEmpty(namespaceProvisioner))
			{
				return FederationTrust.NamespaceProvisionerType.ExternalProcess;
			}
			int num = Array.IndexOf<string>(FederationTrustSchema.namespaceProvisionerProgIds, namespaceProvisioner.Trim().ToUpper());
			if (0 > num)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(FederationTrustSchema.RawAdminDescription.Name, text), FederationTrustSchema.RawAdminDescription, propertyBag[ADObjectSchema.Id]));
			}
			return (FederationTrust.NamespaceProvisionerType)num;
		}

		internal static void NamespaceProvisionerTypeSetter(object value, IPropertyBag propertyBag)
		{
			try
			{
				propertyBag[FederationTrustSchema.RawAdminDescription] = FederationTrustProvisioningControl.PutNamespaceProvisioner(FederationTrustSchema.namespaceProvisionerProgIds[(int)value], propertyBag[FederationTrustSchema.RawAdminDescription] as string);
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ExArgumentOutOfRangeException(FederationTrustSchema.RawAdminDescription.Name, value.ToString()), FederationTrustSchema.RawAdminDescription, propertyBag[ADObjectSchema.Id]), null);
			}
		}

		internal static object AdministratorProvisioningIdGetter(IPropertyBag propertyBag)
		{
			string text = propertyBag[FederationTrustSchema.RawAdminDescription] as string;
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			return FederationTrustProvisioningControl.GetAdministratorProvisioningId(text);
		}

		internal static void AdministratorProvisioningIdSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[FederationTrustSchema.RawAdminDescription] = FederationTrustProvisioningControl.PutAdministratorProvisioningId(value as string, propertyBag[FederationTrustSchema.RawAdminDescription] as string);
		}

		public static readonly ADPropertyDefinition RawAdminDescription = new ADPropertyDefinition("RawAdminDescription", ExchangeObjectVersion.Exchange2010, typeof(string), "adminDescription", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ApplicationIdentifier = new ADPropertyDefinition("ApplicationIdentifier", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchFedApplicationId", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 128)
		}, null, null);

		public static readonly ADPropertyDefinition AdministratorProvisioningId = new ADPropertyDefinition("AdministratorProvisioningId", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FederationTrustSchema.RawAdminDescription
		}, null, new GetterDelegate(FederationTrustSchema.AdministratorProvisioningIdGetter), new SetterDelegate(FederationTrustSchema.AdministratorProvisioningIdSetter), null, null);

		public static readonly ADPropertyDefinition ApplicationUri = new ADPropertyDefinition("ApplicationUri", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchFedApplicationUri", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.RelativeOrAbsolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.RelativeOrAbsolute)
		}, null, null);

		public static readonly ADPropertyDefinition PolicyReferenceUri = new ADPropertyDefinition("PolicyReferenceUri", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchFedPolicyReferenceURI", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TokenIssuerMetadataEpr = new ADPropertyDefinition("TokenIssuerMetadataEpr", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchFedTokenIssuerMetadataEPR", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition MetadataPollInterval = new ADPropertyDefinition("MetadataPollInterval", ExchangeObjectVersion.Exchange2010, typeof(EnhancedTimeSpan), "msExchFedMetadataPollInterval", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMinutes(1440.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneMinute, EnhancedTimeSpan.FromDays(365.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneMinute)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RawTokenIssuerType = new ADPropertyDefinition("RawTokenIssuerType", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchFedTokenIssuerType", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TokenIssuerType = new ADPropertyDefinition("TokenIssuerType", ExchangeObjectVersion.Exchange2010, typeof(FederationTrust.PartnerSTSType), null, ADPropertyDefinitionFlags.Calculated, FederationTrust.PartnerSTSType.LiveId, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FederationTrustSchema.RawTokenIssuerType
		}, null, new GetterDelegate(FederationTrustSchema.PartnerTypeGetter), new SetterDelegate(FederationTrustSchema.PartnerTypeSetter), null, null);

		public static readonly ADPropertyDefinition TokenIssuerUri = new ADPropertyDefinition("TokenIssuerUri", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchFedTokenIssuerURI", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition TokenIssuerEpr = new ADPropertyDefinition("TokenIssuerEpr", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchFedTokenIssuerEPR", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition WebRequestorRedirectEpr = new ADPropertyDefinition("WebRequestorRedirectEpr", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchFedWebRequestorRedirectEPR", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition MetadataEpr = new ADPropertyDefinition("MetadataEpr", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchFedMetadataEPR", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition MetadataPutEpr = new ADPropertyDefinition("MetadataPutEpr", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchFedMetadataPutEPR", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition RawOrgCertificate = new ADPropertyDefinition("RawOrgCertificate", ExchangeObjectVersion.Exchange2010, typeof(byte[]), "msExchFedOrgCertificate", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OrgCertificate = new ADPropertyDefinition("OrgCertificate", ExchangeObjectVersion.Exchange2010, typeof(X509Certificate2), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FederationTrustSchema.RawOrgCertificate
		}, null, FederationTrustSchema.CertificateGetterDelegate(FederationTrustSchema.RawOrgCertificate), FederationTrustSchema.CertificateSetterDelegate(FederationTrustSchema.RawOrgCertificate), null, null);

		public static readonly ADPropertyDefinition OrgPrivCertificate = new ADPropertyDefinition("OrgPrivCertificate", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchFedOrgPrivCertificate", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 256)
		}, null, null);

		public static readonly ADPropertyDefinition RawOrgNextCertificate = new ADPropertyDefinition("RawOrgNextCertificate", ExchangeObjectVersion.Exchange2010, typeof(byte[]), "msExchFedOrgNextCertificate", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OrgNextCertificate = new ADPropertyDefinition("OrgNextCertificate", ExchangeObjectVersion.Exchange2010, typeof(X509Certificate2), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FederationTrustSchema.RawOrgNextCertificate
		}, null, FederationTrustSchema.CertificateGetterDelegate(FederationTrustSchema.RawOrgNextCertificate), FederationTrustSchema.CertificateSetterDelegate(FederationTrustSchema.RawOrgNextCertificate), null, null);

		public static readonly ADPropertyDefinition OrgNextPrivCertificate = new ADPropertyDefinition("OrgNextPrivCertificate", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchFedOrgNextPrivCertificate", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 256)
		}, null, null);

		public static readonly ADPropertyDefinition OrgPrevPrivCertificate = new ADPropertyDefinition("OrgPrevPrivCertificate", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchFedOrgPrevPrivCertificate", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 256)
		}, null, null);

		public static readonly ADPropertyDefinition RawOrgPrevCertificate = new ADPropertyDefinition("RawOrgPrevCertificate", ExchangeObjectVersion.Exchange2010, typeof(byte[]), "msExchFedOrgPrevCertificate", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OrgPrevCertificate = new ADPropertyDefinition("OrgPrevCertificate", ExchangeObjectVersion.Exchange2010, typeof(X509Certificate2), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FederationTrustSchema.RawOrgPrevCertificate
		}, null, FederationTrustSchema.CertificateGetterDelegate(FederationTrustSchema.RawOrgPrevCertificate), FederationTrustSchema.CertificateSetterDelegate(FederationTrustSchema.RawOrgPrevCertificate), null, null);

		public static readonly ADPropertyDefinition RawTokenIssuerCertificate = new ADPropertyDefinition("RawTokenIssuerCertificate", ExchangeObjectVersion.Exchange2010, typeof(byte[]), "msExchFedTokenIssuerCertificate", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TokenIssuerCertificate = new ADPropertyDefinition("TokenIssuerCertificate", ExchangeObjectVersion.Exchange2010, typeof(X509Certificate2), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FederationTrustSchema.RawTokenIssuerCertificate
		}, null, FederationTrustSchema.CertificateGetterDelegate(FederationTrustSchema.RawTokenIssuerCertificate), FederationTrustSchema.CertificateSetterDelegate(FederationTrustSchema.RawTokenIssuerCertificate), null, null);

		public static readonly ADPropertyDefinition RawTokenIssuerPrevCertificate = new ADPropertyDefinition("RawTokenIssuerPrevCertificate", ExchangeObjectVersion.Exchange2010, typeof(byte[]), "msExchFedTokenIssuerPrevCertificate", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TokenIssuerPrevCertificate = new ADPropertyDefinition("TokenIssuerPrevCertificate", ExchangeObjectVersion.Exchange2010, typeof(X509Certificate2), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FederationTrustSchema.RawTokenIssuerPrevCertificate
		}, null, FederationTrustSchema.CertificateGetterDelegate(FederationTrustSchema.RawTokenIssuerPrevCertificate), FederationTrustSchema.CertificateSetterDelegate(FederationTrustSchema.RawTokenIssuerPrevCertificate), null, null);

		public static readonly ADPropertyDefinition TokenIssuerCertReference = new ADPropertyDefinition("TokenIssuerCertReference", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchFedTokenIssuerCertReference", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TokenIssuerPrevCertReference = new ADPropertyDefinition("TokenIssuerPrevCertReference", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchFedTokenIssuerPrevCertReference", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition NamespaceProvisioner = new ADPropertyDefinition("NamespaceProvisioner", ExchangeObjectVersion.Exchange2010, typeof(FederationTrust.NamespaceProvisionerType), null, ADPropertyDefinitionFlags.Calculated, FederationTrust.NamespaceProvisionerType.LiveDomainServices, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FederationTrustSchema.RawAdminDescription
		}, null, new GetterDelegate(FederationTrustSchema.NamespaceProvisionerTypeGetter), new SetterDelegate(FederationTrustSchema.NamespaceProvisionerTypeSetter), null, null);

		private static string[] namespaceProvisionerProgIds = new string[]
		{
			"WINDOWSLIVEDOMAINSERVICES",
			"WINDOWSLIVEDOMAINSERVICES2",
			"EXTERNALPROCESS"
		};

		private static string[] partnerSTSTypeProgIds = new string[]
		{
			"WINDOWSLIVEID"
		};
	}
}
