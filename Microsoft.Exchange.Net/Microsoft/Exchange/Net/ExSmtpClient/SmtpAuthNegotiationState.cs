using System;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	internal enum SmtpAuthNegotiationState
	{
		UnAuthenticated,
		AuthenticationInProgressNoUserOrPasswordGiven,
		AuthenticationInProgressUserGiven,
		AuthenticationInProgressPasswordGiven,
		Authenticated
	}
}
