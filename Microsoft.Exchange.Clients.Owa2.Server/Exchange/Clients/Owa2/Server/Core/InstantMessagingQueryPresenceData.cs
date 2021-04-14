using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal enum InstantMessagingQueryPresenceData
	{
		[DisplayName("QP.LS")]
		LyncServer,
		[DisplayName("QP.UC")]
		UserContext,
		[DisplayName("QP.UCS")]
		UCSMode,
		[DisplayName("QP.PVC")]
		PrivacyMode,
		[DisplayName("QP.QSIP")]
		QueriedSIPs,
		[DisplayName("QP.SSIP")]
		SkippedSIPs,
		[DisplayName("QP.INSIP")]
		InvalidSIPs,
		[DisplayName("QP.SSIP")]
		SuccessfulSIPs,
		[DisplayName("QP.FSIP")]
		FailedSIPs
	}
}
