using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DiagnosticsPhotoHandler : IPhotoHandler
	{
		public DiagnosticsPhotoHandler(ITracer upstreamTracer)
		{
			if (upstreamTracer == null)
			{
				throw new ArgumentNullException("upstreamTracer");
			}
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			if (request.Trace)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Diagnostics photo handler: skipped because request is already being traced.");
				return response;
			}
			if (this.ShouldTraceRequest(response))
			{
				request.Trace = true;
				this.tracer.TraceDebug((long)this.GetHashCode(), "Diagnostics photo handler: enabled tracing on this request.");
			}
			return response;
		}

		public IPhotoHandler Then(IPhotoHandler next)
		{
			return new CompositePhotoHandler(this, next);
		}

		private bool ShouldTraceRequest(PhotoResponse response)
		{
			return response.PhotoWrittenToFileSystem;
		}

		private readonly ITracer tracer;
	}
}
