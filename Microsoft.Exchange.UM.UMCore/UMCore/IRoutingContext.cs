using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IRoutingContext
	{
		string CallId { get; }

		SipRoutingHelper RoutingHelper { get; }

		PlatformSipUri RequestUriOfCall { get; }

		bool IsSecuredCall { get; }

		UMDialPlan DialPlan { get; }

		Guid TenantGuid { get; }

		string UMPodRedirectTemplate { get; }
	}
}
