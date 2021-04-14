using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class HttpRequestTimeoutException : LocalizedException
	{
		public HttpRequestTimeoutException(string httpRequest, string statusCode) : base(Strings.messageHttpRequestTimeoutException(httpRequest, statusCode))
		{
			this.httpRequest = httpRequest;
			this.statusCode = statusCode;
		}

		public HttpRequestTimeoutException(string httpRequest, string statusCode, Exception innerException) : base(Strings.messageHttpRequestTimeoutException(httpRequest, statusCode), innerException)
		{
			this.httpRequest = httpRequest;
			this.statusCode = statusCode;
		}

		protected HttpRequestTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.httpRequest = (string)info.GetValue("httpRequest", typeof(string));
			this.statusCode = (string)info.GetValue("statusCode", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("httpRequest", this.httpRequest);
			info.AddValue("statusCode", this.statusCode);
		}

		public string HttpRequest
		{
			get
			{
				return this.httpRequest;
			}
		}

		public string StatusCode
		{
			get
			{
				return this.statusCode;
			}
		}

		private readonly string httpRequest;

		private readonly string statusCode;
	}
}
