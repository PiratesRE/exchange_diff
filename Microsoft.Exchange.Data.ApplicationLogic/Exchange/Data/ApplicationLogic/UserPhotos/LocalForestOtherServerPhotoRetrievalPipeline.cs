using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LocalForestOtherServerPhotoRetrievalPipeline : IPhotoHandler
	{
		public LocalForestOtherServerPhotoRetrievalPipeline(PhotosConfiguration configuration, string certificateValidationComponentId, IPhotoServiceLocator serviceLocator, IRecipientSession recipientSession, IPhotoRequestOutboundWebProxyProvider outgoingRequestProxyProvider, ITracer upstreamTracer)
		{
			this.pipeline = new HttpPhotoHandler(configuration, this.CreateOutboundSender(certificateValidationComponentId, upstreamTracer), serviceLocator, outgoingRequestProxyProvider, upstreamTracer).Then(new ADPhotoHandler(new ADPhotoReader(upstreamTracer), recipientSession, upstreamTracer));
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			ArgumentValidator.ThrowIfNull("response", response);
			return this.pipeline.Retrieve(request, response);
		}

		public IPhotoHandler Then(IPhotoHandler next)
		{
			throw new NotImplementedException();
		}

		private IPhotoRequestOutboundSender CreateOutboundSender(string certificateValidationComponentId, ITracer tracer)
		{
			return new PhotoRequestOutboundSender(this.CreateOutboundAuthenticator(certificateValidationComponentId, tracer));
		}

		private IPhotoRequestOutboundAuthenticator CreateOutboundAuthenticator(string certificateValidationComponentId, ITracer tracer)
		{
			return new LocalForestOtherServerOutboundAuthenticator(certificateValidationComponentId, tracer);
		}

		private readonly IPhotoHandler pipeline;
	}
}
