using System;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RemoteForestPhotoRetrievalPipelineUsingAvailabilityServiceFactory : IRemoteForestPhotoRetrievalPipelineFactory
	{
		public RemoteForestPhotoRetrievalPipelineUsingAvailabilityServiceFactory(PhotosConfiguration configuration, IRecipientSession recipientSession, ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.configuration = configuration;
			this.recipientSession = recipientSession;
			this.tracer = upstreamTracer;
		}

		public IPhotoHandler Create()
		{
			return new RemoteForestPhotoRetrievalPipeline(this.configuration, this.recipientSession, this.tracer);
		}

		private readonly PhotosConfiguration configuration;

		private readonly IRecipientSession recipientSession;

		private readonly ITracer tracer;
	}
}
