using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class BadItemMarker : XMLSerializableBase
	{
		public BadItemMarker()
		{
		}

		internal BadItemMarker(BadMessageRec badItem)
		{
			this.Kind = badItem.Kind;
			this.EntryId = badItem.EntryId;
			this.MessageSize = badItem.MessageSize;
			this.Category = badItem.Category;
		}

		[XmlIgnore]
		public BadItemKind Kind { get; set; }

		[XmlElement(ElementName = "Kind")]
		public int KindInt
		{
			get
			{
				return (int)this.Kind;
			}
			set
			{
				this.Kind = (BadItemKind)value;
			}
		}

		[XmlElement(ElementName = "EntryId")]
		public byte[] EntryId { get; set; }

		[XmlElement(ElementName = "MessageSize")]
		public int? MessageSize { get; set; }

		[XmlElement(ElementName = "Category")]
		public string Category { get; set; }
	}
}
