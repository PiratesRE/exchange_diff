using System;
using System.IO;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoManagementRetrievalPipeline
	{
		public PhotoManagementRetrievalPipeline(PhotosConfiguration configuration, IMailboxSession mailboxSession, IRecipientSession recipientSession, ITracer upstreamTracer)
		{
			MailboxPhotoReader reader = new MailboxPhotoReader(upstreamTracer);
			MailboxPhotoWriter writer = new MailboxPhotoWriter(mailboxSession, upstreamTracer);
			ADPhotoReader reader2 = new ADPhotoReader(upstreamTracer);
			ADPhotoWriter writer2 = new ADPhotoWriter(recipientSession, upstreamTracer);
			this.pipeline = new MailboxPhotoUploadHandler(mailboxSession, reader, writer, upstreamTracer).Then(new ADPhotoUploadHandler(recipientSession, configuration, reader2, writer2, upstreamTracer));
		}

		public PhotoResponse Retrieve(PhotoRequest request, Stream outputStream)
		{
			return this.pipeline.Retrieve(request, new PhotoResponse(outputStream));
		}

		private readonly IPhotoHandler pipeline;
	}
}
