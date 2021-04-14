using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidStatusDetailException : LocalizedException
	{
		public InvalidStatusDetailException(string uri) : base(Strings.InvalidStatusDetailError(uri))
		{
			this.uri = uri;
		}

		public InvalidStatusDetailException(string uri, Exception innerException) : base(Strings.InvalidStatusDetailError(uri), innerException)
		{
			this.uri = uri;
		}

		protected InvalidStatusDetailException(SerializationInfo info, StreamingContext context) : base(info, context)
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
