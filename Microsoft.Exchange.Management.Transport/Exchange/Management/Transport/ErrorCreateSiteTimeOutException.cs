using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ErrorCreateSiteTimeOutException : SpCsomCallException
	{
		public ErrorCreateSiteTimeOutException(string url) : base(Strings.ErrorCreateSiteTimeOut(url))
		{
			this.url = url;
		}

		public ErrorCreateSiteTimeOutException(string url, Exception innerException) : base(Strings.ErrorCreateSiteTimeOut(url), innerException)
		{
			this.url = url;
		}

		protected ErrorCreateSiteTimeOutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.url = (string)info.GetValue("url", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("url", this.url);
		}

		public string Url
		{
			get
			{
				return this.url;
			}
		}

		private readonly string url;
	}
}
