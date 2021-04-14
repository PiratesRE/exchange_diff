using System;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	internal enum AuthenticationState
	{
		UnInitialized,
		Initialized,
		Negotiating,
		Secured
	}
}
