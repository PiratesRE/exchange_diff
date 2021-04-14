using System;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class ItemAttachment : Attachment
	{
		public Item Item
		{
			get
			{
				return (Item)base[ItemAttachmentSchema.Item];
			}
			set
			{
				base[ItemAttachmentSchema.Item] = value;
			}
		}

		internal override EntitySchema Schema
		{
			get
			{
				return ItemAttachmentSchema.SchemaInstance;
			}
		}

		internal new static readonly EdmEntityType EdmEntityType = new EdmEntityType(typeof(ItemAttachment).Namespace, typeof(ItemAttachment).Name, Attachment.EdmEntityType);
	}
}
