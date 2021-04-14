using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal enum SynchronizeWacAttachmentMetadata
	{
		[DisplayName("SWA", "R")]
		Result,
		[DisplayName("SWA", "C")]
		Count,
		[DisplayName("SWA", "ESID")]
		SessionId
	}
}
