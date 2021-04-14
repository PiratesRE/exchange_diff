using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PreviewPhotoUploadHandler : IPhotoUploadHandler
	{
		public PreviewPhotoUploadHandler(IMailboxSession session, IMailboxPhotoReader reader, IMailboxPhotoWriter writer, IPhotoEditor editor, ITracer upstreamTracer)
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
			if (editor == null)
			{
				throw new ArgumentNullException("editor");
			}
			if (upstreamTracer == null)
			{
				throw new ArgumentNullException("upstreamTracer");
			}
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			this.session = session;
			this.reader = reader;
			this.writer = writer;
			this.editor = editor;
		}

		public PhotoResponse Upload(PhotoRequest request, PhotoResponse response)
		{
			switch (request.UploadCommand)
			{
			case UploadCommand.Upload:
				if (request.Preview)
				{
					return this.UploadPreview(request, response);
				}
				return this.LoadPreview(request, response);
			case UploadCommand.Clear:
				return this.ClearPreview(request, response);
			default:
				return response;
			}
		}

		public IPhotoUploadHandler Then(IPhotoUploadHandler next)
		{
			return new CompositePhotoUploadHandler(this, next);
		}

		private PhotoResponse UploadPreview(PhotoRequest request, PhotoResponse response)
		{
			if (request.RawUploadedPhoto == null || request.RawUploadedPhoto.Length == 0L)
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Preview photo upload handler: skipped because no photo was uploaded in the request.");
				return response;
			}
			request.RawUploadedPhoto.Seek(0L, SeekOrigin.Begin);
			int num = PhotoThumbprinter.Default.Compute(request.RawUploadedPhoto);
			this.tracer.TraceDebug<string, int>((long)this.GetHashCode(), "Preview photo upload handler: uploading preview photo of {0}.  Its thumbprint is {1:X8}.", request.TargetPrimarySmtpAddress, num);
			try
			{
				this.writer.UploadPreview(num, this.CropAndScaleRawPhoto(request.RawUploadedPhoto));
				response.PreviewUploadHandlerProcessed = true;
			}
			catch (StoragePermanentException arg)
			{
				this.tracer.TraceError<StoragePermanentException>((long)this.GetHashCode(), "Preview photo upload handler: hit a permanent storage exception uploading photo to mailbox.  Exception: {0}", arg);
				throw;
			}
			catch (StorageTransientException arg2)
			{
				this.tracer.TraceError<StorageTransientException>((long)this.GetHashCode(), "Preview photo upload handler: hit a transient storage exception uploading photo to mailbox.  Exception: {0}", arg2);
				throw;
			}
			return response;
		}

		private PhotoResponse LoadPreview(PhotoRequest request, PhotoResponse response)
		{
			Dictionary<UserPhotoSize, byte[]> dictionary = new Dictionary<UserPhotoSize, byte[]>();
			try
			{
				int num = this.reader.ReadAllPreviewSizes(this.session, dictionary);
				this.tracer.TraceDebug<string, int>((long)this.GetHashCode(), "Preview photo upload handler: read preview photo of {0} with thumbprint {1:X8}.", request.TargetPrimarySmtpAddress, num);
				response.UploadedPhotos = dictionary;
				response.Thumbprint = new int?(num);
				response.PreviewUploadHandlerProcessed = true;
			}
			catch (ObjectNotFoundException arg)
			{
				this.tracer.TraceError<ObjectNotFoundException>((long)this.GetHashCode(), "Preview photo upload handler: preview photo does NOT exist in mailbox.  Exception: {0}", arg);
				throw;
			}
			catch (StoragePermanentException arg2)
			{
				this.tracer.TraceError<StoragePermanentException>((long)this.GetHashCode(), "Preview photo upload handler: hit a permanent storage exception loading preview photo from mailbox.  Exception: {0}", arg2);
				throw;
			}
			catch (StorageTransientException arg3)
			{
				this.tracer.TraceError<StorageTransientException>((long)this.GetHashCode(), "Preview photo upload handler: hit a transient storage exception loading preview photo from mailbox.  Exception: {0}", arg3);
				throw;
			}
			return response;
		}

		private IDictionary<UserPhotoSize, byte[]> CropAndScaleRawPhoto(Stream rawPhoto)
		{
			rawPhoto.Seek(0L, SeekOrigin.Begin);
			return this.editor.CropAndScale(rawPhoto);
		}

		private PhotoResponse ClearPreview(PhotoRequest request, PhotoResponse response)
		{
			response.PreviewUploadHandlerProcessed = true;
			this.writer.ClearPreview();
			return response;
		}

		private readonly ITracer tracer = ExTraceGlobals.UserPhotosTracer;

		private readonly IMailboxSession session;

		private readonly IMailboxPhotoReader reader;

		private readonly IMailboxPhotoWriter writer;

		private readonly IPhotoEditor editor;
	}
}
