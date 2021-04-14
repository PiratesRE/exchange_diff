using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "GetUserPhotoResponseMessage", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetUserPhotoResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public sealed class GetUserPhotoResponseMessage : ResponseMessage
	{
		public GetUserPhotoResponseMessage()
		{
		}

		internal GetUserPhotoResponseMessage(ServiceResultCode code, ServiceError error, Stream result, bool hasChanged, string contentType) : base(code, error)
		{
			this.UserPhotoStream = result;
			this.pictureData = null;
			this.hasChanged = hasChanged;
			this.contentType = contentType;
		}

		internal Stream UserPhotoStream { get; set; }

		[DataMember(Name = "HasChanged", IsRequired = true)]
		[XmlElement("HasChanged")]
		public bool HasChanged
		{
			get
			{
				return this.hasChanged;
			}
			set
			{
				this.hasChanged = value;
			}
		}

		[XmlElement("PictureData")]
		[DataMember(Name = "PictureData", IsRequired = false)]
		public byte[] PictureData
		{
			get
			{
				if (this.pictureData == null && this.UserPhotoStream != null)
				{
					int num = (int)this.UserPhotoStream.Length;
					this.pictureData = new byte[num];
					this.UserPhotoStream.Read(this.pictureData, 0, num);
				}
				return this.pictureData;
			}
			set
			{
				this.pictureData = value;
			}
		}

		[DataMember(Name = "ContentType", IsRequired = false)]
		[XmlElement("ContentType")]
		public string ContentType
		{
			get
			{
				return this.contentType;
			}
			set
			{
				this.contentType = value;
			}
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetUserPhotoResponseMessage;
		}

		private byte[] pictureData;

		private bool hasChanged;

		private string contentType;
	}
}
