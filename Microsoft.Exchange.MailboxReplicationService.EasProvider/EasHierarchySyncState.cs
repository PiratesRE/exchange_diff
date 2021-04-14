using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Response.FolderHierarchy;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[XmlType(TypeName = "EasHierarchySyncState")]
	[Serializable]
	public class EasHierarchySyncState : XMLSerializableBase
	{
		public EasHierarchySyncState()
		{
		}

		internal EasHierarchySyncState(Add[] folders, string syncKey)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("folders", folders);
			ArgumentValidator.ThrowIfNullOrEmpty("syncKey", syncKey);
			this.Folders = new List<Add>(folders);
			this.SyncKey = syncKey;
		}

		[XmlElement(ElementName = "Folder")]
		public EasHierarchySyncState.EasFolderData[] FolderData
		{
			get
			{
				if (this.Folders == null || this.Folders.Count == 0)
				{
					return Array<EasHierarchySyncState.EasFolderData>.Empty;
				}
				EasHierarchySyncState.EasFolderData[] array = new EasHierarchySyncState.EasFolderData[this.Folders.Count];
				for (int i = 0; i < this.Folders.Count; i++)
				{
					Add add = this.Folders[i];
					array[i] = new EasHierarchySyncState.EasFolderData
					{
						ServerId = add.ServerId,
						ParentId = add.ParentId,
						DisplayName = add.DisplayName,
						Type = add.Type
					};
				}
				return array;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					this.Folders = new List<Add>(0);
					return;
				}
				List<Add> list = new List<Add>(value.Length);
				for (int i = 0; i < value.Length; i++)
				{
					EasHierarchySyncState.EasFolderData easFolderData = value[i];
					list.Add(new Add
					{
						ServerId = easFolderData.ServerId,
						ParentId = easFolderData.ParentId,
						DisplayName = easFolderData.DisplayName,
						Type = easFolderData.Type
					});
				}
				this.Folders = list;
			}
		}

		[XmlElement(ElementName = "SyncKey")]
		public string SyncKey { get; set; }

		[XmlIgnore]
		public List<Add> Folders { get; private set; }

		internal static EasHierarchySyncState Deserialize(string data)
		{
			return XMLSerializableBase.Deserialize<EasHierarchySyncState>(data, false);
		}

		public sealed class EasFolderData : XMLSerializableBase
		{
			[XmlAttribute(AttributeName = "ServerId")]
			public string ServerId { get; set; }

			[XmlAttribute(AttributeName = "ParentId")]
			public string ParentId { get; set; }

			[XmlText]
			public string DisplayName { get; set; }

			[XmlAttribute(AttributeName = "Type")]
			public int Type { get; set; }
		}
	}
}
