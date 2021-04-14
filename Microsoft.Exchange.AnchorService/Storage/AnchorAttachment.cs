using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AnchorService.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AnchorAttachment : DisposeTrackableBase
	{
		internal AnchorAttachment(AnchorContext anchorContext, StreamAttachment attachment, PropertyOpenMode openMode) : this(anchorContext, attachment.GetContentStream(openMode), attachment.LastModifiedTime, null)
		{
			this.attachment = attachment;
		}

		protected AnchorAttachment(AnchorContext anchorContext, Stream stream, ExDateTime lastModifiedTime, string id)
		{
			AnchorUtil.ThrowOnNullArgument(anchorContext, "anchorContext");
			AnchorUtil.ThrowOnNullArgument(stream, "stream");
			this.anchorContext = anchorContext;
			this.stream = stream;
			this.lastModifiedTime = lastModifiedTime;
			this.Id = id;
		}

		public ExDateTime LastModifiedTime
		{
			get
			{
				return this.lastModifiedTime;
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
			this.attachment.Save();
			this.attachment.Load(AnchorStoreObject.IdPropertyDefinition);
			this.Id = this.attachment.Id.ToBase64String();
			this.anchorContext.Logger.Log(MigrationEventType.Information, "Saved attachment with id {0}", new object[]
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
			return DisposeTracker.Get<AnchorAttachment>(this);
		}

		private StreamAttachment attachment;

		private Stream stream;

		private ExDateTime lastModifiedTime;

		private AnchorContext anchorContext;
	}
}
