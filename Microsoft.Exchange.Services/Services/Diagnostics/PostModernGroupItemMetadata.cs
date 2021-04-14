using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum PostModernGroupItemMetadata
	{
		[DisplayName("PMGI", "CT")]
		ConversationTopic,
		[DisplayName("PMGI", "CI")]
		ConversationId,
		[DisplayName("PMGI", "TRC")]
		ToRecipientCount,
		[DisplayName("PMGI", "CRC")]
		CcRecipientCount,
		[DisplayName("PMGI", "IR")]
		IsReplying,
		[DisplayName("PMGI", "IRUD")]
		IsReplyingUsingDraft,
		[DisplayName("PMGI", "PECL")]
		PreExecuteCommandLatency,
		[DisplayName("PMGI", "MAC")]
		MissedAdCache
	}
}
