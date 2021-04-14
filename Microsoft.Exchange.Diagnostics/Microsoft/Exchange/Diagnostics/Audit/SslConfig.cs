using System;

namespace Microsoft.Exchange.Diagnostics.Audit
{
	public sealed class SslConfig
	{
		public bool AllowInternalUntrustedCerts { get; set; }

		public bool AllowExternalUntrustedCerts { get; set; }
	}
}
