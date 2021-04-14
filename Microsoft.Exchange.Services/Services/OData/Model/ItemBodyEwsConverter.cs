using System;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal static class ItemBodyEwsConverter
	{
		public static BodyContentType ToBodyContentType(this ItemBody itemBody)
		{
			if (itemBody == null)
			{
				return null;
			}
			return new BodyContentType
			{
				BodyType = EnumConverter.CastEnumType<Microsoft.Exchange.Services.Core.Types.BodyType>(itemBody.ContentType),
				Value = itemBody.Content
			};
		}

		public static ItemBody ToItemBody(this BodyContentType bodyContentType)
		{
			if (bodyContentType == null)
			{
				return null;
			}
			return new ItemBody
			{
				ContentType = EnumConverter.CastEnumType<Microsoft.Exchange.Services.OData.Model.BodyType>(bodyContentType.BodyType),
				Content = bodyContentType.Value
			};
		}

		public static ItemBody ToDataEntityItemBody(this ItemBody itemBody)
		{
			if (itemBody == null)
			{
				return null;
			}
			return new ItemBody
			{
				ContentType = EnumConverter.CastEnumType<Microsoft.Exchange.Entities.DataModel.Items.BodyType>(itemBody.ContentType),
				Content = itemBody.Content
			};
		}

		public static ItemBody ToItemBody(this ItemBody dataEntityItemBody)
		{
			if (dataEntityItemBody == null)
			{
				return null;
			}
			return new ItemBody
			{
				ContentType = EnumConverter.CastEnumType<Microsoft.Exchange.Services.OData.Model.BodyType>(dataEntityItemBody.ContentType),
				Content = dataEntityItemBody.Content
			};
		}
	}
}
