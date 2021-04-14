using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class OneTimeSearch : DisposableBase
	{
		private OneTimeSearch()
		{
		}

		private OneTimeSearch(UMSubscriber user, StoreObjectId folderId, int itemCount)
		{
			this.user = user;
			this.folderId = folderId;
			this.itemCount = itemCount;
		}

		internal StoreObjectId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		internal int ItemCount
		{
			get
			{
				return this.itemCount;
			}
		}

		internal static OneTimeSearch Execute(UMSubscriber user, MailboxSession session, StoreId fid, QueryFilter filter)
		{
			string displayName = "EXUM-23479-" + Guid.NewGuid().ToString();
			OneTimeSearch result;
			using (SearchFolder searchFolder = SearchFolder.Create(session, session.GetDefaultFolderId(DefaultFolderType.SearchFolders), displayName, CreateMode.OpenIfExists))
			{
				searchFolder.Save();
				searchFolder.Load();
				IAsyncResult asyncResult = searchFolder.BeginApplyOneTimeSearch(new SearchFolderCriteria(filter, new StoreId[]
				{
					fid
				})
				{
					DeepTraversal = false
				}, null, null);
				searchFolder.EndApplyOneTimeSearch(asyncResult);
				searchFolder.Save();
				searchFolder.Load(new PropertyDefinition[]
				{
					FolderSchema.ItemCount
				});
				result = new OneTimeSearch(user, searchFolder.Id.ObjectId, searchFolder.ItemCount);
			}
			return result;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.folderId != null)
			{
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
				{
					mailboxSessionLock.Session.Delete(DeleteItemFlags.HardDelete, new StoreId[]
					{
						this.folderId
					});
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OneTimeSearch>(this);
		}

		private const string UmPrefix = "EXUM-23479-";

		private StoreObjectId folderId;

		private int itemCount;

		private UMSubscriber user;
	}
}
