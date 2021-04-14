using System;
using System.IO;
using System.Net;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.ApplicationLogic.Performance;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class HttpPhotoHandler : IPhotoHandler
	{
		public HttpPhotoHandler(PhotosConfiguration configuration, IPhotoRequestOutboundSender outboundRequestSender, IPhotoServiceLocator serviceLocator, IPhotoRequestOutboundWebProxyProvider outgoingRequestProxyProvider, ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNull("outboundRequestSender", outboundRequestSender);
			ArgumentValidator.ThrowIfNull("serviceLocator", serviceLocator);
			ArgumentValidator.ThrowIfNull("outgoingRequestProxyProvider", outgoingRequestProxyProvider);
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.configuration = configuration;
			this.outboundRequestSender = outboundRequestSender;
			this.serviceLocator = serviceLocator;
			this.outgoingRequestProxyProvider = outgoingRequestProxyProvider;
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			ArgumentValidator.ThrowIfNull("response", response);
			PhotoResponse result;
			using (new StopwatchPerformanceTracker("HttpHandlerTotal", request.PerformanceLogger))
			{
				using (new ADPerformanceTracker("HttpHandlerTotal", request.PerformanceLogger))
				{
					if (request.ShouldSkipHandlers(PhotoHandlers.Http))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "HTTP HANDLER: skipped by request.");
						result = response;
					}
					else if (response.Served)
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "HTTP HANDLER: skipped because photo has already been served by an upstream handler.");
						result = response;
					}
					else if (request.TargetRecipient == null || !(request.TargetRecipient is ADUser))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "HTTP HANDLER: skipped because target recipient has not been initialized OR is of unexpected type.");
						result = response;
					}
					else
					{
						response.HttpHandlerProcessed = true;
						request.PerformanceLogger.Log("HttpHandlerProcessed", string.Empty, 1U);
						try
						{
							HttpWebRequest httpRequestToConfigure = new HttpPhotoRequestBuilder(this.configuration, this.tracer).Build(this.LocatePhotoService(request), request, this.outgoingRequestProxyProvider, request.Trace);
							using (HttpWebResponse httpWebResponse = this.ConfigureAndSendRequest(request, httpRequestToConfigure))
							{
								result = this.ProcessResponseFromRemoteServer(request, response, httpWebResponse);
							}
						}
						catch (WebException ex)
						{
							this.WriteTracesCollectedByRemoteServerOntoLocalTracer(ex.Response);
							HttpStatusCode httpStatusCodeFromWebException = HttpPhotoHandler.GetHttpStatusCodeFromWebException(ex);
							this.tracer.TraceDebug<HttpStatusCode, WebExceptionStatus>((long)this.GetHashCode(), "HTTP HANDLER: caught WebException and translated it to HTTP {0}.  Web exception status: {1}", httpStatusCodeFromWebException, ex.Status);
							HttpStatusCode httpStatusCode = httpStatusCodeFromWebException;
							if (httpStatusCode <= HttpStatusCode.Unauthorized)
							{
								if (httpStatusCode == HttpStatusCode.NotModified)
								{
									response.Served = true;
									response.Status = HttpStatusCode.NotModified;
									response.HttpExpiresHeader = HttpPhotoHandler.GetHeaderValueFromWebException(ex, HttpResponseHeader.Expires);
									response.ETag = HttpPhotoHandler.GetHeaderValueFromWebException(ex, HttpResponseHeader.ETag);
									return response;
								}
								if (httpStatusCode == HttpStatusCode.Unauthorized)
								{
									response.Status = HttpStatusCode.Unauthorized;
									request.PerformanceLogger.Log("HttpHandlerError", string.Empty, 1U);
									return response;
								}
							}
							else
							{
								if (httpStatusCode == HttpStatusCode.NotFound)
								{
									response.Served = true;
									response.Status = HttpStatusCode.NotFound;
									response.HttpExpiresHeader = HttpPhotoHandler.GetHeaderValueFromWebException(ex, HttpResponseHeader.Expires);
									return response;
								}
								if (httpStatusCode != HttpStatusCode.InternalServerError)
								{
								}
							}
							response.Status = HttpStatusCode.InternalServerError;
							request.PerformanceLogger.Log("HttpHandlerError", string.Empty, 1U);
							result = response;
						}
						catch (BackEndLocatorException arg)
						{
							this.tracer.TraceError<BackEndLocatorException>((long)this.GetHashCode(), "HTTP HANDLER: failed to locate service.  Exception: {0}", arg);
							request.PerformanceLogger.Log("HttpHandlerError", string.Empty, 1U);
							result = response;
						}
						catch (IOException arg2)
						{
							this.tracer.TraceError<IOException>((long)this.GetHashCode(), "HTTP HANDLER: request to remote server failed with I/O exception: {0}", arg2);
							request.PerformanceLogger.Log("HttpHandlerError", string.Empty, 1U);
							result = response;
						}
						catch (ProtocolViolationException arg3)
						{
							this.tracer.TraceError<ProtocolViolationException>((long)this.GetHashCode(), "HTTP HANDLER: request to remote server failed with a protocol violation.  Exception: {0}", arg3);
							request.PerformanceLogger.Log("HttpHandlerError", string.Empty, 1U);
							result = response;
						}
						catch (InvalidOperationException arg4)
						{
							this.tracer.TraceError<InvalidOperationException>((long)this.GetHashCode(), "HTTP HANDLER: request to remote server failed with invalid operation.  Exception: {0}", arg4);
							request.PerformanceLogger.Log("HttpHandlerError", string.Empty, 1U);
							result = response;
						}
						catch (NotSupportedException arg5)
						{
							this.tracer.TraceError<NotSupportedException>((long)this.GetHashCode(), "HTTP HANDLER: request to remote server with an unsupported operation.  Exception: {0}", arg5);
							request.PerformanceLogger.Log("HttpHandlerError", string.Empty, 1U);
							result = response;
						}
						catch (TimeoutException arg6)
						{
							this.tracer.TraceError<TimeoutException>((long)this.GetHashCode(), "HTTP HANDLER: timed out.  Exception: {0}", arg6);
							request.PerformanceLogger.Log("HttpHandlerError", string.Empty, 1U);
							result = response;
						}
						catch (TransientException arg7)
						{
							this.tracer.TraceError<TransientException>((long)this.GetHashCode(), "HTTP HANDLER: failed with a transient error.  Exception: {0}", arg7);
							request.PerformanceLogger.Log("HttpHandlerError", string.Empty, 1U);
							result = response;
						}
					}
				}
			}
			return result;
		}

		public IPhotoHandler Then(IPhotoHandler next)
		{
			return new CompositePhotoHandler(this, next);
		}

		private HttpWebResponse ConfigureAndSendRequest(PhotoRequest request, HttpWebRequest httpRequestToConfigure)
		{
			return this.SendRequestOutbound(request, this.ConfigureOutgoingRequest(request, httpRequestToConfigure));
		}

		private HttpWebRequest ConfigureOutgoingRequest(PhotoRequest request, HttpWebRequest httpRequest)
		{
			httpRequest.UserAgent = "Exchange/15.0 (HttpPhotoHandler)";
			httpRequest.Timeout = this.configuration.OutgoingPhotoRequestTimeoutMilliseconds;
			httpRequest.Headers.Add("RequestType", "CrossSite");
			if (!string.IsNullOrEmpty(request.ClientRequestId))
			{
				httpRequest.Headers.Add("client-request-id", request.ClientRequestId);
			}
			return httpRequest;
		}

		private HttpWebResponse SendRequestOutbound(PhotoRequest request, HttpWebRequest httpRequest)
		{
			HttpWebResponse result;
			using (new StopwatchPerformanceTracker("HttpHandlerSendRequestAndGetResponse", request.PerformanceLogger))
			{
				using (new ADPerformanceTracker("HttpHandlerSendRequestAndGetResponse", request.PerformanceLogger))
				{
					result = this.outboundRequestSender.SendAndGetResponse(httpRequest);
				}
			}
			return result;
		}

		private PhotoResponse ProcessResponseFromRemoteServer(PhotoRequest request, PhotoResponse response, HttpWebResponse httpResponse)
		{
			this.WriteTracesCollectedByRemoteServerOntoLocalTracer(httpResponse);
			HttpStatusCode statusCode = httpResponse.StatusCode;
			if (statusCode <= HttpStatusCode.NotModified)
			{
				if (statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.NotModified)
				{
					goto IL_78;
				}
			}
			else if (statusCode != HttpStatusCode.Unauthorized && statusCode == HttpStatusCode.NotFound)
			{
				goto IL_78;
			}
			this.tracer.TraceError<HttpStatusCode, string>((long)this.GetHashCode(), "HTTP HANDLER: request to remote service FAILED.  Returning HTTP {0}: {1}", httpResponse.StatusCode, httpResponse.StatusDescription);
			response.Status = httpResponse.StatusCode;
			return response;
			IL_78:
			using (new StopwatchPerformanceTracker("HttpHandlerGetAndReadResponseStream", request.PerformanceLogger))
			{
				using (Stream responseStream = httpResponse.GetResponseStream())
				{
					string text = httpResponse.Headers[HttpResponseHeader.ETag];
					string text2 = httpResponse.Headers[HttpResponseHeader.Expires];
					this.tracer.TraceDebug((long)this.GetHashCode(), "HTTP HANDLER: request to remote service was SUCCESSFUL.  HTTP status: {0};  ETag: {1};  Content-Length: {2};  Content-Type: {3};  Expires: {4}", new object[]
					{
						httpResponse.StatusCode,
						text,
						httpResponse.ContentLength,
						httpResponse.ContentType,
						text2
					});
					responseStream.CopyTo(response.OutputPhotoStream);
					response.Served = true;
					response.ContentLength = httpResponse.ContentLength;
					response.ContentType = httpResponse.ContentType;
					response.Status = httpResponse.StatusCode;
					response.ETag = text;
					response.HttpExpiresHeader = text2;
				}
			}
			return response;
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

		private void WriteTracesCollectedByRemoteServerOntoLocalTracer(WebResponse httpResponse)
		{
			string text = PhotosDiagnostics.Instance.ReadGetUserPhotoTracesFromResponse(httpResponse);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			this.tracer.TraceDebug<Uri, string>((long)this.GetHashCode(), "Traces collected by {0}: [[ {1} ]]", httpResponse.ResponseUri, text);
		}

		private Uri LocatePhotoService(PhotoRequest request)
		{
			Uri result;
			using (new StopwatchPerformanceTracker("HttpHandlerLocateService", request.PerformanceLogger))
			{
				using (new ADPerformanceTracker("HttpHandlerLocateService", request.PerformanceLogger))
				{
					result = this.serviceLocator.Locate((ADUser)request.TargetRecipient);
				}
			}
			return result;
		}

		private const string ProxyRequestTypeHeaderName = "RequestType";

		private const string ProxyRequestType = "CrossSite";

		private const HttpStatusCode UnknownStatusCode = (HttpStatusCode)0;

		private const string UserAgentForOutgoingRequests = "Exchange/15.0 (HttpPhotoHandler)";

		private readonly PhotosConfiguration configuration;

		private readonly IPhotoRequestOutboundSender outboundRequestSender;

		private readonly IPhotoServiceLocator serviceLocator;

		private readonly IPhotoRequestOutboundWebProxyProvider outgoingRequestProxyProvider;

		private readonly ITracer tracer = ExTraceGlobals.UserPhotosTracer;
	}
}
