using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal class RequestSecurityTokenResponseException : WSTrustException
	{
		public RequestSecurityTokenResponseException() : base(WSTrustStrings.MalformedRequestSecurityTokenResponse)
		{
		}

		public RequestSecurityTokenResponseException(Exception innerException) : base(WSTrustStrings.MalformedRequestSecurityTokenResponse, innerException)
		{
		}
	}
}
