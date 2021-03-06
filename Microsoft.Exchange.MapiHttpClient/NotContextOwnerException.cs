using System;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public sealed class NotContextOwnerException : ProtocolFailureException
	{
		public NotContextOwnerException(LID failureLID, string failureDescription, string failureInfo, HttpStatusCode httpStatusCode, string httpStatusDescription, Exception innerException, WebHeaderCollection requestHeaders, WebHeaderCollection responseHeaders) : base(failureLID, failureDescription, failureInfo, httpStatusCode, httpStatusDescription, ResponseCode.NotContextOwner, ServiceCode.Success, innerException, requestHeaders, responseHeaders)
		{
		}

		private NotContextOwnerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
