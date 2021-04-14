using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class AttachmentHierarchy : IDisposable
	{
		public void Dispose()
		{
			if (!this.isDisposed)
			{
				for (int i = this.Count - 1; i >= 0; i--)
				{
					this.items[i].Dispose();
				}
				if (!this.rootItemFromExternal && this.rootItem != null)
				{
					this.rootItem.Dispose();
				}
				this.isDisposed = true;
			}
		}

		private static Attachment FindAttachmentInAttachmentCollection(AttachmentCollection attachments, AttachmentId attachmentId)
		{
			if (attachments.Contains(attachmentId))
			{
				return attachments.Open(attachmentId);
			}
			throw new InvalidStoreIdException(CoreResources.IDs.ErrorInvalidAttachmentId);
		}

		public AttachmentHierarchy(Item rootItem)
		{
			this.rootItem = rootItem;
			this.rootItemFromExternal = true;
		}

		public AttachmentHierarchy(IdAndSession idAndSession, bool openAsReadWrite, bool clientSupportsIrm)
		{
			bool flag = false;
			try
			{
				bool flag2 = IrmUtils.IsIrmEnabled(clientSupportsIrm, idAndSession.Session);
				this.rootItem = idAndSession.GetRootXsoItem(null);
				StoreSession session = idAndSession.Session;
				if (openAsReadWrite)
				{
					this.rootItem.OpenAsReadWrite();
				}
				Item xsoItem = this.rootItem;
				if (flag2)
				{
					RightsManagedMessageItem rightsManagedMessageItem = xsoItem as RightsManagedMessageItem;
					if (rightsManagedMessageItem != null)
					{
						IrmUtils.DecryptForGetAttachment(session, rightsManagedMessageItem);
					}
				}
				for (int i = 0; i < idAndSession.AttachmentIds.Count; i++)
				{
					AttachmentCollection effectiveAttachmentCollection = Util.GetEffectiveAttachmentCollection(xsoItem, false);
					Attachment attachment = AttachmentHierarchy.FindAttachmentInAttachmentCollection(effectiveAttachmentCollection, idAndSession.AttachmentIds[i]);
					AttachmentHierarchyItem attachmentHierarchyItem = new AttachmentHierarchyItem(attachment, effectiveAttachmentCollection);
					this.items.Add(attachmentHierarchyItem);
					if (i < idAndSession.AttachmentIds.Count - 1)
					{
						if (!(attachment is ItemAttachment))
						{
							throw new InvalidStoreIdException(CoreResources.IDs.ErrorInvalidAttachmentId);
						}
						xsoItem = attachmentHierarchyItem.XsoItem;
					}
					if (flag2)
					{
						RightsManagedMessageItem rightsManagedMessageItem = xsoItem as RightsManagedMessageItem;
						if (rightsManagedMessageItem != null)
						{
							IrmUtils.DecryptForGetAttachment(session, rightsManagedMessageItem);
						}
					}
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
		}

		public void DeleteLast()
		{
			AttachmentHierarchyItem attachmentHierarchyItem = this[this.Count - 1];
			attachmentHierarchyItem.Delete();
			this.items.Remove(attachmentHierarchyItem);
			attachmentHierarchyItem.Dispose();
		}

		public void SaveAll()
		{
			for (int i = this.Count - 1; i >= 0; i--)
			{
				this.items[i].Save();
				this.items[i].Dispose();
			}
			this.rootItem.Save(SaveMode.NoConflictResolution);
		}

		public Item LastAsXsoItem
		{
			get
			{
				if (this.Count == 0)
				{
					return this.rootItem;
				}
				Item xsoItem = this[this.Count - 1].XsoItem;
				RightsManagedMessageItem rightsManagedMessageItem = xsoItem as RightsManagedMessageItem;
				if (rightsManagedMessageItem != null)
				{
					IrmUtils.DecryptForGetAttachment(this.rootItem.Session, rightsManagedMessageItem);
					return rightsManagedMessageItem;
				}
				return xsoItem;
			}
		}

		public AttachmentHierarchyItem Last
		{
			get
			{
				if (this.Count > 0)
				{
					return this[this.Count - 1];
				}
				return null;
			}
		}

		public AttachmentHierarchyItem this[int index]
		{
			get
			{
				return this.items[index];
			}
		}

		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		public Item RootItem
		{
			get
			{
				return this.rootItem;
			}
		}

		private readonly bool rootItemFromExternal;

		private bool isDisposed;

		private Item rootItem;

		private List<AttachmentHierarchyItem> items = new List<AttachmentHierarchyItem>();
	}
}
