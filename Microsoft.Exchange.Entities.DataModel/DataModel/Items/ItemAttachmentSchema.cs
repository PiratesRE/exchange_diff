using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public sealed class ItemAttachmentSchema : AttachmentSchema
	{
		public ItemAttachmentSchema()
		{
			base.RegisterPropertyDefinition(ItemAttachmentSchema.StaticItemProperty);
		}

		public TypedPropertyDefinition<IItem> ItemProperty
		{
			get
			{
				return ItemAttachmentSchema.StaticItemProperty;
			}
		}

		private static readonly TypedPropertyDefinition<IItem> StaticItemProperty = new TypedPropertyDefinition<IItem>("ItemAttachment.Item", null, false);
	}
}
