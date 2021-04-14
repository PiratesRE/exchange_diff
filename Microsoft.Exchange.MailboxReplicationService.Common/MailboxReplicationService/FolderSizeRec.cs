using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class FolderSizeRec : XMLSerializableBase
	{
		public FolderSizeRec()
		{
			this.Source = new FolderSizeRec.CountAndSize();
			this.SourceFAI = new FolderSizeRec.CountAndSize();
			this.Target = new FolderSizeRec.CountAndSize();
			this.TargetFAI = new FolderSizeRec.CountAndSize();
			this.Corrupt = new FolderSizeRec.CountAndSize();
			this.Large = new FolderSizeRec.CountAndSize();
			this.Skipped = new FolderSizeRec.CountAndSize();
			this.Missing = new FolderSizeRec.CountAndSize();
		}

		[XmlElement(ElementName = "FolderPath")]
		public string FolderPath { get; set; }

		[XmlElement(ElementName = "FolderID")]
		public byte[] FolderID { get; set; }

		[XmlElement(ElementName = "ParentID")]
		public byte[] ParentID { get; set; }

		[XmlIgnore]
		public WellKnownFolderType WKFType { get; set; }

		[XmlElement(ElementName = "WKFType")]
		public int WKFTypeInt
		{
			get
			{
				return (int)this.WKFType;
			}
			set
			{
				this.WKFType = (WellKnownFolderType)value;
			}
		}

		[XmlElement(ElementName = "Source")]
		public FolderSizeRec.CountAndSize Source { get; set; }

		[XmlElement(ElementName = "SourceFAI")]
		public FolderSizeRec.CountAndSize SourceFAI { get; set; }

		[XmlElement(ElementName = "Target")]
		public FolderSizeRec.CountAndSize Target { get; set; }

		[XmlElement(ElementName = "TargetFAI")]
		public FolderSizeRec.CountAndSize TargetFAI { get; set; }

		[XmlElement(ElementName = "Corrupt")]
		public FolderSizeRec.CountAndSize Corrupt { get; set; }

		[XmlElement(ElementName = "Large")]
		public FolderSizeRec.CountAndSize Large { get; set; }

		[XmlElement(ElementName = "Skipped")]
		public FolderSizeRec.CountAndSize Skipped { get; set; }

		[XmlElement(ElementName = "Missing")]
		public FolderSizeRec.CountAndSize Missing { get; set; }

		[XmlElement(ElementName = "MissingItems")]
		public List<BadMessageRec> MissingItems { get; set; }

		[XmlElement(ElementName = "MailboxGuid")]
		public Guid MailboxGuid { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}: Source: {1}, Target {2}", this.FolderPath, this.Source.ToString(), this.Target.ToString());
			if (!this.SourceFAI.IsEmpty)
			{
				stringBuilder.AppendFormat(", SourceFAI: {0}, TargetFAI: {1}", this.SourceFAI.ToString(), this.TargetFAI.ToString());
			}
			if (!this.Corrupt.IsEmpty)
			{
				stringBuilder.AppendFormat(", Corrupt: {0}", this.Corrupt.ToString());
			}
			if (!this.Large.IsEmpty)
			{
				stringBuilder.AppendFormat(", Large: {0}", this.Large.ToString());
			}
			if (!this.Skipped.IsEmpty)
			{
				stringBuilder.AppendFormat(", Skipped: {0}", this.Skipped.ToString());
			}
			if (!this.Missing.IsEmpty)
			{
				stringBuilder.AppendFormat(", Missing: {0}", this.Missing.ToString());
			}
			return stringBuilder.ToString();
		}

		[Serializable]
		public sealed class CountAndSize : XMLSerializableBase
		{
			[XmlAttribute(AttributeName = "C")]
			public int Count { get; set; }

			[XmlAttribute(AttributeName = "S")]
			public ulong Size { get; set; }

			internal bool IsEmpty
			{
				get
				{
					return this.Count == 0;
				}
			}

			public override string ToString()
			{
				return string.Format("{0} [{1}]", this.Count, new ByteQuantifiedSize(this.Size).ToString());
			}

			internal void Add(MessageRec msgRec)
			{
				this.Count++;
				this.Size += (ulong)((long)msgRec.MessageSize);
			}
		}
	}
}
