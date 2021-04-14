using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class AuthMetadataConstants
	{
		public static readonly string Protocol = "OAuth2";

		public static readonly string MetadataEndpointUsage = "metadata";

		public static readonly string IssuingEndpointUsage = "issuance";

		public static readonly string KeyUsage = "signing";

		public static readonly string OpenIdConnectSigningKeyUsage = "sig";

		public static readonly string SigningKeyType = "x509Certificate";

		public static readonly string AzureADTenantIdTemplate = "{tenantid}";

		public static readonly string SelfIssuingAuthorityMetadataVersion = "Exchange/1.0";
	}
}
