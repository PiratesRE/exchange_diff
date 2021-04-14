using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum UpdateAndPostModernGroupItemMetadata
	{
		[DisplayName("UAPMGI", "CT")]
		ConversationTopic,
		[DisplayName("UAPMGI", "CI")]
		ConversationId,
		[DisplayName("UAPMGI", "TRC")]
		ToRecipientCount,
		[DisplayName("UAPMGI", "CRC")]
		CcRecipientCount
	}
}
