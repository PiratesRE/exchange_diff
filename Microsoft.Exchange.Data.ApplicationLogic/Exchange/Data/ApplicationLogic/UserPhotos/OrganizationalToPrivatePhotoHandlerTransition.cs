using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OrganizationalToPrivatePhotoHandlerTransition : IPhotoHandler
	{
		public OrganizationalToPrivatePhotoHandlerTransition(ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.tracer = upstreamTracer;
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			ArgumentValidator.ThrowIfNull("response", response);
			if (!response.Served)
			{
				return response;
			}
			response.OrganizationalToPrivateHandlerTransitionProcessed = true;
			HttpStatusCode status = response.Status;
			if (status <= HttpStatusCode.NotModified)
			{
				if (status != HttpStatusCode.OK)
				{
					switch (status)
					{
					case HttpStatusCode.Found:
					case HttpStatusCode.NotModified:
						break;
					case HttpStatusCode.SeeOther:
						goto IL_6C;
					default:
						goto IL_6C;
					}
				}
				return response;
			}
			if (status != HttpStatusCode.NotFound && status != HttpStatusCode.InternalServerError)
			{
			}
			IL_6C:
			this.tracer.TraceDebug<HttpStatusCode>((long)this.GetHashCode(), "ORGANIZATIONAL to PRIVATE HANDLER TRANSITION: resetting response.  Original status: {0}", response.Status);
			response.Served = false;
			response.Status = HttpStatusCode.NotFound;
			response.ContentLength = -1L;
			response.ContentType = null;
			response.Thumbprint = null;
			response.HttpExpiresHeader = string.Empty;
			response.PhotoUrl = string.Empty;
			response.ServerCacheHit = false;
			return response;
		}

		public IPhotoHandler Then(IPhotoHandler next)
		{
			return new CompositePhotoHandler(this, next);
		}

		private readonly ITracer tracer;
	}
}
