using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class AttachmentHierarchyItem : IDisposable
	{
		public AttachmentHierarchyItem(Attachment attachment, AttachmentCollection ownerCollection)
		{
			this.attachment = attachment;
			this.ownerCollection = ownerCollection;
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				if (this.xsoItem != null)
				{
					this.xsoItem.Dispose();
				}
				if (this.attachment != null)
				{
					this.attachment.Dispose();
				}
				this.isDisposed = true;
			}
		}

		public void Delete()
		{
			if (!this.isDeleted)
			{
				try
				{
					if (this.ownerCollection.IsReadOnly)
					{
						throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorDeleteItemsFailed);
					}
					this.ownerCollection.Remove(this.Attachment.Id);
					this.isDeleted = true;
				}
				finally
				{
					this.Dispose();
				}
			}
		}

		public void Save()
		{
			this.XsoItem.Save(SaveMode.NoConflictResolution);
			this.Attachment.Save();
		}

		public Attachment Attachment
		{
			get
			{
				return this.attachment;
			}
		}

		public bool IsItemAttachment
		{
			get
			{
				return this.Attachment is ItemAttachment;
			}
		}

		public Item XsoItem
		{
			get
			{
				if (this.xsoItem == null)
				{
					ItemAttachment itemAttachment = this.Attachment as ItemAttachment;
					if (itemAttachment == null)
					{
						throw new InvalidStoreIdException(CoreResources.IDs.ErrorInvalidIdNotAnItemAttachmentId);
					}
					this.xsoItem = itemAttachment.GetItem();
				}
				return this.xsoItem;
			}
		}

		private bool isDeleted;

		private bool isDisposed;

		private Attachment attachment;

		private AttachmentCollection ownerCollection;

		private Item xsoItem;
	}
}
