using System;

namespace Microsoft.Exchange.BITS
{
	internal struct BG_AUTH_CREDENTIALS
	{
		public BG_AUTH_TARGET Target;

		public BG_AUTH_SCHEME Scheme;

		public BG_AUTH_CREDENTIALS_UNION Credentials;
	}
}
