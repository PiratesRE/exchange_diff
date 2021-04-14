using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class MissingMessageRec : XMLSerializableBase
	{
		public MissingMessageRec()
		{
		}

		internal MissingMessageRec(MessageRec mr)
		{
			this.entryId = mr.EntryId;
			this.folderId = mr.FolderId;
			this.flags = (int)mr.Flags;
		}

		[XmlElement]
		public byte[] EntryId
		{
			get
			{
				return this.entryId;
			}
			set
			{
				this.entryId = value;
			}
		}

		[XmlElement]
		public byte[] FolderId
		{
			get
			{
				return this.folderId;
			}
			set
			{
				this.folderId = value;
			}
		}

		[XmlElement]
		public int Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		private byte[] entryId;

		private byte[] folderId;

		private int flags;
	}
}
