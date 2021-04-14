using System;
using System.IO;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OwaPhotoRetrievalPipeline
	{
		public OwaPhotoRetrievalPipeline(PhotosConfiguration configuration, string certificateValidationComponentId, string clientInfo, IRecipientSession recipientSession, IPhotoRequestOutboundWebProxyProvider outgoingRequestProxyProvider, IRemoteForestPhotoRetrievalPipelineFactory remoteForestPipelineFactory, IXSOFactory xsoFactory, ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNullOrEmpty("certificateValidationComponentId", certificateValidationComponentId);
			ArgumentValidator.ThrowIfNullOrEmpty("clientInfo", clientInfo);
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			ArgumentValidator.ThrowIfNull("outgoingRequestProxyProvider", outgoingRequestProxyProvider);
			ArgumentValidator.ThrowIfNull("remoteForestPipelineFactory", remoteForestPipelineFactory);
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.pipeline = new OrganizationalPhotoRetrievalPipeline(configuration, certificateValidationComponentId, clientInfo, recipientSession, outgoingRequestProxyProvider, remoteForestPipelineFactory, xsoFactory, upstreamTracer).Then(new OrganizationalToPrivatePhotoHandlerTransition(upstreamTracer)).Then(new PrivatePhotoHandler(configuration, xsoFactory, upstreamTracer)).Then(new TransparentImagePhotoHandler(configuration, upstreamTracer));
		}

		public PhotoResponse Retrieve(PhotoRequest request, Stream outputStream)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			ArgumentValidator.ThrowIfNull("outputStream", outputStream);
			return this.pipeline.Retrieve(request, new PhotoResponse(outputStream));
		}

		private readonly IPhotoHandler pipeline;
	}
}
