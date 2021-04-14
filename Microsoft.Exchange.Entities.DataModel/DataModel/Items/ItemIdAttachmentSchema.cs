using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public sealed class ItemIdAttachmentSchema : AttachmentSchema
	{
		public ItemIdAttachmentSchema()
		{
			base.RegisterPropertyDefinition(ItemIdAttachmentSchema.StaticItemToAttachIdProperty);
		}

		public TypedPropertyDefinition<string> ItemToAttachIdProperty
		{
			get
			{
				return ItemIdAttachmentSchema.StaticItemToAttachIdProperty;
			}
		}

		private static readonly TypedPropertyDefinition<string> StaticItemToAttachIdProperty = new TypedPropertyDefinition<string>("ItemAttachment.ItemToAttachId", null, true);
	}
}
