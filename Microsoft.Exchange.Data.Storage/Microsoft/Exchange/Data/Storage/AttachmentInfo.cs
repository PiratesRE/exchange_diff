using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AttachmentInfo
	{
		internal AttachmentInfo(StoreObjectId messageId, Attachment attachment)
		{
			this.fileName = attachment.FileName;
			this.fileExtension = attachment.FileExtension;
			this.displayName = attachment.DisplayName;
			this.contentType = attachment.ContentType;
			if (string.IsNullOrEmpty(this.contentType))
			{
				this.contentType = attachment.CalculatedContentType;
			}
			this.isInline = attachment.IsInline;
			this.size = attachment.Size;
			this.attachmentType = attachment.AttachmentType;
			this.messageId = messageId;
			this.attachmentId = attachment.Id;
			this.contentId = attachment.ContentId;
			this.lastModifiedTime = attachment.LastModifiedTime;
			this.contentLocation = attachment.ContentLocation;
			if (attachment.AttachmentType == AttachmentType.Stream)
			{
				StreamAttachment streamAttachment = attachment as StreamAttachment;
				if (streamAttachment != null)
				{
					this.imageThumbnail = streamAttachment.LoadAttachmentThumbnail();
					this.imageThumbnailHeight = streamAttachment.ImageThumbnailHeight;
					this.imageThumbnailWidth = streamAttachment.ImageThumbnailWidth;
					if (this.imageThumbnail != null)
					{
						this.salientRegions = streamAttachment.LoadAttachmentThumbnailSalientRegions();
					}
				}
			}
			if (attachment.AttachmentType == AttachmentType.EmbeddedMessage)
			{
				ItemAttachment itemAttachment = attachment as ItemAttachment;
				if (itemAttachment != null)
				{
					using (Item item = itemAttachment.GetItem())
					{
						this.embeddedItemClass = item.ClassName;
					}
				}
			}
			if (attachment.AttachmentType == AttachmentType.Reference)
			{
				ReferenceAttachment referenceAttachment = attachment as ReferenceAttachment;
				if (referenceAttachment != null)
				{
					this.attachLongPathName = referenceAttachment.AttachLongPathName;
					this.providerType = referenceAttachment.ProviderType;
				}
			}
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		public string FileExtension
		{
			get
			{
				return this.fileExtension;
			}
		}

		public string ContentType
		{
			get
			{
				return this.contentType;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public bool IsInline
		{
			get
			{
				return this.isInline;
			}
		}

		public long Size
		{
			get
			{
				return this.size;
			}
		}

		public byte[] ImageThumbnail
		{
			get
			{
				return this.imageThumbnail;
			}
		}

		public byte[] ImageThumbnailSalientRegions
		{
			get
			{
				return this.salientRegions;
			}
		}

		public int ImageThumbnailHeight
		{
			get
			{
				return this.imageThumbnailHeight;
			}
		}

		public int ImageThumbnailWidth
		{
			get
			{
				return this.imageThumbnailWidth;
			}
		}

		public StoreObjectId MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		public AttachmentId AttachmentId
		{
			get
			{
				return this.attachmentId;
			}
		}

		public AttachmentType AttachmentType
		{
			get
			{
				return this.attachmentType;
			}
		}

		public string ContentId
		{
			get
			{
				return this.contentId;
			}
		}

		public Uri ContentLocation
		{
			get
			{
				return this.contentLocation;
			}
		}

		public ExDateTime LastModifiedTime
		{
			get
			{
				return this.lastModifiedTime;
			}
		}

		public string EmbeddedItemClass
		{
			get
			{
				return this.embeddedItemClass;
			}
		}

		public string AttachLongPathName
		{
			get
			{
				return this.attachLongPathName;
			}
		}

		public string ProviderType
		{
			get
			{
				return this.providerType;
			}
		}

		private readonly string displayName;

		private readonly string fileExtension;

		private readonly string fileName;

		private readonly string contentType;

		private readonly long size;

		private readonly bool isInline;

		private readonly AttachmentType attachmentType;

		private readonly StoreObjectId messageId;

		private readonly AttachmentId attachmentId;

		private readonly string contentId;

		private readonly Uri contentLocation;

		private readonly ExDateTime lastModifiedTime;

		private readonly byte[] imageThumbnail;

		private readonly string embeddedItemClass;

		private readonly string attachLongPathName;

		private readonly string providerType;

		private readonly byte[] salientRegions;

		private readonly int imageThumbnailHeight;

		private readonly int imageThumbnailWidth;
	}
}
