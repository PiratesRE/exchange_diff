using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class MoveFolderInfo : XMLSerializableBase, IEquatable<MoveFolderInfo>
	{
		private MoveFolderInfo()
		{
		}

		public MoveFolderInfo(string entryId, bool isFinalized)
		{
			this.EntryId = entryId;
			this.IsFinalized = isFinalized;
		}

		[XmlElement(ElementName = "EntryId")]
		public string EntryId { get; set; }

		[XmlElement(ElementName = "IsFinalized")]
		public bool IsFinalized { get; set; }

		public bool Equals(MoveFolderInfo folder)
		{
			return this.EntryId == folder.EntryId;
		}
	}
}
