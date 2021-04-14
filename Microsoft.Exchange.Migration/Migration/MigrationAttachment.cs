using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationAttachment : DisposeTrackableBase, IMigrationAttachment, IDisposable
	{
		internal MigrationAttachment(StreamAttachment attachment, PropertyOpenMode openMode) : this(attachment.GetContentStream(openMode), attachment.LastModifiedTime, null)
		{
			this.attachment = attachment;
			this.size = attachment.Size;
			this.readOnly = (openMode == PropertyOpenMode.ReadOnly);
		}

		protected MigrationAttachment(Stream stream, ExDateTime lastModifiedTime, string id)
		{
			MigrationUtil.ThrowOnNullArgument(stream, "stream");
			this.stream = stream;
			this.lastModifiedTime = lastModifiedTime;
			this.size = 0L;
			this.Id = id;
		}

		public ExDateTime LastModifiedTime
		{
			get
			{
				return this.lastModifiedTime;
			}
		}

		public long Size
		{
			get
			{
				return this.size;
			}
		}

		public string Id { get; private set; }

		public Stream Stream
		{
			get
			{
				base.CheckDisposed();
				return this.stream;
			}
		}

		public virtual void Save(string contentId)
		{
			base.CheckDisposed();
			if (!string.IsNullOrEmpty(contentId))
			{
				this.attachment.ContentId = contentId;
				this.attachment.IsInline = true;
			}
			this.stream.Dispose();
			this.stream = null;
			this.attachment.Save();
			this.attachment.Load(MigrationStoreObject.IdPropertyDefinition);
			this.Id = this.attachment.Id.ToBase64String();
			this.lastModifiedTime = this.attachment.LastModifiedTime;
			this.size = this.attachment.Size;
			this.stream = this.attachment.GetContentStream(PropertyOpenMode.Modify);
			MigrationLogger.Log(MigrationEventType.Information, "Saved attachment with id {0}", new object[]
			{
				this.Id
			});
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.stream != null)
				{
					this.stream.Dispose();
					this.stream = null;
				}
				if (this.attachment != null)
				{
					this.attachment.Dispose();
					this.attachment = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MigrationAttachment>(this);
		}

		private StreamAttachment attachment;

		private Stream stream;

		private ExDateTime lastModifiedTime;

		private long size;

		private readonly bool readOnly;
	}
}
