using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class TooComplexPhotoHandler : IPhotoHandler
	{
		public TooComplexPhotoHandler(ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.tracer = upstreamTracer;
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			this.tracer.TraceDebug((long)this.GetHashCode(), "TOO COMPLEX HANDLER: rejecting request.");
			throw new TooComplexPhotoRequestException();
		}

		public IPhotoHandler Then(IPhotoHandler next)
		{
			throw new NotImplementedException();
		}

		private readonly ITracer tracer;
	}
}
