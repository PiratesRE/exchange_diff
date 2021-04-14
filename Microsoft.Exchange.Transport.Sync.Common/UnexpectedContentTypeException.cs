using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnexpectedContentTypeException : LocalizedException
	{
		public UnexpectedContentTypeException(string contentType) : base(Strings.UnexpectedContentTypeException(contentType))
		{
			this.contentType = contentType;
		}

		public UnexpectedContentTypeException(string contentType, Exception innerException) : base(Strings.UnexpectedContentTypeException(contentType), innerException)
		{
			this.contentType = contentType;
		}

		protected UnexpectedContentTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.contentType = (string)info.GetValue("contentType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("contentType", this.contentType);
		}

		public string ContentType
		{
			get
			{
				return this.contentType;
			}
		}

		private readonly string contentType;
	}
}
