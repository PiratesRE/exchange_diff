using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public sealed class ItemIdAttachment : Attachment<ItemIdAttachmentSchema>, IItemIdAttachment, IAttachment, IEntity, IPropertyChangeTracker<PropertyDefinition>
	{
		public string ItemToAttachId
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.ItemToAttachIdProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.ItemToAttachIdProperty, value);
			}
		}

		public new static class Accessors
		{
			public static readonly EntityPropertyAccessor<ItemIdAttachment, string> ItemToAttachId = new EntityPropertyAccessor<ItemIdAttachment, string>(SchematizedObject<ItemIdAttachmentSchema>.SchemaInstance.ItemToAttachIdProperty, (ItemIdAttachment itemIdAttachment) => itemIdAttachment.ItemToAttachId, delegate(ItemIdAttachment itemIdAttachment, string itemToAttachId)
			{
				itemIdAttachment.ItemToAttachId = itemToAttachId;
			});
		}
	}
}
