using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class MailboxSizeRec : XMLSerializableBase
	{
		public MailboxSizeRec()
		{
		}

		internal MailboxSizeRec(MailboxInformation mailboxInfo)
		{
			this.ItemCount = mailboxInfo.RegularItemCount;
			this.ItemSize = mailboxInfo.RegularItemsSize;
			this.FAIItemCount = mailboxInfo.AssociatedItemCount;
			this.FAIItemSize = mailboxInfo.AssociatedItemsSize;
			this.DeletedItemCount = mailboxInfo.RegularDeletedItemCount;
			this.DeletedItemSize = mailboxInfo.RegularDeletedItemsSize;
			this.DeletedFAIItemCount = mailboxInfo.AssociatedDeletedItemCount;
			this.DeletedFAIItemSize = mailboxInfo.AssociatedDeletedItemsSize;
		}

		[XmlElement(ElementName = "ItemCount")]
		public ulong ItemCount { get; set; }

		[XmlElement(ElementName = "ItemSize")]
		public ulong ItemSize { get; set; }

		[XmlElement(ElementName = "FAIItemCount")]
		public ulong FAIItemCount { get; set; }

		[XmlElement(ElementName = "FAIItemSize")]
		public ulong FAIItemSize { get; set; }

		[XmlElement(ElementName = "DeletedItemCount")]
		public ulong DeletedItemCount { get; set; }

		[XmlElement(ElementName = "DeletedItemSize")]
		public ulong DeletedItemSize { get; set; }

		[XmlElement(ElementName = "DeletedFAIItemCount")]
		public ulong DeletedFAIItemCount { get; set; }

		[XmlElement(ElementName = "DeletedFAIItemSize")]
		public ulong DeletedFAIItemSize { get; set; }

		public override string ToString()
		{
			return MrsStrings.ItemCountsAndSizes(this.ItemCount, new ByteQuantifiedSize(this.ItemSize).ToString(), this.DeletedItemCount, new ByteQuantifiedSize(this.DeletedItemSize).ToString(), this.FAIItemCount, new ByteQuantifiedSize(this.FAIItemSize).ToString(), this.DeletedFAIItemCount, new ByteQuantifiedSize(this.DeletedFAIItemSize).ToString());
		}
	}
}
