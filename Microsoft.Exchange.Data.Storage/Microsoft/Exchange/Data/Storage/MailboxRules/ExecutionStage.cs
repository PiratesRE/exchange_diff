using System;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[Flags]
	internal enum ExecutionStage
	{
		OnPromotedMessage = 1,
		OnCreatedMessage = 2,
		OnDeliveredMessage = 4,
		OnPublicFolderBefore = 8,
		OnPublicFolderAfter = 16
	}
}
