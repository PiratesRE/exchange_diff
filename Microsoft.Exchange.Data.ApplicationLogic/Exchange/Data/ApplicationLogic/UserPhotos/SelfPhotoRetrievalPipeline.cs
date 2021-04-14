using System;
using System.IO;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SelfPhotoRetrievalPipeline : IPhotoHandler
	{
		public SelfPhotoRetrievalPipeline(PhotosConfiguration configuration, string clientInfo, IRecipientSession recipientSession, IXSOFactory xsoFactory, ITracer upstreamTracer)
		{
			this.pipeline = new MailboxPhotoHandler(configuration, clientInfo, new MailboxPhotoReader(upstreamTracer), recipientSession, upstreamTracer, xsoFactory).Then(new ADPhotoHandler(new ADPhotoReader(upstreamTracer), recipientSession, upstreamTracer));
		}

		public PhotoResponse Retrieve(PhotoRequest request, Stream outputStream)
		{
			return this.pipeline.Retrieve(request, new PhotoResponse(outputStream));
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
