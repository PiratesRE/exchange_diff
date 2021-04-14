using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IEntryIdTranslator
	{
		byte[] GetSourceFolderIdFromTargetFolderId(byte[] targetFolderId);

		byte[] GetSourceMessageIdFromTargetMessageId(byte[] targetMessageId);
	}
}
