using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class AuthMetadata
	{
		public string ServiceName { get; set; }

		public string Issuer { get; set; }

		public string Realm { get; set; }

		public string IssuingEndpoint { get; set; }

		public string AuthorizationEndpoint { get; set; }

		public string KeysEndpoint { get; set; }

		public string[] CertificateStrings { get; set; }
	}
}
