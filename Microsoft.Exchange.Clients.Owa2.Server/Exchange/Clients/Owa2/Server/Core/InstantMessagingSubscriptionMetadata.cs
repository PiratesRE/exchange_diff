using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal enum InstantMessagingSubscriptionMetadata
	{
		[DisplayName("IMSUB.LS")]
		LyncServer,
		[DisplayName("IMSUB.UC")]
		UserContext,
		[DisplayName("IMSUB.UCS")]
		UCSMode,
		[DisplayName("IMSUB.PVC")]
		PrivacyMode,
		[DisplayName("IMSUB.SUBSIP")]
		SubscribedSIPs,
		[DisplayName("IMSUB.UNSIP")]
		UnSubscribedSIP
	}
}
