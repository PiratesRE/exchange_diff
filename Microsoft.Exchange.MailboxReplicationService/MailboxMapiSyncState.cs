using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class MailboxMapiSyncState : XMLSerializableBase
	{
		public MailboxMapiSyncState()
		{
			this.folderSnapshots = new EntryIdMap<FolderStateSnapshot>();
		}

		[XmlArrayItem("FolderSnapshot")]
		[XmlArray("FolderSnapshots")]
		public FolderStateSnapshot[] FolderSnapshots
		{
			get
			{
				FolderStateSnapshot[] array = new FolderStateSnapshot[this.folderSnapshots.Count];
				this.folderSnapshots.Values.CopyTo(array, 0);
				return array;
			}
			set
			{
				this.folderSnapshots.Clear();
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						FolderStateSnapshot folderStateSnapshot = value[i];
						this.folderSnapshots[folderStateSnapshot.FolderId ?? MailboxMapiSyncState.NullFolderKey] = folderStateSnapshot;
					}
				}
			}
		}

		[XmlIgnore]
		public FolderStateSnapshot this[byte[] folderId]
		{
			get
			{
				byte[] key = folderId ?? MailboxMapiSyncState.NullFolderKey;
				FolderStateSnapshot folderStateSnapshot;
				if (!this.folderSnapshots.TryGetValue(key, out folderStateSnapshot))
				{
					folderStateSnapshot = new FolderStateSnapshot();
					folderStateSnapshot.FolderId = folderId;
					this.folderSnapshots[key] = folderStateSnapshot;
				}
				return folderStateSnapshot;
			}
		}

		[XmlElement(ElementName = "ProviderState")]
		public string ProviderState { get; set; }

		public static MailboxMapiSyncState Deserialize(string data)
		{
			return XMLSerializableBase.Deserialize<MailboxMapiSyncState>(data, true);
		}

		private static readonly byte[] NullFolderKey = Array<byte>.Empty;

		private EntryIdMap<FolderStateSnapshot> folderSnapshots;
	}
}
