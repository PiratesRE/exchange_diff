using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class MapiSyncState : XMLSerializableBase
	{
		public MapiSyncState()
		{
			this.hierData = new SyncHierarchyManifestState();
			this.folderData = new EntryIdMap<SyncContentsManifestState>();
		}

		[XmlElement]
		public SyncContentsManifestState[] FolderData
		{
			get
			{
				SyncContentsManifestState[] array = new SyncContentsManifestState[this.folderData.Count];
				this.folderData.Values.CopyTo(array, 0);
				return array;
			}
			set
			{
				this.folderData.Clear();
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						SyncContentsManifestState syncContentsManifestState = value[i];
						this.folderData[syncContentsManifestState.FolderId] = syncContentsManifestState;
					}
				}
			}
		}

		[XmlElement]
		public SyncHierarchyManifestState HierarchyData
		{
			get
			{
				return this.hierData;
			}
			set
			{
				this.hierData = value;
			}
		}

		[XmlIgnore]
		public SyncContentsManifestState this[byte[] folderKey]
		{
			get
			{
				SyncContentsManifestState syncContentsManifestState;
				if (!this.folderData.TryGetValue(folderKey, out syncContentsManifestState))
				{
					syncContentsManifestState = new SyncContentsManifestState();
					syncContentsManifestState.FolderId = folderKey;
					this.folderData.Add(folderKey, syncContentsManifestState);
				}
				return syncContentsManifestState;
			}
		}

		public void RemoveContentsManifestState(byte[] folderId)
		{
			this.folderData.Remove(folderId);
		}

		internal static MapiSyncState Deserialize(string data)
		{
			return XMLSerializableBase.Deserialize<MapiSyncState>(data, false);
		}

		private EntryIdMap<SyncContentsManifestState> folderData;

		private SyncHierarchyManifestState hierData;
	}
}
