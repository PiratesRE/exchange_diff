using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class TargetNotFoundPhotoHandler : IPhotoHandler
	{
		public TargetNotFoundPhotoHandler(PhotosConfiguration configuration, ITracer upstreamTracer)
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
			if (response.Served)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "TARGET NOT FOUND HANDLER: skipped because photo has already been served by an upstream handler.");
				return response;
			}
			this.tracer.TraceDebug((long)this.GetHashCode(), "TARGET NOT FOUND HANDLER: responding request with HTTP 404 Not Found.");
			response.TargetNotFoundHandlerProcessed = true;
			response.Served = true;
			response.Status = HttpStatusCode.NotFound;
			response.HttpExpiresHeader = UserAgentPhotoExpiresHeader.Default.ComputeExpiresHeader(DateTime.UtcNow, HttpStatusCode.NotFound, this.configuration);
			return response;
		}

		public IPhotoHandler Then(IPhotoHandler next)
		{
			throw new NotImplementedException();
		}

		private readonly PhotosConfiguration configuration;

		private readonly ITracer tracer;
	}
}
