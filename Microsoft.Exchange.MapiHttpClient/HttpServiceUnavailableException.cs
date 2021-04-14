using System;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public class HttpServiceUnavailableException : ProtocolFailureException
	{
		public HttpServiceUnavailableException(LID failureLID, string failureDescription, string failureInfo, string httpStatusDescription, ResponseCode responseCode, ServiceCode serviceCode, Exception innerException, WebHeaderCollection requestHeaders, WebHeaderCollection responseHeaders) : base(failureLID, failureDescription, failureInfo, HttpStatusCode.ServiceUnavailable, httpStatusDescription, responseCode, serviceCode, innerException, requestHeaders, responseHeaders)
		{
		}

		protected HttpServiceUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
