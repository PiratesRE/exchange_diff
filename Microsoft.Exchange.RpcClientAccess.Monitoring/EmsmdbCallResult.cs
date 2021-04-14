using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class EmsmdbCallResult : RpcCallResult
	{
		protected EmsmdbCallResult(Exception exception, IPropertyBag httpResponseInformation) : this(exception, ErrorCode.None, null, null, httpResponseInformation)
		{
		}

		protected EmsmdbCallResult(Exception exception, ErrorCode errorCode, ExceptionTraceAuxiliaryBlock remoteExceptionTrace) : this(exception, errorCode, remoteExceptionTrace, null, null)
		{
		}

		protected EmsmdbCallResult(Exception exception, ErrorCode errorCode, ExceptionTraceAuxiliaryBlock remoteExceptionTrace, MonitoringActivityAuxiliaryBlock activityContext, IPropertyBag httpResponseInformation) : base(exception, errorCode, remoteExceptionTrace, activityContext)
		{
			if (httpResponseInformation != null)
			{
				WebHeaderCollection webHeaderCollection;
				if (httpResponseInformation.TryGet(ContextPropertySchema.RequestHeaderCollection, out webHeaderCollection))
				{
					this.httpRequestHeaders = webHeaderCollection;
				}
				webHeaderCollection = null;
				if (httpResponseInformation.TryGet(ContextPropertySchema.ResponseHeaderCollection, out webHeaderCollection))
				{
					this.httpResponseHeaders = webHeaderCollection;
				}
				httpResponseInformation.TryGet(ContextPropertySchema.ResponseStatusCode, out this.httpStatusCode);
				httpResponseInformation.TryGet(ContextPropertySchema.ResponseStatusCodeDescription, out this.httpStatusCodeDescription);
			}
			this.FilterHttpHeaders();
		}

		public WebHeaderCollection HttpResponseHeaders
		{
			get
			{
				return this.httpResponseHeaders;
			}
		}

		public WebHeaderCollection HttpRequestHeaders
		{
			get
			{
				return this.httpRequestHeaders;
			}
		}

		public HttpStatusCode HttpResponseStatusCode
		{
			get
			{
				return this.httpStatusCode;
			}
		}

		public string HttpResponseStatusCodeDescription
		{
			get
			{
				return this.httpStatusCodeDescription;
			}
		}

		private void FilterHttpHeaders()
		{
			this.httpRequestHeaders.Remove("Cookie");
			this.httpResponseHeaders.Remove("Set-Cookie");
		}

		private readonly WebHeaderCollection httpResponseHeaders = new WebHeaderCollection();

		private readonly WebHeaderCollection httpRequestHeaders = new WebHeaderCollection();

		private readonly HttpStatusCode httpStatusCode;

		private readonly string httpStatusCodeDescription;
	}
}
