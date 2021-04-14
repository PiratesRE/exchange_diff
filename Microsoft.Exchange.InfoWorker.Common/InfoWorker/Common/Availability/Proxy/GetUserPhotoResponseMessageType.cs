using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetUserPhotoResponseMessageType : ResponseMessage
	{
		internal string CacheId
		{
			get
			{
				return this.cacheId;
			}
			set
			{
				this.cacheId = value;
			}
		}

		internal HttpStatusCode StatusCode
		{
			get
			{
				return this.code;
			}
			set
			{
				this.code = value;
			}
		}

		internal string Expires
		{
			get
			{
				return this.expires;
			}
			set
			{
				this.expires = value;
			}
		}

		public byte[] PictureData
		{
			get
			{
				return this.pictureData;
			}
			set
			{
				this.pictureData = value;
			}
		}

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

		private byte[] pictureData;

		private bool hasChanged;

		private string cacheId;

		private HttpStatusCode code;

		private string expires;

		private string contentType;
	}
}
