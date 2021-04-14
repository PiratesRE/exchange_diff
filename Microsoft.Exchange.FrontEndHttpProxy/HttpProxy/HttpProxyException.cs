using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;

namespace Microsoft.Exchange.HttpProxy
{
	[Serializable]
	internal class HttpProxyException : Exception
	{
		public HttpProxyException(HttpStatusCode statusCode, HttpProxySubErrorCode errorCode, string message) : base(message)
		{
			this.statusCode = statusCode;
			this.errorCode = errorCode;
		}

		public HttpProxyException(HttpStatusCode statusCode, HttpProxySubErrorCode errorCode, string message, Exception innerException) : base(message, innerException)
		{
			this.statusCode = statusCode;
			this.errorCode = errorCode;
		}

		protected HttpProxyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if (info != null)
			{
				this.statusCode = (HttpStatusCode)info.GetValue("statusCode", typeof(int));
				this.errorCode = (HttpProxySubErrorCode)info.GetValue("errorCode", typeof(HttpProxySubErrorCode));
			}
		}

		public HttpStatusCode StatusCode
		{
			get
			{
				return this.statusCode;
			}
		}

		public HttpProxySubErrorCode ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			if (info != null)
			{
				info.AddValue("statusCode", this.StatusCode);
				info.AddValue("errorCode", this.ErrorCode);
			}
		}

		private readonly HttpStatusCode statusCode;

		private readonly HttpProxySubErrorCode errorCode;
	}
}
