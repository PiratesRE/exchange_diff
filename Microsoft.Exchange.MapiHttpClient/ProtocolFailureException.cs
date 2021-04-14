using System;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public class ProtocolFailureException : ProtocolException
	{
		public ProtocolFailureException(LID failureLID, string failureDescription, string failureInfo, HttpStatusCode httpStatusCode, string httpStatusDescription, ResponseCode responseCode, ServiceCode serviceCode, Exception innerException, WebHeaderCollection requestHeaders, WebHeaderCollection responseHeaders) : base(failureLID, failureDescription, failureInfo, innerException, requestHeaders, responseHeaders)
		{
			this.httpStatusCode = httpStatusCode;
			this.httpStatusDescription = httpStatusDescription;
			this.responseCode = responseCode;
			this.serviceCode = serviceCode;
		}

		protected ProtocolFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public HttpStatusCode HttpStatusCode
		{
			get
			{
				return this.httpStatusCode;
			}
		}

		public string HttpStatusDescription
		{
			get
			{
				return this.httpStatusDescription;
			}
		}

		public ResponseCode ResponseCode
		{
			get
			{
				return this.responseCode;
			}
		}

		public ServiceCode ServiceCode
		{
			get
			{
				return this.serviceCode;
			}
		}

		private readonly HttpStatusCode httpStatusCode;

		private readonly string httpStatusDescription;

		private readonly ResponseCode responseCode;

		private readonly ServiceCode serviceCode;
	}
}
