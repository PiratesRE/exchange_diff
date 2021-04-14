using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ItemShape : Shape
	{
		static ItemShape()
		{
			ItemShape.defaultProperties.Add(ItemSchema.ItemId);
			ItemShape.defaultProperties.Add(ItemSchema.Attachments);
			ItemShape.defaultProperties.Add(ItemSchema.ResponseObjects);
			ItemShape.defaultProperties.Add(ItemSchema.HasAttachments);
			ItemShape.defaultProperties.Add(ItemSchema.Culture);
			ItemShape.defaultProperties.Add(ItemSchema.IsAssociated);
		}

		private ItemShape() : base(Schema.Item, ItemSchema.GetSchema(), null, ItemShape.defaultProperties)
		{
		}

		internal static ItemShape CreateShape()
		{
			return new ItemShape();
		}

		private static List<PropertyInformation> defaultProperties = new List<PropertyInformation>();
	}
}
