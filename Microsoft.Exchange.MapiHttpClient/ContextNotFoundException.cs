using System;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public sealed class ContextNotFoundException : ProtocolFailureException
	{
		public ContextNotFoundException(LID failureLID, string failureDescription, string failureInfo, HttpStatusCode httpStatusCode, string httpStatusDescription, Exception innerException, WebHeaderCollection requestHeaders, WebHeaderCollection responseHeaders) : base(failureLID, failureDescription, failureInfo, httpStatusCode, httpStatusDescription, ResponseCode.ContextNotFound, ServiceCode.Success, innerException, requestHeaders, responseHeaders)
		{
		}

		private ContextNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
