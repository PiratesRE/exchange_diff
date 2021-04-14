using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("DeletePolicy")]
	internal sealed class DeletePolicyEventHandler : PolicyEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(DeletePolicyEventHandler));
		}

		public DeletePolicyEventHandler() : base(PolicyProvider.DeletePolicyProvider)
		{
		}

		protected override PolicyContextMenuBase InternalGetPolicyMenu(ref OwaStoreObjectId itemId)
		{
			if (itemId != null)
			{
				if (itemId.OwaStoreObjectIdType != OwaStoreObjectIdType.MailBoxObject && itemId.OwaStoreObjectIdType != OwaStoreObjectIdType.ArchiveMailboxObject && itemId.OwaStoreObjectIdType != OwaStoreObjectIdType.Conversation && itemId.OwaStoreObjectIdType != OwaStoreObjectIdType.ArchiveConversation)
				{
					throw new OwaInvalidRequestException("Only (archive)mailbox and (archive)conversation objects can be handled.");
				}
				if (itemId.IsConversationId)
				{
					OwaStoreObjectId[] allItemIds = ConversationUtilities.GetAllItemIds(base.UserContext, new OwaStoreObjectId[]
					{
						itemId
					}, new StoreObjectId[0]);
					if (allItemIds.Length == 1)
					{
						itemId = allItemIds[0];
					}
				}
			}
			DeletePolicyContextMenu deletePolicyContextMenu = DeletePolicyContextMenu.Create(base.UserContext);
			if (itemId != null && !itemId.IsConversationId && itemId.IsStoreObjectId && itemId.StoreObjectId.IsFolderId)
			{
				StoreObjectId storeObjectId = itemId.StoreObjectId;
				StoreSession session = itemId.GetSession(base.UserContext);
				if (Utilities.IsDefaultFolderId(session, storeObjectId, DefaultFolderType.Inbox) || Utilities.IsDefaultFolderId(session, storeObjectId, DefaultFolderType.Drafts) || Utilities.IsDefaultFolderId(session, storeObjectId, DefaultFolderType.SentItems) || Utilities.IsDefaultFolderId(session, storeObjectId, DefaultFolderType.Notes) || Utilities.IsDefaultFolderId(session, storeObjectId, DefaultFolderType.JunkEmail) || Utilities.IsDefaultFolderId(session, storeObjectId, DefaultFolderType.DeletedItems))
				{
					deletePolicyContextMenu.RenderCheckedOnly = true;
				}
			}
			return deletePolicyContextMenu;
		}

		public const string EventNamespace = "DeletePolicy";
	}
}
