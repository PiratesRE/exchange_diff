using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoResponse
	{
		public PhotoResponse(Stream outputPhotoStream)
		{
			if (outputPhotoStream == null)
			{
				throw new ArgumentNullException("outputPhotoStream");
			}
			this.OutputPhotoStream = outputPhotoStream;
			this.Status = HttpStatusCode.NotFound;
		}

		public int? Thumbprint { get; set; }

		public string ETag
		{
			get
			{
				if (!this.etagExplicitlyInitialized)
				{
					return PhotoThumbprinter.Default.FormatAsETag(this.Thumbprint);
				}
				return this.etag;
			}
			set
			{
				this.etagExplicitlyInitialized = true;
				this.etag = value;
			}
		}

		public IDictionary<UserPhotoSize, byte[]> UploadedPhotos { get; set; }

		public Stream OutputPhotoStream { get; private set; }

		public bool ServerCacheHit { get; set; }

		public bool IsPhotoFileOnFileSystem { get; set; }

		public bool Served { get; set; }

		public HttpStatusCode Status { get; set; }

		public string PhotoFullPathOnFileSystem { get; set; }

		public long ContentLength { get; set; }

		public string ContentType { get; set; }

		public bool FileSystemHandlerProcessed { get; set; }

		public bool MailboxHandlerProcessed { get; set; }

		public bool ADHandlerProcessed { get; set; }

		public bool CachingHandlerProcessed { get; set; }

		public bool PreviewUploadHandlerProcessed { get; set; }

		public bool FileSystemUploadHandlerProcessed { get; set; }

		public bool MailboxUploadHandlerProcessed { get; set; }

		public bool ADUploadHandlerProcessed { get; set; }

		public bool HttpHandlerProcessed { get; set; }

		public bool PrivateHandlerProcessed { get; set; }

		public bool TargetNotFoundHandlerProcessed { get; set; }

		public bool TransparentImageHandlerProcessed { get; set; }

		public bool OrganizationalToPrivateHandlerTransitionProcessed { get; set; }

		public bool PhotoWrittenToFileSystem { get; set; }

		public string HttpExpiresHeader { get; set; }

		public string PhotoUrl { get; set; }

		public bool RemoteForestHandlerProcessed { get; set; }

		private bool etagExplicitlyInitialized;

		private string etag;
	}
}
