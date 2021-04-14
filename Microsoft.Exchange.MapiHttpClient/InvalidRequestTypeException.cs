using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public sealed class InvalidRequestTypeException : ProtocolFailureException
	{
		public InvalidRequestTypeException(LID failureLID, string failureDescription, string failureInfo, HttpStatusCode httpStatusCode, string httpStatusDescription, Exception innerException, WebHeaderCollection requestHeaders, WebHeaderCollection responseHeaders, MapiHttpVersion mapiHttpVersion) : base(failureLID, failureDescription, failureInfo, httpStatusCode, httpStatusDescription, ResponseCode.InvalidRequestType, ServiceCode.Success, innerException, requestHeaders, responseHeaders)
		{
			this.mapiHttpVersion = mapiHttpVersion;
		}

		public MapiHttpVersion MapiHttpVersion
		{
			get
			{
				return this.mapiHttpVersion;
			}
		}

		private readonly MapiHttpVersion mapiHttpVersion;
	}
}
