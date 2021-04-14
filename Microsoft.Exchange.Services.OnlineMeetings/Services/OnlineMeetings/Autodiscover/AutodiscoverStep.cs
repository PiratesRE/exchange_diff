using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.Autodiscover
{
	internal enum AutodiscoverStep
	{
		SipDomain,
		OAuth,
		AnonymousAutodiscover,
		AuthenticatedAutodiscover
	}
}
