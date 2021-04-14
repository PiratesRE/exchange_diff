using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ItemAttachment : Attachment, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag
	{
		internal ItemAttachment(CoreAttachment coreAttachment) : base(coreAttachment)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ItemAttachment>(this);
		}

		public Item GetItem()
		{
			base.CheckDisposed("GetItem");
			return this.GetItem(base.CalculateOpenMode(), false, null);
		}

		public Item GetItem(ICollection<PropertyDefinition> propsToLoad)
		{
			base.CheckDisposed("GetItem");
			return this.GetItem(base.CalculateOpenMode(), false, propsToLoad);
		}

		public MessageItem GetItemAsMessage()
		{
			return this.GetItemAsMessage(null);
		}

		public MessageItem GetItemAsMessage(params PropertyDefinition[] propsToLoad)
		{
			return this.GetItemAsMessage((ICollection<PropertyDefinition>)propsToLoad);
		}

		public MessageItem GetItemAsMessage(ICollection<PropertyDefinition> propsToLoad)
		{
			base.CheckDisposed("GetItemAsMessage");
			return (MessageItem)this.GetItem(base.CalculateOpenMode(), true, propsToLoad);
		}

		public Item GetItemAsReadOnly(ICollection<PropertyDefinition> propsToLoad)
		{
			return this.GetItem(PropertyOpenMode.ReadOnly, false, propsToLoad);
		}

		private Item GetItem(PropertyOpenMode openMode, bool bindAsMessage, ICollection<PropertyDefinition> propsToLoad)
		{
			base.CheckDisposed("GetItem");
			if (this.IsItemOpen)
			{
				throw new InvalidOperationException("Embedded item is already open, close/dispose it before calling GetItem");
			}
			ICoreItem coreItem = null;
			Item item = null;
			bool flag = false;
			try
			{
				bool noMessageDecoding = base.CoreAttachment.ParentCollection.ContainerItem.CharsetDetector.NoMessageDecoding;
				coreItem = base.CoreAttachment.PropertyBag.OpenAttachedItem(openMode, propsToLoad, noMessageDecoding);
				coreItem.TopLevelItem = (base.CoreAttachment.ParentCollection.ContainerItem.TopLevelItem ?? base.CoreAttachment.ParentCollection.ContainerItem);
				if (bindAsMessage)
				{
					item = new MessageItem(coreItem, false);
				}
				else
				{
					item = Microsoft.Exchange.Data.Storage.Item.InternalBindCoreItem(coreItem);
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					if (item != null)
					{
						item.Dispose();
					}
					if (coreItem != null)
					{
						coreItem.Dispose();
					}
				}
			}
			this.item = item;
			return item;
		}

		internal bool IsItemOpen
		{
			get
			{
				return this.item != null && !this.item.IsDisposed;
			}
		}

		protected override Schema Schema
		{
			get
			{
				return ItemAttachmentSchema.Instance;
			}
		}

		public override AttachmentType AttachmentType
		{
			get
			{
				base.CheckDisposed("AttachmentType::get");
				return AttachmentType.EmbeddedMessage;
			}
		}

		protected override void OnBeforeSave()
		{
			base.OnBeforeSave();
			base.PropertyBag[InternalSchema.AttachMethod] = 5;
		}

		internal const int AttachMethod = 5;

		private Item item;
	}
}
