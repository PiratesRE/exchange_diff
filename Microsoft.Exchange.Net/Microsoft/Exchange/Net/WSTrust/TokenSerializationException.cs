using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal class TokenSerializationException : WSTrustException
	{
		public TokenSerializationException(Exception innerException) : base(WSTrustStrings.FailedToSerializeToken(innerException), innerException)
		{
		}
	}
}
