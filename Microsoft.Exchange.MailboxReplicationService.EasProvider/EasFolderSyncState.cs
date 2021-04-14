using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[XmlType(TypeName = "EasFolderSyncState")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	public sealed class EasFolderSyncState : XMLSerializableBase
	{
		public EasFolderSyncState()
		{
			this.CrawlerDeletions = new List<string>(512);
		}

		[XmlElement(ElementName = "SyncKey")]
		public string SyncKey { get; set; }

		[XmlElement(ElementName = "ChangesSynced")]
		public DateTime? ChangesSynced { get; set; }

		[XmlElement(ElementName = "CrawlerSyncKey")]
		public string CrawlerSyncKey { get; set; }

		[XmlArray(ElementName = "CrawlerDeletions")]
		public List<string> CrawlerDeletions { get; set; }

		internal static EasFolderSyncState Deserialize(byte[] data)
		{
			string @string = Encoding.UTF7.GetString(data);
			return XMLSerializableBase.Deserialize<EasFolderSyncState>(@string, true);
		}

		internal byte[] Serialize()
		{
			string s = base.Serialize(false);
			return Encoding.UTF7.GetBytes(s);
		}
	}
}
