using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsupportedUriFormatException : LocalizedException
	{
		public UnsupportedUriFormatException(string uri) : base(HttpStrings.UnsupportedUriFormatException(uri))
		{
			this.uri = uri;
		}

		public UnsupportedUriFormatException(string uri, Exception innerException) : base(HttpStrings.UnsupportedUriFormatException(uri), innerException)
		{
			this.uri = uri;
		}

		protected UnsupportedUriFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.uri = (string)info.GetValue("uri", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("uri", this.uri);
		}

		public string Uri
		{
			get
			{
				return this.uri;
			}
		}

		private readonly string uri;
	}
}
