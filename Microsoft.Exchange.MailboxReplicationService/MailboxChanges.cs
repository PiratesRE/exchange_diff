using System;
using System.Linq;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MailboxChanges
	{
		public MailboxChanges(MailboxChangesManifest hierarchyChanges)
		{
			this.hierarchyChanges = hierarchyChanges;
			this.folderChanges = new EntryIdMap<FolderChangesManifest>();
		}

		public MailboxChanges(EntryIdMap<FolderChangesManifest> folderChanges)
		{
			this.hierarchyChanges = new MailboxChangesManifest();
			this.folderChanges = folderChanges;
		}

		public MailboxChangesManifest HierarchyChanges
		{
			get
			{
				return this.hierarchyChanges;
			}
		}

		public EntryIdMap<FolderChangesManifest> FolderChanges
		{
			get
			{
				return this.folderChanges;
			}
		}

		public bool HasFolderRecoverySync
		{
			get
			{
				return this.FolderChanges.Values.Any((FolderChangesManifest fc) => fc.FolderRecoverySync);
			}
		}

		public void GetMessageCounts(out int newMessages, out int updated, out int deleted, out int read, out int unread)
		{
			newMessages = 0;
			updated = 0;
			deleted = 0;
			read = 0;
			unread = 0;
			foreach (FolderChangesManifest folderChangesManifest in this.FolderChanges.Values)
			{
				if (folderChangesManifest.ReadMessages != null)
				{
					read += folderChangesManifest.ReadMessages.Count;
				}
				if (folderChangesManifest.UnreadMessages != null)
				{
					unread += folderChangesManifest.UnreadMessages.Count;
				}
				int num;
				int num2;
				int num3;
				folderChangesManifest.GetMessageCounts(out num, out num2, out num3);
				newMessages += num;
				updated += num2;
				deleted += num3;
			}
		}

		public int MessageChanges
		{
			get
			{
				int num = 0;
				foreach (FolderChangesManifest folderChangesManifest in this.FolderChanges.Values)
				{
					if (folderChangesManifest.ChangedMessages != null)
					{
						num += folderChangesManifest.ChangedMessages.Count;
					}
					if (folderChangesManifest.ReadMessages != null)
					{
						num += folderChangesManifest.ReadMessages.Count;
					}
					if (folderChangesManifest.UnreadMessages != null)
					{
						num += folderChangesManifest.UnreadMessages.Count;
					}
				}
				return num;
			}
		}

		public int EntryCount
		{
			get
			{
				int num = this.MessageChanges;
				if (this.hierarchyChanges.ChangedFolders != null)
				{
					num += this.hierarchyChanges.ChangedFolders.Count;
				}
				if (this.hierarchyChanges.DeletedFolders != null)
				{
					num += this.hierarchyChanges.DeletedFolders.Count;
				}
				return num;
			}
		}

		public bool HasChanges
		{
			get
			{
				foreach (FolderChangesManifest folderChangesManifest in this.folderChanges.Values)
				{
					if (folderChangesManifest.HasChanges)
					{
						return true;
					}
				}
				return (this.hierarchyChanges.ChangedFolders != null && this.hierarchyChanges.ChangedFolders.Count > 0) || (this.hierarchyChanges.DeletedFolders != null && this.hierarchyChanges.DeletedFolders.Count > 0);
			}
		}

		public FolderChangesManifest this[byte[] folderId]
		{
			get
			{
				FolderChangesManifest folderChangesManifest;
				if (!this.folderChanges.TryGetValue(folderId, out folderChangesManifest))
				{
					folderChangesManifest = new FolderChangesManifest(folderId);
					this.folderChanges.Add(folderId, folderChangesManifest);
				}
				return folderChangesManifest;
			}
			set
			{
				this.folderChanges[folderId] = value;
			}
		}

		private MailboxChangesManifest hierarchyChanges;

		private EntryIdMap<FolderChangesManifest> folderChanges;
	}
}
