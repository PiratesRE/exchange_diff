using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public class FederationTrust : ADConfigurationObject
	{
		public string ApplicationIdentifier
		{
			get
			{
				return this[FederationTrustSchema.ApplicationIdentifier] as string;
			}
			internal set
			{
				this[FederationTrustSchema.ApplicationIdentifier] = value;
			}
		}

		public Uri ApplicationUri
		{
			get
			{
				return this[FederationTrustSchema.ApplicationUri] as Uri;
			}
			internal set
			{
				this[FederationTrustSchema.ApplicationUri] = value;
			}
		}

		public X509Certificate2 OrgCertificate
		{
			get
			{
				return this[FederationTrustSchema.OrgCertificate] as X509Certificate2;
			}
			internal set
			{
				this[FederationTrustSchema.OrgCertificate] = value;
			}
		}

		public X509Certificate2 OrgNextCertificate
		{
			get
			{
				return this[FederationTrustSchema.OrgNextCertificate] as X509Certificate2;
			}
			internal set
			{
				this[FederationTrustSchema.OrgNextCertificate] = value;
			}
		}

		public X509Certificate2 OrgPrevCertificate
		{
			get
			{
				return this[FederationTrustSchema.OrgPrevCertificate] as X509Certificate2;
			}
			internal set
			{
				this[FederationTrustSchema.OrgPrevCertificate] = value;
			}
		}

		public string OrgPrivCertificate
		{
			get
			{
				return this[FederationTrustSchema.OrgPrivCertificate] as string;
			}
			internal set
			{
				this[FederationTrustSchema.OrgPrivCertificate] = value;
			}
		}

		public string OrgNextPrivCertificate
		{
			get
			{
				return this[FederationTrustSchema.OrgNextPrivCertificate] as string;
			}
			internal set
			{
				this[FederationTrustSchema.OrgNextPrivCertificate] = value;
			}
		}

		public string OrgPrevPrivCertificate
		{
			get
			{
				return this[FederationTrustSchema.OrgPrevPrivCertificate] as string;
			}
			internal set
			{
				this[FederationTrustSchema.OrgPrevPrivCertificate] = value;
			}
		}

		public X509Certificate2 TokenIssuerCertificate
		{
			get
			{
				return this[FederationTrustSchema.TokenIssuerCertificate] as X509Certificate2;
			}
			internal set
			{
				this[FederationTrustSchema.TokenIssuerCertificate] = value;
			}
		}

		public X509Certificate2 TokenIssuerPrevCertificate
		{
			get
			{
				return this[FederationTrustSchema.TokenIssuerPrevCertificate] as X509Certificate2;
			}
			internal set
			{
				this[FederationTrustSchema.TokenIssuerPrevCertificate] = value;
			}
		}

		public string PolicyReferenceUri
		{
			get
			{
				return this[FederationTrustSchema.PolicyReferenceUri] as string;
			}
			internal set
			{
				this[FederationTrustSchema.PolicyReferenceUri] = value;
			}
		}

		public Uri TokenIssuerMetadataEpr
		{
			get
			{
				return this[FederationTrustSchema.TokenIssuerMetadataEpr] as Uri;
			}
			internal set
			{
				this[FederationTrustSchema.TokenIssuerMetadataEpr] = value;
			}
		}

		public EnhancedTimeSpan MetadataPollInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[FederationTrustSchema.MetadataPollInterval];
			}
			internal set
			{
				this[FederationTrustSchema.MetadataPollInterval] = value;
			}
		}

		public FederationTrust.PartnerSTSType TokenIssuerType
		{
			get
			{
				return (FederationTrust.PartnerSTSType)this[FederationTrustSchema.TokenIssuerType];
			}
			internal set
			{
				this[FederationTrustSchema.TokenIssuerType] = value;
			}
		}

		public Uri TokenIssuerUri
		{
			get
			{
				return this[FederationTrustSchema.TokenIssuerUri] as Uri;
			}
			internal set
			{
				this[FederationTrustSchema.TokenIssuerUri] = value;
			}
		}

		public Uri TokenIssuerEpr
		{
			get
			{
				return this[FederationTrustSchema.TokenIssuerEpr] as Uri;
			}
			internal set
			{
				this[FederationTrustSchema.TokenIssuerEpr] = value;
			}
		}

		public Uri WebRequestorRedirectEpr
		{
			get
			{
				return this[FederationTrustSchema.WebRequestorRedirectEpr] as Uri;
			}
			internal set
			{
				this[FederationTrustSchema.WebRequestorRedirectEpr] = value;
			}
		}

		public Uri MetadataEpr
		{
			get
			{
				return this[FederationTrustSchema.MetadataEpr] as Uri;
			}
			internal set
			{
				this[FederationTrustSchema.MetadataEpr] = value;
			}
		}

		public Uri MetadataPutEpr
		{
			get
			{
				return this[FederationTrustSchema.MetadataPutEpr] as Uri;
			}
			internal set
			{
				this[FederationTrustSchema.MetadataPutEpr] = value;
			}
		}

		public string TokenIssuerCertReference
		{
			get
			{
				return this[FederationTrustSchema.TokenIssuerCertReference] as string;
			}
			internal set
			{
				this[FederationTrustSchema.TokenIssuerCertReference] = value;
			}
		}

		public string TokenIssuerPrevCertReference
		{
			get
			{
				return this[FederationTrustSchema.TokenIssuerPrevCertReference] as string;
			}
			internal set
			{
				this[FederationTrustSchema.TokenIssuerPrevCertReference] = value;
			}
		}

		internal string AdministratorProvisioningId
		{
			get
			{
				return this[FederationTrustSchema.AdministratorProvisioningId] as string;
			}
			set
			{
				this[FederationTrustSchema.AdministratorProvisioningId] = value;
			}
		}

		public FederationTrust.NamespaceProvisionerType NamespaceProvisioner
		{
			get
			{
				return (FederationTrust.NamespaceProvisionerType)this[FederationTrustSchema.NamespaceProvisioner];
			}
			internal set
			{
				this[FederationTrustSchema.NamespaceProvisioner] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return FederationTrust.SchemaObject;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchFedTrust";
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return FederationTrust.FederationTrustsContainer;
			}
		}

		public const string ContainerName = "Federation Trusts";

		internal const string TaskNoun = "FederationTrust";

		internal const string LdapName = "msExchFedTrust";

		internal static readonly ADObjectId FederationTrustsContainer = new ADObjectId("CN=Federation Trusts");

		private static readonly FederationTrustSchema SchemaObject = ObjectSchema.GetInstance<FederationTrustSchema>();

		public enum PartnerSTSType
		{
			LiveId
		}

		public enum NamespaceProvisionerType
		{
			LiveDomainServices,
			LiveDomainServices2,
			ExternalProcess
		}
	}
}
