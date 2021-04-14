using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailboxFolder : NodeInfo, IComparable
	{
		[DataMember]
		public List<MailboxFolder> Children
		{
			get
			{
				return this.children;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public MailboxFolder(MailboxFolder folder)
		{
			this.Folder = folder;
			base.ID = folder.FolderStoreObjectId;
			base.Name = folder.Name;
			this.IsInboxFolder = (this.Folder.DefaultFolderType == DefaultFolderType.Inbox);
			base.CanNewSubNode = (this.Folder.DefaultFolderType != DefaultFolderType.ElcRoot && this.Folder.ExtendedFolderFlags != ExtendedFolderFlags.ExclusivelyBound && this.Folder.ExtendedFolderFlags != ExtendedFolderFlags.RemoteHierarchy);
		}

		[DataMember]
		public bool IsInboxFolder { get; internal set; }

		internal MailboxFolder Folder { get; private set; }

		public int CompareTo(object obj)
		{
			MailboxFolder mailboxFolder = obj as MailboxFolder;
			int num = 5;
			int num2 = 5;
			if (MailboxFolder.FoldersToShowFirst.ContainsKey(this.Folder.DefaultFolderType))
			{
				MailboxFolder.FoldersToShowFirst.TryGetValue(this.Folder.DefaultFolderType, out num);
			}
			if (MailboxFolder.FoldersToShowFirst.ContainsKey(mailboxFolder.Folder.DefaultFolderType))
			{
				MailboxFolder.FoldersToShowFirst.TryGetValue(mailboxFolder.Folder.DefaultFolderType, out num2);
			}
			if (num == num2)
			{
				return this.Folder.Name.CompareTo(mailboxFolder.Folder.Name);
			}
			if (num < num2)
			{
				return -1;
			}
			return 1;
		}

		private List<MailboxFolder> children = new List<MailboxFolder>();

		private static readonly Dictionary<DefaultFolderType?, int> FoldersToShowFirst = new Dictionary<DefaultFolderType?, int>
		{
			{
				new DefaultFolderType?(DefaultFolderType.Inbox),
				1
			},
			{
				new DefaultFolderType?(DefaultFolderType.Drafts),
				2
			},
			{
				new DefaultFolderType?(DefaultFolderType.SentItems),
				3
			},
			{
				new DefaultFolderType?(DefaultFolderType.DeletedItems),
				4
			}
		};
	}
}
