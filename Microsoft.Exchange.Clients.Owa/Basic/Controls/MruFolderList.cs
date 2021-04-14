using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class MruFolderList
	{
		public MruFolderList(UserContext userContext)
		{
			PropertyDefinition[] propsToReturn = new PropertyDefinition[]
			{
				FolderSchema.DisplayName,
				FolderSchema.ItemCount,
				FolderSchema.UnreadCount,
				FolderSchema.ExtendedFolderFlags
			};
			FolderMruCache cacheInstance = FolderMruCache.GetCacheInstance(userContext);
			if (cacheInstance != null)
			{
				cacheInstance.Sort();
				int num = 0;
				while (num < cacheInstance.CacheLength && num < this.folderItems.Length)
				{
					StoreObjectId folderId = cacheInstance.CacheEntries[num].FolderId;
					Folder folder = null;
					try
					{
						folder = Folder.Bind(userContext.MailboxSession, folderId, propsToReturn);
					}
					catch (ObjectNotFoundException)
					{
						FolderMruCache.DeleteFromCache(folderId, userContext);
						cacheInstance = FolderMruCache.GetCacheInstance(userContext);
						cacheInstance.Sort();
						int entryIndexByFolderId = cacheInstance.GetEntryIndexByFolderId(folderId);
						if (entryIndexByFolderId != -1)
						{
							num++;
						}
						continue;
					}
					int i;
					for (i = 0; i < this.folderItemCount; i++)
					{
						if (string.CompareOrdinal(folder.DisplayName, this.folderItems[i].DisplayName) < 0)
						{
							for (int j = this.folderItemCount - 1; j >= i; j--)
							{
								if (j + 1 < this.folderItems.Length)
								{
									this.folderItems[j + 1] = this.folderItems[j];
								}
							}
							break;
						}
					}
					this.folderItems[i] = new MruFolderItem(folderId, folder.DisplayName, folder.ItemCount, (int)folder.TryGetProperty(FolderSchema.UnreadCount), folder.TryGetProperty(FolderSchema.ExtendedFolderFlags));
					this.folderItemCount++;
					folder.Dispose();
					num++;
				}
			}
		}

		public int Count
		{
			get
			{
				return this.folderItemCount;
			}
		}

		public MruFolderItem this[int i]
		{
			get
			{
				if (i < 0 || i >= this.Count)
				{
					throw new ArgumentOutOfRangeException("i", "Indexer of MruFolderList");
				}
				return this.folderItems[i];
			}
		}

		public const int MaxMruFolderNum = 10;

		private MruFolderItem[] folderItems = new MruFolderItem[10];

		private int folderItemCount;
	}
}
