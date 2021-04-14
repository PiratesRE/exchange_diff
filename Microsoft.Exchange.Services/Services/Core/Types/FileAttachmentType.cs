using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "FileAttachment")]
	[Serializable]
	public class FileAttachmentType : AttachmentType
	{
		[XmlElement(DataType = "base64Binary")]
		[IgnoreDataMember]
		public byte[] Content
		{
			get
			{
				if (this.ContentStream != null)
				{
					this.GetContentStreamBytes();
				}
				return this.contentBytes;
			}
			set
			{
				this.contentBytes = value;
			}
		}

		[DataMember(Name = "Content", EmitDefaultValue = false)]
		[XmlIgnore]
		public string ContentString
		{
			get
			{
				if (this.Content != null)
				{
					return Convert.ToBase64String(this.Content);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.Content = Convert.FromBase64String(value);
					return;
				}
				this.Content = null;
			}
		}

		internal Stream ContentStream { private get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool IsContactPhoto
		{
			get
			{
				return this.isContactPhoto;
			}
			set
			{
				this.IsContactPhotoSpecified = true;
				this.isContactPhoto = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsContactPhotoSpecified { get; set; }

		[XmlArrayItem("byte", IsNullable = false)]
		[IgnoreDataMember]
		public byte[] ImageThumbnailSalientRegions
		{
			get
			{
				return this.imageThumbnailSalientRegions;
			}
			set
			{
				this.imageThumbnailSalientRegions = value;
			}
		}

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false)]
		public int ImageThumbnailHeight
		{
			get
			{
				return this.imageThumbnailHeight;
			}
			set
			{
				this.imageThumbnailHeight = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		[XmlIgnore]
		public int ImageThumbnailWidth
		{
			get
			{
				return this.imageThumbnailWidth;
			}
			set
			{
				this.imageThumbnailWidth = value;
			}
		}

		private void GetContentStreamBytes()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				byte[] array = new byte[4096];
				int count;
				while ((count = this.ContentStream.Read(array, 0, array.Length)) != 0)
				{
					memoryStream.Write(array, 0, count);
				}
				this.contentBytes = memoryStream.ToArray();
				this.ContentStream = null;
			}
		}

		private byte[] contentBytes;

		private bool isContactPhoto;

		private byte[] imageThumbnailSalientRegions;

		private int imageThumbnailHeight;

		private int imageThumbnailWidth;
	}
}
