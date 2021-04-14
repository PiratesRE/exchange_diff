using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal class FailedToIssueTokenException : WSTrustException
	{
		public FailedToIssueTokenException(Exception innerException) : base(WSTrustStrings.FailedToIssueToken(innerException), innerException)
		{
		}
	}
}
