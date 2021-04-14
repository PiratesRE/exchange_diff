using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal enum RedirectionOptions
	{
		FollowUntilNo302,
		StopOnFirstCrossDomainRedirect,
		FollowUntilNo302OrSpecificRedirection,
		FollowUntilNo302ExpectCrossDomainOnFirstRedirect
	}
}
