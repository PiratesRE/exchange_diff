using System;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public class ProtocolException : Exception
	{
		public ProtocolException(LID failureLID, string failureDescription, string failureInfo, Exception innerException = null) : this(failureLID, failureDescription, failureInfo, innerException, null, null)
		{
		}

		public ProtocolException(LID failureLID, string failureDescription, string failureInfo, Exception innerException, WebHeaderCollection requestHeaders, WebHeaderCollection responseHeaders)
		{
			this.httpRequestHeaders = new WebHeaderCollection();
			this.httpResponseHeaders = new WebHeaderCollection();
			base..ctor(string.Format("{0} [LID={1}] {2}", failureDescription, (uint)failureLID, failureInfo), innerException);
			this.failureLID = failureLID;
			this.failureDescription = failureDescription;
			this.failureInfo = failureInfo;
			if (requestHeaders != null)
			{
				this.httpRequestHeaders = requestHeaders;
			}
			if (responseHeaders != null)
			{
				this.httpResponseHeaders = responseHeaders;
			}
		}

		protected ProtocolException(SerializationInfo info, StreamingContext context)
		{
			this.httpRequestHeaders = new WebHeaderCollection();
			this.httpResponseHeaders = new WebHeaderCollection();
			base..ctor(info, context);
		}

		public LID FailureLID
		{
			get
			{
				return this.failureLID;
			}
		}

		public string FailureDescription
		{
			get
			{
				return this.failureDescription;
			}
		}

		public string FailureInfo
		{
			get
			{
				return this.failureInfo;
			}
		}

		public WebHeaderCollection HttpRequestHeaders
		{
			get
			{
				return this.httpRequestHeaders;
			}
		}

		public WebHeaderCollection HttpResponseHeaders
		{
			get
			{
				return this.httpResponseHeaders;
			}
		}

		public static ProtocolException FromHttpStatusCode(LID failureLID, string failureDescription, string failureInfo, HttpStatusCode httpStatusCode, string httpStatusDescription, WebHeaderCollection requestHeaders, WebHeaderCollection responseHeaders, Exception innerException = null)
		{
			if (httpStatusCode == HttpStatusCode.ServiceUnavailable)
			{
				return new HttpServiceUnavailableException(failureLID, string.Format("{0} [HttpStatusCode={1} {2}]", failureDescription, (int)httpStatusCode, httpStatusDescription), failureInfo, httpStatusDescription, ResponseCode.Success, ServiceCode.Success, innerException, requestHeaders, responseHeaders);
			}
			return new ProtocolFailureException(failureLID, string.Format("{0} [HttpStatusCode={1} {2}]", failureDescription, (int)httpStatusCode, httpStatusDescription), failureInfo, httpStatusCode, httpStatusDescription, ResponseCode.Success, ServiceCode.Success, innerException, requestHeaders, responseHeaders);
		}

		public static ProtocolException FromResponseCode(LID failureLID, string failureDescription, ResponseCode responseCode, Exception innerException = null)
		{
			return ProtocolException.FromResponseCode(failureLID, failureDescription, string.Empty, HttpStatusCode.OK, "OK", responseCode, innerException, null, null, null);
		}

		public static ProtocolException FromResponseCode(LID failureLID, string failureDescription, string failureInfo, HttpStatusCode httpStatusCode, string httpStatusDescription, ResponseCode responseCode, Exception innerException, WebHeaderCollection requestHeaders, WebHeaderCollection responseHeaders, MapiHttpVersion mapiHttpVersion)
		{
			string text = string.Format("{0} [ResponseCode={1}]", failureDescription, responseCode);
			switch (responseCode)
			{
			case ResponseCode.InvalidRequestType:
				return new InvalidRequestTypeException(failureLID, text, failureInfo, httpStatusCode, httpStatusDescription, innerException, requestHeaders, responseHeaders, mapiHttpVersion);
			case ResponseCode.ContextNotFound:
				return new ContextNotFoundException(failureLID, text, failureInfo, httpStatusCode, httpStatusDescription, innerException, requestHeaders, responseHeaders);
			case ResponseCode.NotContextOwner:
				return new NotContextOwnerException(failureLID, text, failureInfo, httpStatusCode, httpStatusDescription, innerException, requestHeaders, responseHeaders);
			case ResponseCode.InvalidPayload:
				return new InvalidPayloadException(failureLID, text, failureInfo, httpStatusCode, httpStatusDescription, innerException, requestHeaders, responseHeaders);
			case ResponseCode.MissingCookie:
				return new MissingCookieException(failureLID, text, failureInfo, httpStatusCode, httpStatusDescription, innerException, requestHeaders, responseHeaders);
			case ResponseCode.InvalidSequence:
				return new InvalidSequenceException(failureLID, text, failureInfo, httpStatusCode, httpStatusDescription, innerException, requestHeaders, responseHeaders);
			case ResponseCode.EndpointDisabled:
				return new EndpointDisabledException(failureLID, text, failureInfo, httpStatusCode, httpStatusDescription, innerException, requestHeaders, responseHeaders);
			}
			return new ProtocolFailureException(failureLID, text, failureInfo, httpStatusCode, httpStatusDescription, responseCode, ServiceCode.Success, innerException, requestHeaders, responseHeaders);
		}

		public static ProtocolException FromServiceCode(LID failureLID, string failureDescription, ServiceCode serviceCode, Exception innerException = null)
		{
			return ProtocolException.FromServiceCode(failureLID, failureDescription, string.Empty, HttpStatusCode.OK, "OK", serviceCode, innerException, null, null);
		}

		public static ProtocolException FromServiceCode(LID failureLID, string failureDescription, string failureInfo, HttpStatusCode httpStatusCode, string httpStatusDescription, ServiceCode serviceCode, Exception innerException, WebHeaderCollection requestHeaders, WebHeaderCollection responseHeaders)
		{
			string text = string.Format("{0} [ServiceCode={1}]", failureDescription, serviceCode);
			switch (serviceCode)
			{
			case ServiceCode.Unavailable:
				return new ServiceUnavailableException(failureLID, text, failureInfo, httpStatusCode, httpStatusDescription, innerException, requestHeaders, responseHeaders);
			case ServiceCode.TooBusy:
				return new ServiceTooBusyException(failureLID, text, failureInfo, httpStatusCode, httpStatusDescription, innerException, requestHeaders, responseHeaders);
			default:
				return new ProtocolFailureException(failureLID, text, failureInfo, httpStatusCode, httpStatusDescription, ResponseCode.Success, serviceCode, innerException, requestHeaders, responseHeaders);
			}
		}

		public static ProtocolException FromTransportException(LID failureLID, string failureDescription, string failureInfo, HttpStatusCode httpStatusCode, string httpStatusDescription, Exception innerException, WebHeaderCollection requestHeaders, WebHeaderCollection responseHeaders)
		{
			return new ProtocolTransportException(failureLID, failureDescription, failureInfo, httpStatusCode, httpStatusDescription, innerException, requestHeaders, responseHeaders);
		}

		private readonly LID failureLID;

		private readonly string failureDescription;

		private readonly string failureInfo;

		private readonly WebHeaderCollection httpRequestHeaders;

		private readonly WebHeaderCollection httpResponseHeaders;
	}
}
