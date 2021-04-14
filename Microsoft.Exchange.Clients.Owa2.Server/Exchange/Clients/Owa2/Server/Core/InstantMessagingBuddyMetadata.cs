using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal enum InstantMessagingBuddyMetadata
	{
		[DisplayName("BD.LS")]
		LyncServer,
		[DisplayName("BD.UC")]
		UserContext,
		[DisplayName("BD.UCS")]
		UCSMode,
		[DisplayName("BD.PVC")]
		PrivacyMode,
		[DisplayName("BD.SIP")]
		SIP,
		[DisplayName("BD.CID")]
		ContactId
	}
}
