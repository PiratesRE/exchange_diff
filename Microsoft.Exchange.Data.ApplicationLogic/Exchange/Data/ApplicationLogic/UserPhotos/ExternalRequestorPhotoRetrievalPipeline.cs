using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ExternalRequestorPhotoRetrievalPipeline : IPhotoHandler
	{
		public ExternalRequestorPhotoRetrievalPipeline(IRecipientSession recipientSession, ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.pipeline = new ADPhotoHandler(new ADPhotoReader(upstreamTracer), recipientSession, upstreamTracer);
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			return this.pipeline.Retrieve(request, response);
		}

		public IPhotoHandler Then(IPhotoHandler next)
		{
			throw new NotImplementedException();
		}

		private readonly IPhotoHandler pipeline;
	}
}
