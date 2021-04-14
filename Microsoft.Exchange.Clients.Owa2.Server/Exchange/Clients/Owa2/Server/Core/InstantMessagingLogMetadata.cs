using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal enum InstantMessagingLogMetadata
	{
		[DisplayName("IMC", "EC")]
		OperationErrorCode,
		[DisplayName("IMC", "CID")]
		ConversationId
	}
}
