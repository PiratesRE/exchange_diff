using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class OpenIdConnectJsonMetadataDocument
	{
		public string token_endpoint;

		public string check_session_iframe;

		public string[] scopes_supported;

		public string[] response_modes_supported;

		public bool microsoft_multi_refresh_token;

		public string authorization_endpoint;

		public string userinfo_endpoint;

		public string[] token_endpoint_auth_methods_supported;

		public string jwks_uri;

		public string[] id_token_signing_alg_values_supported;

		public string end_session_endpoint;

		public string issuer;

		public string[] subject_types_supported;

		public string[] response_types_supported;
	}
}
