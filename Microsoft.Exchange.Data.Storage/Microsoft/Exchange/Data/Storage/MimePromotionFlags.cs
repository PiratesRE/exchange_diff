using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum MimePromotionFlags
	{
		Default = 0,
		SkipMessageHeaders = 1,
		SkipMessageBody = 2,
		SkipRegularAttachments = 4,
		SkipInlineAttachments = 8,
		SkipAllAttachments = 12,
		AllFlags = 15,
		PromoteHeadersOnly = 14,
		PromoteBodyOnly = 13,
		PromoteAttachmentsOnly = 3,
		PromoteInlineAttachmentsOnly = 7
	}
}
