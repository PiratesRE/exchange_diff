using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	public enum GetAttachmentMetadata
	{
		[DisplayName("GA", "X")]
		Extension,
		[DisplayName("GA", "L")]
		Length,
		[DisplayName("GA", "U")]
		Updated
	}
}
