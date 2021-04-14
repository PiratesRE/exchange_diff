using System;
using System.IO;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OrganizationalPhotoRetrievalPipeline : IPhotoHandler
	{
		public OrganizationalPhotoRetrievalPipeline(PhotosConfiguration configuration, string certificateValidationComponentId, string clientInfo, IRecipientSession recipientSession, IPhotoRequestOutboundWebProxyProvider outgoingRequestProxyProvider, IRemoteForestPhotoRetrievalPipelineFactory remoteForestPipelineFactory, IXSOFactory xsoFactory, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			ArgumentValidator.ThrowIfNullOrEmpty("certificateValidationComponentId", certificateValidationComponentId);
			ArgumentValidator.ThrowIfNullOrEmpty("clientInfo", clientInfo);
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			ArgumentValidator.ThrowIfNull("outgoingRequestProxyProvider", outgoingRequestProxyProvider);
			ArgumentValidator.ThrowIfNull("remoteForestPipelineFactory", remoteForestPipelineFactory);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.router = new PhotoRequestRouter(configuration, certificateValidationComponentId, clientInfo, recipientSession, new PhotoServiceLocatorFactory(tracer), outgoingRequestProxyProvider, remoteForestPipelineFactory, xsoFactory, tracer);
		}

		public PhotoResponse Retrieve(PhotoRequest request, Stream outputStream)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			ArgumentValidator.ThrowIfNull("outputStream", outputStream);
			return this.router.Route(request).Retrieve(request, new PhotoResponse(outputStream));
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			ArgumentValidator.ThrowIfNull("response", response);
			return this.router.Route(request).Retrieve(request, response);
		}

		public IPhotoHandler Then(IPhotoHandler next)
		{
			return new CompositePhotoHandler(this, next);
		}

		private readonly PhotoRequestRouter router;
	}
}
