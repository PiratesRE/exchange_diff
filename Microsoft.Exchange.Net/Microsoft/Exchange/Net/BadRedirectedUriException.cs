using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class BadRedirectedUriException : LocalizedException
	{
		public BadRedirectedUriException(string uri) : base(HttpStrings.BadRedirectedUriException(uri))
		{
			this.uri = uri;
		}

		public BadRedirectedUriException(string uri, Exception innerException) : base(HttpStrings.BadRedirectedUriException(uri), innerException)
		{
			this.uri = uri;
		}

		protected BadRedirectedUriException(SerializationInfo info, StreamingContext context) : base(info, context)
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
