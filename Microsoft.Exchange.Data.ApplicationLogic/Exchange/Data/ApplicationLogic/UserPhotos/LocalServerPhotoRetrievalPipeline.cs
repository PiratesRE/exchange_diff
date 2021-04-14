using System;
using System.IO;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LocalServerPhotoRetrievalPipeline : IPhotoHandler
	{
		public LocalServerPhotoRetrievalPipeline(PhotosConfiguration configuration, string clientInfo, IRecipientSession recipientSession, IXSOFactory xsoFactory, ITracer upstreamTracer)
		{
			this.pipeline = new FileSystemPhotoHandler(configuration, new FileSystemPhotoReader(upstreamTracer), upstreamTracer).Then(new MailboxPhotoHandler(configuration, clientInfo, new MailboxPhotoReader(upstreamTracer), recipientSession, upstreamTracer, xsoFactory)).Then(new ADPhotoHandler(new ADPhotoReader(upstreamTracer), recipientSession, upstreamTracer)).Then(new CachingPhotoHandler(new FileSystemPhotoWriter(upstreamTracer), configuration, upstreamTracer)).Then(new DiagnosticsPhotoHandler(upstreamTracer));
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
