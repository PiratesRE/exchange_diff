using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal class TokenDecryptionException : WSTrustException
	{
		public TokenDecryptionException() : base(WSTrustStrings.CannotDecryptToken)
		{
		}
	}
}
