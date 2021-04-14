using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.SharedMailboxSentItems
{
	internal sealed class StoreOperations : IStoreOperations
	{
		public StoreOperations(MailboxSession session, ILogger logger)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			ArgumentValidator.ThrowIfNull("logger", logger);
			this.session = session;
			this.logger = logger;
		}

		public bool MessageExistsInSentItems(string internetMessageId)
		{
			using (Folder folder = Folder.Bind(this.session, DefaultFolderType.SentItems))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, StoreOperations.SortByInternetMessageId, new PropertyDefinition[]
				{
					ItemSchema.InternetMessageId
				}))
				{
					queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.InternetMessageId, internetMessageId));
					object[][] rows = queryResult.GetRows(1);
					if (rows != null && rows.Length > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void CopyAttachmentToSentItemsFolder(MessageItem attachedMessageItem)
		{
			StoreId destinationSentItemsFolderId = this.CreateOrGetSentItemsFolderId();
			this.CopyAttachmentToDestinationFolder(destinationSentItemsFolderId, attachedMessageItem);
		}

		private void CopyAttachmentToDestinationFolder(StoreId destinationSentItemsFolderId, MessageItem attachedMessageItem)
		{
			using (MessageItem messageItem = MessageItem.Create(this.session, destinationSentItemsFolderId))
			{
				Item.CopyItemContent(attachedMessageItem, messageItem);
				messageItem.PropertyBag[MessageItemSchema.Flags] = (MessageFlags.IsRead | MessageFlags.HasBeenSubmitted | MessageFlags.IsFromMe);
				messageItem.Save(SaveMode.NoConflictResolution);
			}
		}

		private StoreId CreateOrGetSentItemsFolderId()
		{
			StoreId storeId = this.session.GetDefaultFolderId(DefaultFolderType.SentItems);
			if (storeId == null)
			{
				this.logger.TraceDebug(new string[]
				{
					"SharedMailboxSentItemsAgent.OnPromotedMessageHandler: sent items folder Id does not exists, Create the default folder."
				});
				storeId = this.session.CreateDefaultFolder(DefaultFolderType.SentItems);
			}
			return storeId;
		}

		private static readonly SortBy[] SortByInternetMessageId = new SortBy[]
		{
			new SortBy(ItemSchema.InternetMessageId, SortOrder.Ascending)
		};

		private readonly MailboxSession session;

		private readonly ILogger logger;
	}
}
