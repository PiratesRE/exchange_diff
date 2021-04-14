using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.SoapWebClient;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RestService : IService, CertificateValidationManager.IComponent
	{
		internal RestService(HttpAuthenticator authenticator, WebServiceUri webServiceUri, bool traceRequest, ITracer upstreamTracer)
		{
			this.authenticator = authenticator;
			this.componentId = Globals.CertificateValidationComponentId;
			this.httpHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			this.traceRequest = traceRequest;
			this.tracer = upstreamTracer;
			this.targetUri = webServiceUri.Uri;
			CertificateValidationManager.RegisterCallback(this.componentId, new RemoteCertificateValidationCallback(CertificateErrorHandler.CertValidationCallback));
		}

		public Dictionary<string, string> HttpHeaders
		{
			get
			{
				return this.httpHeaders;
			}
		}

		public CookieContainer CookieContainer { get; set; }

		public IWebProxy Proxy { get; set; }

		public ICredentials Credentials { get; set; }

		public bool EnableDecompression { get; set; }

		public string UserAgent { get; set; }

		public string Url
		{
			get
			{
				return this.targetUri.OriginalString;
			}
		}

		public int Timeout { get; set; }

		public bool SupportsProxyAuthentication
		{
			get
			{
				return false;
			}
		}

		public RequestTypeHeader requestTypeValue { get; set; }

		public RequestServerVersion RequestServerVersionValue { get; set; }

		public int ServiceVersion
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string GetComponentId()
		{
			return this.componentId;
		}

		public void Abort()
		{
			if (this.httpPhotoRequest != null)
			{
				this.httpPhotoRequest.Abort();
			}
		}

		public void Dispose()
		{
		}

		private HttpWebRequest ConfigureRequest(HttpWebRequest request)
		{
			request.CookieContainer = this.CookieContainer;
			request.Proxy = this.Proxy;
			request.Credentials = this.Credentials;
			request.UserAgent = this.UserAgent;
			request.Timeout = this.Timeout;
			if (!string.IsNullOrEmpty(this.componentId))
			{
				CertificateValidationManager.SetComponentId(request, this.componentId);
			}
			return this.ApplyHeadersToRequest(request);
		}

		private HttpWebRequest ApplyHeadersToRequest(HttpWebRequest request)
		{
			if (this.httpHeaders == null || this.httpHeaders.Count == 0)
			{
				return request;
			}
			foreach (KeyValuePair<string, string> keyValuePair in this.httpHeaders)
			{
				request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
			}
			if (this.requestTypeValue != null)
			{
				request.Headers.Add("RequestType", this.requestTypeValue.RequestType.ToString());
			}
			return request;
		}

		private IAsyncResult AuthenticateAndBeginInvoke(WebRequest request, AuthenticateAndExecuteHandler<IAsyncResult> methodToInvoke)
		{
			return this.authenticator.AuthenticateAndExecute<IAsyncResult>(request, methodToInvoke);
		}

		public IAsyncResult BeginGetUserAvailability(GetUserAvailabilityRequest request, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public GetUserAvailabilityResponse EndGetUserAvailability(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginGetMailTips(GetMailTipsType getMailTips1, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public GetMailTipsResponseMessageType EndGetMailTips(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginGetUserPhoto(PhotoRequest request, PhotosConfiguration configuration, AsyncCallback callback, object asyncState)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			HttpWebRequest request2 = new HttpPhotoRequestBuilder(configuration, this.tracer).Build(this.targetUri, request, new PhotoRequestOutboundWebProxyProviderUsingLocalServerConfiguration(this.tracer), this.traceRequest);
			this.httpPhotoRequest = this.ConfigureRequest(request2);
			return this.AuthenticateAndBeginInvoke(this.httpPhotoRequest, () => this.httpPhotoRequest.BeginGetResponse(callback, asyncState));
		}

		public GetUserPhotoResponseMessageType EndGetUserPhoto(IAsyncResult asyncResult)
		{
			ArgumentValidator.ThrowIfNull("asyncResult", asyncResult);
			GetUserPhotoResponseMessageType result;
			try
			{
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)this.httpPhotoRequest.EndGetResponse(asyncResult))
				{
					this.WriteTracesCollectedByRemoteServerOntoLocalTracer(httpWebResponse);
					HttpStatusCode statusCode = httpWebResponse.StatusCode;
					if (statusCode <= HttpStatusCode.NotModified)
					{
						if (statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.NotModified)
						{
							goto IL_98;
						}
					}
					else if (statusCode == HttpStatusCode.NotFound)
					{
						goto IL_98;
					}
					this.tracer.TraceError<HttpStatusCode, string>((long)this.GetHashCode(), "REST service proxy: request to remote service FAILED.  Returning HTTP {0}: {1}", httpWebResponse.StatusCode, httpWebResponse.StatusDescription);
					return new GetUserPhotoResponseMessageType
					{
						StatusCode = httpWebResponse.StatusCode
					};
					IL_98:
					using (Stream responseStream = httpWebResponse.GetResponseStream())
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							string text = httpWebResponse.Headers[HttpResponseHeader.ETag];
							string text2 = httpWebResponse.Headers[HttpResponseHeader.Expires];
							this.tracer.TraceDebug((long)this.GetHashCode(), "REST service proxy: returning photo from proxy.  HTTP status: {0};  ETag: {1};  Expires: {2};  Content-Length: {3};  Content-Type: {4}", new object[]
							{
								httpWebResponse.StatusCode,
								text,
								text2,
								httpWebResponse.ContentLength,
								httpWebResponse.ContentType
							});
							responseStream.CopyTo(memoryStream);
							result = new GetUserPhotoResponseMessageType
							{
								StatusCode = httpWebResponse.StatusCode,
								CacheId = text,
								Expires = text2,
								HasChanged = (httpWebResponse.StatusCode != HttpStatusCode.NotModified),
								PictureData = memoryStream.ToArray(),
								ContentType = httpWebResponse.ContentType
							};
						}
					}
				}
			}
			catch (WebException ex)
			{
				this.WriteTracesCollectedByRemoteServerOntoLocalTracer(ex.Response);
				HttpStatusCode httpStatusCodeFromWebException = RestService.GetHttpStatusCodeFromWebException(ex);
				this.tracer.TraceDebug<HttpStatusCode>((long)this.GetHashCode(), "REST service proxy: caught WebException and translated it to HTTP {0}", httpStatusCodeFromWebException);
				HttpStatusCode httpStatusCode = httpStatusCodeFromWebException;
				if (httpStatusCode != HttpStatusCode.NotModified)
				{
					if (httpStatusCode != HttpStatusCode.NotFound)
					{
						if (httpStatusCode != HttpStatusCode.InternalServerError)
						{
							throw;
						}
						result = new GetUserPhotoResponseMessageType
						{
							StatusCode = HttpStatusCode.InternalServerError
						};
					}
					else
					{
						result = new GetUserPhotoResponseMessageType
						{
							Expires = RestService.GetHeaderValueFromWebException(ex, HttpResponseHeader.Expires),
							StatusCode = HttpStatusCode.NotFound
						};
					}
				}
				else
				{
					result = new GetUserPhotoResponseMessageType
					{
						StatusCode = HttpStatusCode.NotModified,
						CacheId = RestService.GetHeaderValueFromWebException(ex, HttpResponseHeader.ETag),
						Expires = RestService.GetHeaderValueFromWebException(ex, HttpResponseHeader.Expires),
						HasChanged = false
					};
				}
			}
			return result;
		}

		private static HttpStatusCode GetHttpStatusCodeFromWebException(WebException e)
		{
			HttpWebResponse httpWebResponse = e.Response as HttpWebResponse;
			if (httpWebResponse == null)
			{
				return (HttpStatusCode)0;
			}
			return httpWebResponse.StatusCode;
		}

		private static string GetHeaderValueFromWebException(WebException e, HttpResponseHeader header)
		{
			if (e == null || e.Response == null)
			{
				return null;
			}
			return e.Response.Headers[header];
		}

		private void WriteTracesCollectedByRemoteServerOntoLocalTracer(WebResponse response)
		{
			string text = PhotosDiagnostics.Instance.ReadGetUserPhotoTracesFromResponse(response);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			this.tracer.TraceDebug<Uri, string>((long)this.GetHashCode(), "Traces collected by {0}: [[ {1} ]]", response.ResponseUri, text);
		}

		public IAsyncResult BeginFindMessageTrackingReport(FindMessageTrackingReportRequestType findMessageTrackingReport1, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public FindMessageTrackingReportResponseMessageType EndFindMessageTrackingReport(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginGetMessageTrackingReport(GetMessageTrackingReportRequestType getMessageTrackingReport1, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public GetMessageTrackingReportResponseMessageType EndGetMessageTrackingReport(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		private const string ProxyRequestTypeHeaderName = "RequestType";

		private readonly ITracer tracer;

		private readonly bool traceRequest;

		private readonly Dictionary<string, string> httpHeaders;

		private readonly string componentId;

		private readonly HttpAuthenticator authenticator;

		private readonly Uri targetUri;

		private HttpWebRequest httpPhotoRequest;
	}
}
