using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.SharedMailboxSentItems
{
	internal interface IStoreOperations
	{
		bool MessageExistsInSentItems(string internetMessageId);

		void CopyAttachmentToSentItemsFolder(MessageItem attachedMessageItem);
	}
}
