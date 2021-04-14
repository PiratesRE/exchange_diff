using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public sealed class ItemAttachment : Attachment<ItemAttachmentSchema>, IItemAttachment, IAttachment, IEntity, IPropertyChangeTracker<PropertyDefinition>
	{
		public IItem Item
		{
			get
			{
				return base.GetPropertyValueOrDefault<IItem>(base.Schema.ItemProperty);
			}
			set
			{
				base.SetPropertyValue<IItem>(base.Schema.ItemProperty, value);
			}
		}

		public new static class Accessors
		{
			public static readonly EntityPropertyAccessor<ItemAttachment, IItem> Item = new EntityPropertyAccessor<ItemAttachment, IItem>(SchematizedObject<ItemAttachmentSchema>.SchemaInstance.ItemProperty, (ItemAttachment itemAttachment) => itemAttachment.Item, delegate(ItemAttachment itemAttachment, IItem item)
			{
				itemAttachment.Item = item;
			});
		}
	}
}
