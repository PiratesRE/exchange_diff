using System;
using System.IO;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LocalServerFallbackToOtherServerPhotoRetrievalPipeline : IPhotoHandler
	{
		public LocalServerFallbackToOtherServerPhotoRetrievalPipeline(PhotosConfiguration configuration, string clientInfo, IRecipientSession recipientSession, IXSOFactory xsoFactory, string certificateValidationComponentId, IPhotoServiceLocatorFactory serviceLocatorFactory, IPhotoRequestOutboundWebProxyProvider outgoingRequestProxyProvider, ITracer upstreamTracer)
		{
			this.configuration = configuration;
			this.certificateValidationComponentId = certificateValidationComponentId;
			this.recipientSession = recipientSession;
			this.serviceLocatorFactory = serviceLocatorFactory;
			this.outgoingRequestProxyProvider = outgoingRequestProxyProvider;
			this.tracer = upstreamTracer;
			this.localServerPipeline = new LocalServerPhotoRetrievalPipeline(configuration, clientInfo, recipientSession, xsoFactory, upstreamTracer);
		}

		public PhotoResponse Retrieve(PhotoRequest request, Stream outputStream)
		{
			return this.Retrieve(request, new PhotoResponse(outputStream));
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			PhotoResponse result;
			try
			{
				result = this.localServerPipeline.Retrieve(request, response);
			}
			catch (WrongServerException)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "LOCAL SERVER WITH FALLBACK TO OTHER SERVER PIPELINE: target mailbox is NOT on this server.  Falling back to other server.");
				result = this.FallbackToOtherServer(request, response);
			}
			return result;
		}

		public IPhotoHandler Then(IPhotoHandler next)
		{
			throw new NotImplementedException();
		}

		private PhotoResponse FallbackToOtherServer(PhotoRequest request, PhotoResponse response)
		{
			request.PerformanceLogger.Log("WrongRoutingDetectedThenFallbackToOtherServer", string.Empty, 1U);
			return this.CreateOtherServerPipeline(request).Retrieve(request, response);
		}

		private LocalForestOtherServerPhotoRetrievalPipeline CreateOtherServerPipeline(PhotoRequest request)
		{
			return new LocalForestOtherServerPhotoRetrievalPipeline(this.configuration, this.certificateValidationComponentId, this.serviceLocatorFactory.CreateForLocalForest(request.PerformanceLogger), this.recipientSession, this.outgoingRequestProxyProvider, this.tracer);
		}

		private readonly LocalServerPhotoRetrievalPipeline localServerPipeline;

		private readonly PhotosConfiguration configuration;

		private readonly string certificateValidationComponentId;

		private readonly IRecipientSession recipientSession;

		private readonly IPhotoServiceLocatorFactory serviceLocatorFactory;

		private readonly IPhotoRequestOutboundWebProxyProvider outgoingRequestProxyProvider;

		private readonly ITracer tracer;
	}
}
