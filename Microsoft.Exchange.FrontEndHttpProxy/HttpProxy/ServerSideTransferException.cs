using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Web;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	[Serializable]
	internal class ServerSideTransferException : HttpException
	{
		public ServerSideTransferException(string redirectUrl, LegacyRedirectTypeOptions redirectType)
		{
			this.RedirectUrl = redirectUrl;
			this.RedirectType = redirectType;
		}

		public ServerSideTransferException(Exception innerException) : base(innerException.Message, innerException)
		{
		}

		public ServerSideTransferException(string redirectUrl, string message) : base(message)
		{
			this.RedirectUrl = redirectUrl;
		}

		public ServerSideTransferException(string redirectUrl, string message, Exception innerException) : base(message, innerException)
		{
			this.RedirectUrl = redirectUrl;
		}

		protected ServerSideTransferException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if (info != null)
			{
				this.RedirectUrl = (string)info.GetValue("redirectUrl", typeof(string));
			}
		}

		public string RedirectUrl { get; private set; }

		public LegacyRedirectTypeOptions RedirectType { get; private set; }

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			if (info != null)
			{
				info.AddValue("redirectUrl", this.RedirectUrl);
			}
		}
	}
}
