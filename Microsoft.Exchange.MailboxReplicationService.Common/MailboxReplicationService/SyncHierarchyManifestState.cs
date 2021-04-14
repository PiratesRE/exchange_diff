using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class SyncHierarchyManifestState : XMLSerializableBase
	{
		public SyncHierarchyManifestState()
		{
			this.IdsetGiven = Array<byte>.Empty;
			this.CnsetSeen = Array<byte>.Empty;
		}

		[XmlElement(ElementName = "IdsetGiven")]
		public byte[] IdsetGiven { get; set; }

		[XmlElement(ElementName = "CnsetSeen")]
		public byte[] CnsetSeen { get; set; }

		[XmlElement(ElementName = "ProviderSyncState")]
		public string ProviderSyncState { get; set; }

		[XmlElement(ElementName = "ManualSyncData")]
		public SyncHierarchyManifestState.FolderData[] ManualSyncData { get; set; }

		public sealed class FolderData : XMLSerializableBase
		{
			public FolderData()
			{
			}

			internal FolderData(FolderRec fRec)
			{
				this.EntryId = fRec.EntryId;
				this.ParentId = fRec.ParentId;
				this.LastModifyTimestamp = fRec.LastModifyTimestamp;
			}

			[XmlElement(ElementName = "EntryId")]
			public byte[] EntryId { get; set; }

			[XmlElement(ElementName = "ParentId")]
			public byte[] ParentId { get; set; }

			[XmlElement(ElementName = "LastModifyTimestamp")]
			public DateTime LastModifyTimestamp { get; set; }
		}
	}
}
