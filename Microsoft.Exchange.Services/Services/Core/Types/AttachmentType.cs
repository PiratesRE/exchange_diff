using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(FileAttachmentType))]
	[XmlInclude(typeof(ItemAttachmentType))]
	[XmlInclude(typeof(ReferenceAttachmentType))]
	[KnownType(typeof(ReferenceAttachmentType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(FileAttachmentType))]
	[KnownType(typeof(ItemAttachmentType))]
	[KnownType(typeof(ItemIdAttachmentType))]
	[Serializable]
	public class AttachmentType
	{
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		[XmlElement("AttachmentId")]
		public AttachmentIdType AttachmentId { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string Name { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string ContentType { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string ContentId { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string ContentLocation { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public int Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.SizeSpecified = true;
				this.size = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool SizeSpecified { get; set; }

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string LastModifiedTime
		{
			get
			{
				return this.lastModifiedTime;
			}
			set
			{
				this.LastModifiedTimeSpecified = true;
				this.lastModifiedTime = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool LastModifiedTimeSpecified { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public bool IsInline
		{
			get
			{
				return this.isInline;
			}
			set
			{
				this.IsInlineSpecified = true;
				this.isInline = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IsInlineSpecified { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public bool IsInlineToUniqueBody { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		[XmlIgnore]
		public bool IsInlineToNormalBody { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string ThumbnailMimeType { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string Thumbnail { get; set; }

		private int size;

		private string lastModifiedTime;

		private bool isInline;
	}
}
