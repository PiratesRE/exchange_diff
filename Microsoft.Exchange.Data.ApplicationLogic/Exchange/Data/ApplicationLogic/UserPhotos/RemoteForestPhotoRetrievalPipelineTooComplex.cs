using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RemoteForestPhotoRetrievalPipelineTooComplex : IRemoteForestPhotoRetrievalPipelineFactory
	{
		public RemoteForestPhotoRetrievalPipelineTooComplex(ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.tracer = upstreamTracer;
		}

		public IPhotoHandler Create()
		{
			return new TooComplexPhotoHandler(this.tracer);
		}

		private readonly ITracer tracer;
	}
}
