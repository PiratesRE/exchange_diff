using System;
using System.Net.Security;

namespace Microsoft.Exchange.Diagnostics.Audit
{
	public sealed class SslError
	{
		public SslError()
		{
		}

		public SslError(SslPolicyErrors sslPolicyErrors, bool allowInternalUntrustedCerts, bool allowExternalUntrustedCerts)
		{
			this.Timestamp = DateTime.UtcNow;
			this.SslPolicyErrors = sslPolicyErrors;
			this.SslConfig = new SslConfig
			{
				AllowInternalUntrustedCerts = allowInternalUntrustedCerts,
				AllowExternalUntrustedCerts = allowExternalUntrustedCerts
			};
		}

		public long Index { get; set; }

		public DateTime Timestamp { get; set; }

		public SslPolicyErrors SslPolicyErrors { get; set; }

		public SslConfig SslConfig { get; set; }
	}
}
