using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class MiniVirtualDirectorySchema : ADObjectSchema
	{
		public static readonly ADPropertyDefinition Server = ADVirtualDirectorySchema.Server;

		public static readonly ADPropertyDefinition ExternalUrl = ADVirtualDirectorySchema.ExternalUrl;

		public static readonly ADPropertyDefinition ExternalAuthenticationMethods = ADVirtualDirectorySchema.ExternalAuthenticationMethods;

		public static readonly ADPropertyDefinition ExternalAuthenticationMethodFlags = ADVirtualDirectorySchema.ExternalAuthenticationMethodFlags;

		public static readonly ADPropertyDefinition InternalUrl = ADVirtualDirectorySchema.InternalUrl;

		public static readonly ADPropertyDefinition InternalAuthenticationMethods = ADVirtualDirectorySchema.InternalAuthenticationMethods;

		public static readonly ADPropertyDefinition InternalAuthenticationMethodFlags = ADVirtualDirectorySchema.InternalAuthenticationMethodFlags;

		public static readonly ADPropertyDefinition MetabasePath = ExchangeVirtualDirectorySchema.MetabasePath;

		public static readonly ADPropertyDefinition LiveIdAuthentication = ExchangeWebAppVirtualDirectorySchema.LiveIdAuthentication;

		public static readonly ADPropertyDefinition AvailabilityForeignConnectorType = ADAvailabilityForeignConnectorVirtualDirectorySchema.AvailabilityForeignConnectorType;

		public static readonly ADPropertyDefinition AvailabilityForeignConnectorDomains = ADAvailabilityForeignConnectorVirtualDirectorySchema.AvailabilityForeignConnectorDomains;

		public static readonly ADPropertyDefinition ADFeatureSet = ADEcpVirtualDirectorySchema.ADFeatureSet;

		public static readonly ADPropertyDefinition AdminEnabled = ADEcpVirtualDirectorySchema.AdminEnabled;

		public static readonly ADPropertyDefinition OwaOptionsEnabled = ADEcpVirtualDirectorySchema.OwaOptionsEnabled;

		public static readonly ADPropertyDefinition MobileClientCertificateProvisioningEnabled = ADMobileVirtualDirectorySchema.MobileClientCertificateProvisioningEnabled;

		public static readonly ADPropertyDefinition MobileClientCertificateAuthorityURL = ADMobileVirtualDirectorySchema.MobileClientCertificateAuthorityURL;

		public static readonly ADPropertyDefinition MobileClientCertTemplateName = ADMobileVirtualDirectorySchema.MobileClientCertTemplateName;

		public static readonly ADPropertyDefinition OfflineAddressBooks = ADOabVirtualDirectorySchema.OfflineAddressBooks;

		public static readonly ADPropertyDefinition OwaVersion = ADOwaVirtualDirectorySchema.OwaVersion;

		public static readonly ADPropertyDefinition AnonymousFeaturesEnabled = ADOwaVirtualDirectorySchema.AnonymousFeaturesEnabled;

		public static readonly ADPropertyDefinition FailbackUrl = ADOwaVirtualDirectorySchema.FailbackUrl;

		public static readonly ADPropertyDefinition IntegratedFeaturesEnabled = ADOwaVirtualDirectorySchema.IntegratedFeaturesEnabled;

		public static readonly ADPropertyDefinition IISAuthenticationMethods = ADRpcHttpVirtualDirectorySchema.IISAuthenticationMethods;

		public static readonly ADPropertyDefinition ExternalClientAuthenticationMethod = ADRpcHttpVirtualDirectorySchema.ExternalClientAuthenticationMethod;

		public static readonly ADPropertyDefinition XropUrl = ADRpcHttpVirtualDirectorySchema.XropUrl;

		public static readonly ADPropertyDefinition InternalNLBBypassUrl = ADWebServicesVirtualDirectorySchema.InternalNLBBypassUrl;

		public static readonly ADPropertyDefinition MRSProxyEnabled = ADWebServicesVirtualDirectorySchema.MRSProxyEnabled;
	}
}
