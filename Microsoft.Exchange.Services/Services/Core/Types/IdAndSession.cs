using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class IdAndSession
	{
		internal IdAndSession()
		{
		}

		public IdAndSession(StoreId storeId, StoreSession session)
		{
			this.id = storeId;
			this.session = session;
		}

		public IdAndSession(StoreId storeId, StoreSession session, IList<AttachmentId> attachmentIds) : this(storeId, session)
		{
			this.attachmentIds.AddRange(attachmentIds);
		}

		public IdAndSession(StoreId storeId, StoreId parentFolderId, StoreSession session) : this(storeId, session)
		{
			this.parentFolderId = parentFolderId;
		}

		public IdAndSession(StoreId storeId, StoreId parentFolderId, StoreSession session, IList<AttachmentId> attachmentIds) : this(storeId, session, attachmentIds)
		{
			this.parentFolderId = parentFolderId;
		}

		public static IdAndSession CreateFromItem(Item item)
		{
			IdAndSession result = null;
			if (item.Session is MailboxSession)
			{
				result = new IdAndSession(item.Id, item.Session);
			}
			else if (item.Session is PublicFolderSession)
			{
				result = new IdAndSession(item.Id, item.ParentId, item.Session);
			}
			return result;
		}

		public static IdAndSession CreateFromFolder(Folder folder)
		{
			return new IdAndSession(folder.Id, folder.Session);
		}

		public IdAndSession Clone()
		{
			return new IdAndSession(this.Id, this.Session, this.AttachmentIds)
			{
				parentFolderId = this.parentFolderId
			};
		}

		public Item GetRootXsoItem(PropertyDefinition[] propertyDefinitions)
		{
			return ServiceCommandBase.GetXsoItem(this.Session, this.Id, propertyDefinitions);
		}

		public AggregateOperationResult DeleteRootXsoItem(StoreObjectId parentStoreObjectId, DeleteItemFlags deleteItemFlags)
		{
			return this.DeleteRootXsoItem(parentStoreObjectId, deleteItemFlags, false);
		}

		public AggregateOperationResult DeleteRootXsoItem(StoreObjectId parentStoreObjectId, DeleteItemFlags deleteItemFlags, bool returnNewItemIds)
		{
			MailboxSession mailboxSession = this.Session as MailboxSession;
			if (mailboxSession != null && mailboxSession.IsDefaultFolderType(parentStoreObjectId) == DefaultFolderType.JunkEmail)
			{
				deleteItemFlags |= DeleteItemFlags.SuppressReadReceipt;
			}
			CallContext callContext = CallContext.Current;
			if (callContext != null && (callContext.WorkloadType == WorkloadType.Owa || callContext.WorkloadType == WorkloadType.OwaVoice) && mailboxSession != null && mailboxSession.LogonType == LogonType.Delegated && (deleteItemFlags & DeleteItemFlags.MoveToDeletedItems) == DeleteItemFlags.MoveToDeletedItems && this.GetAsStoreObjectId().ObjectType == StoreObjectType.CalendarItem)
			{
				MailboxSession mailboxIdentityMailboxSession = CallContext.Current.SessionCache.GetMailboxIdentityMailboxSession();
				StoreObjectId defaultFolderId = mailboxIdentityMailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems);
				return mailboxSession.Move(mailboxIdentityMailboxSession, defaultFolderId, new DeleteItemFlags?(deleteItemFlags), new StoreId[]
				{
					this.Id
				});
			}
			return this.Session.Delete(deleteItemFlags, returnNewItemIds, new StoreId[]
			{
				this.Id
			});
		}

		public StoreId Id
		{
			get
			{
				return this.id;
			}
		}

		public StoreId ParentFolderId
		{
			get
			{
				return this.parentFolderId;
			}
		}

		public StoreSession Session
		{
			get
			{
				return this.session;
			}
		}

		public List<AttachmentId> AttachmentIds
		{
			get
			{
				return this.attachmentIds;
			}
		}

		public ConcatenatedIdAndChangeKey GetConcatenatedId()
		{
			return IdConverter.GetConcatenatedId(this.Id, this, this.AttachmentIds);
		}

		public StoreObjectId GetAsStoreObjectId()
		{
			return IdConverter.GetAsStoreObjectId(this.id);
		}

		private StoreId id;

		private StoreId parentFolderId;

		private StoreSession session;

		private List<AttachmentId> attachmentIds = new List<AttachmentId>();
	}
}
