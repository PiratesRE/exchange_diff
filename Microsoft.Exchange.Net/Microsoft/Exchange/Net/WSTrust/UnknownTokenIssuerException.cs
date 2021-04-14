using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal class UnknownTokenIssuerException : WSTrustException
	{
		public UnknownTokenIssuerException(string federatedTokenIssuerUri, string targetTokenIssuerUri) : base(WSTrustStrings.UnknownTokenIssuerException(federatedTokenIssuerUri, targetTokenIssuerUri))
		{
		}
	}
}
