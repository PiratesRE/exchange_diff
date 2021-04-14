using System;
using System.IO;
using System.Net;
using Microsoft.Exchange.Data.ApplicationLogic.Performance;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RemoteForestPhotoHandler : IPhotoHandler
	{
		public RemoteForestPhotoHandler(PhotosConfiguration configuration, ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.configuration = configuration;
			this.tracer = upstreamTracer;
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			ArgumentValidator.ThrowIfNull("response", response);
			PhotoResponse result;
			using (new StopwatchPerformanceTracker("RemoteForestHandlerTotal", request.PerformanceLogger))
			{
				using (new ADPerformanceTracker("RemoteForestHandlerTotal", request.PerformanceLogger))
				{
					if (request.ShouldSkipHandlers(PhotoHandlers.RemoteForest))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "REMOTE FOREST HANDLER: skipped by request.");
						result = response;
					}
					else if (response.Served)
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "REMOTE FOREST HANDLER: skipped because photo has already been served by an upstream handler.");
						result = response;
					}
					else
					{
						response.RemoteForestHandlerProcessed = true;
						request.PerformanceLogger.Log("RemoteForestHandlerProcessed", string.Empty, 1U);
						GetUserPhotoQuery getUserPhotoQuery;
						using (new StopwatchPerformanceTracker("QueryCreation", request.PerformanceLogger))
						{
							using (new ADPerformanceTracker("QueryCreation", request.PerformanceLogger))
							{
								getUserPhotoQuery = new GetUserPhotoQuery((ClientContext)request.ClientContextForRemoteForestRequests, request, null, request.RequestorFromExternalOrganization, this.configuration, this.tracer);
							}
						}
						try
						{
							byte[] array = getUserPhotoQuery.Execute();
							if (RemoteForestPhotoHandler.IsInvalidHttpStatusCode(getUserPhotoQuery.StatusCode))
							{
								result = this.RespondWithErrorBecauseQueryReturnedInvalidStatusCode(request, response);
							}
							else
							{
								response.Served = true;
								response.Status = getUserPhotoQuery.StatusCode;
								response.ETag = getUserPhotoQuery.ETag;
								response.HttpExpiresHeader = getUserPhotoQuery.Expires;
								response.ContentType = getUserPhotoQuery.ContentType;
								this.tracer.TraceDebug((long)this.GetHashCode(), "REMOTE FOREST HANDLER:  query completed.  Result is empty? {0}; HTTP status: {1}; ETag: {2}; Content-Type: {3}; HTTP Expires: {4}", new object[]
								{
									array == null || array.Length == 0,
									response.Status,
									getUserPhotoQuery.ETag,
									getUserPhotoQuery.ContentType,
									getUserPhotoQuery.Expires
								});
								using (MemoryStream memoryStream = new MemoryStream((array == null) ? Array<byte>.Empty : array))
								{
									memoryStream.CopyTo(response.OutputPhotoStream);
								}
								request.PerformanceLogger.Log("RemoteForestHandlerServed", string.Empty, 1U);
								result = response;
							}
						}
						catch (ClientDisconnectedException arg)
						{
							this.tracer.TraceDebug<ClientDisconnectedException>((long)this.GetHashCode(), "REMOTE FOREST HANDLER: client disconnected.  Exception: {0}", arg);
							result = this.ServePhotoWhenClientDisconnected(request, response);
						}
						catch (AccessDeniedException arg2)
						{
							this.tracer.TraceDebug<AccessDeniedException>((long)this.GetHashCode(), "REMOTE FOREST HANDLER: access denied.  Requestor does NOT have permission to retrieve this photo.  Exception: {0}", arg2);
							result = this.RespondWithAccessDenied(request, response);
						}
						finally
						{
							if (getUserPhotoQuery != null && getUserPhotoQuery.RequestLogger != null && getUserPhotoQuery.RequestLogger.LogData != null)
							{
								string text = getUserPhotoQuery.RequestLogger.LogData.ToString();
								if (!string.IsNullOrEmpty(text))
								{
									this.tracer.TracePerformance<string>((long)this.GetHashCode(), "REMOTE FOREST HANDLER: {0}", text);
									request.PerformanceLogger.Log("MiscRoutingAndDiscovery", string.Empty, text);
								}
							}
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

		private static bool IsInvalidHttpStatusCode(HttpStatusCode code)
		{
			return code == (HttpStatusCode)0;
		}

		private PhotoResponse ServePhotoWhenClientDisconnected(PhotoRequest request, PhotoResponse response)
		{
			response.Served = true;
			response.Status = HttpStatusCode.InternalServerError;
			request.PerformanceLogger.Log("RemoteForestHandlerServed", string.Empty, 1U);
			return response;
		}

		private PhotoResponse RespondWithAccessDenied(PhotoRequest request, PhotoResponse response)
		{
			response.Served = true;
			response.Status = HttpStatusCode.Forbidden;
			request.PerformanceLogger.Log("RemoteForestHandlerServed", string.Empty, 1U);
			return response;
		}

		private PhotoResponse RespondWithErrorBecauseQueryReturnedInvalidStatusCode(PhotoRequest request, PhotoResponse response)
		{
			this.tracer.TraceError((long)this.GetHashCode(), "REMOTE FOREST HANDLER: HTTP status code in query is invalid.  Overwriting with InternalServerError.");
			request.PerformanceLogger.Log("RemoteForestHandlerError", string.Empty, 1U);
			response.Served = false;
			response.Status = HttpStatusCode.InternalServerError;
			return response;
		}

		private readonly PhotosConfiguration configuration;

		private readonly ITracer tracer;
	}
}
