using System;
using System.Net;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxPhotoUploadHandler : IPhotoHandler, IPhotoUploadHandler
	{
		public MailboxPhotoUploadHandler(IMailboxSession session, IMailboxPhotoReader reader, IMailboxPhotoWriter writer, ITracer upstreamTracer)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (upstreamTracer == null)
			{
				throw new ArgumentNullException("upstreamTracer");
			}
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			this.session = session;
			this.reader = reader;
			this.writer = writer;
		}

		public PhotoResponse Upload(PhotoRequest request, PhotoResponse response)
		{
			if (request.Preview)
			{
				return response;
			}
			PhotoResponse result;
			try
			{
				switch (request.UploadCommand)
				{
				case UploadCommand.Upload:
					result = this.SavePhotoToMailbox(request, response);
					break;
				case UploadCommand.Clear:
					result = this.ClearPhotoFromMailbox(request, response);
					break;
				default:
					result = response;
					break;
				}
			}
			catch (ObjectNotFoundException arg)
			{
				this.tracer.TraceDebug<ObjectNotFoundException>((long)this.GetHashCode(), "Mailbox photo upload handler: photo not found.  Exception: {0}", arg);
				throw;
			}
			catch (StorageTransientException arg2)
			{
				this.tracer.TraceError<StorageTransientException>((long)this.GetHashCode(), "Mailbox photo upload handler: transient exception saving or clearing photo.  Exception: {0}", arg2);
				throw;
			}
			catch (StoragePermanentException arg3)
			{
				this.tracer.TraceError<StoragePermanentException>((long)this.GetHashCode(), "Mailbox photo upload handler: permanent exception saving or clearing photo.  Exception: {0}", arg3);
				throw;
			}
			return result;
		}

		public IPhotoUploadHandler Then(IPhotoUploadHandler next)
		{
			return new CompositePhotoUploadHandler(this, next);
		}

		public PhotoResponse Retrieve(PhotoRequest request, PhotoResponse response)
		{
			if (response.Served)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Mailbox photo upload handler: skipped because photo has already been served by an upstream handler.");
				return response;
			}
			PhotoResponse result;
			try
			{
				response.MailboxUploadHandlerProcessed = true;
				this.tracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "Mailbox photo upload handler: reading photo of user {0}.", this.session.MailboxOwner);
				this.reader.Read(this.session, request.Size, request.Preview, response.OutputPhotoStream, request.PerformanceLogger);
				response.Thumbprint = new int?(this.reader.ReadThumbprint(this.session, request.Preview));
				response.Served = true;
				response.Status = HttpStatusCode.OK;
				result = response;
			}
			catch (ObjectNotFoundException arg)
			{
				this.tracer.TraceDebug<bool, ObjectNotFoundException>((long)this.GetHashCode(), "Mailbox photo upload handler: photo not found.  Preview? {0}.  Exception: {1}", request.Preview, arg);
				result = response;
			}
			catch (StorageTransientException arg2)
			{
				this.tracer.TraceError<StorageTransientException>((long)this.GetHashCode(), "Mailbox photo upload handler: transient exception at reading photo.  Exception: {0}", arg2);
				throw;
			}
			catch (StoragePermanentException arg3)
			{
				this.tracer.TraceError<StoragePermanentException>((long)this.GetHashCode(), "Mailbox photo upload handler: permanent exception at reading photo.  Exception: {0}", arg3);
				throw;
			}
			return result;
		}

		public IPhotoHandler Then(IPhotoHandler next)
		{
			return new CompositePhotoHandler(this, next);
		}

		private PhotoResponse SavePhotoToMailbox(PhotoRequest request, PhotoResponse response)
		{
			response.MailboxUploadHandlerProcessed = true;
			this.writer.Save();
			return response;
		}

		private PhotoResponse ClearPhotoFromMailbox(PhotoRequest request, PhotoResponse response)
		{
			response.MailboxUploadHandlerProcessed = true;
			this.writer.Clear();
			return response;
		}

		private readonly ITracer tracer = ExTraceGlobals.UserPhotosTracer;

		private readonly IMailboxSession session;

		private readonly IMailboxPhotoReader reader;

		private readonly IMailboxPhotoWriter writer;
	}
}
