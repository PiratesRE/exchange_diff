using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoAccountIdProperty : XsoGuidProperty
	{
		public XsoAccountIdProperty(PropertyType type) : base(MessageItemSchema.SharingInstanceGuid, type, XsoAccountIdProperty.prefetchProps)
		{
		}

		public override string StringData
		{
			get
			{
				object obj = base.XsoItem.TryGetProperty(ItemSchema.CloudId);
				if (obj is PropertyError)
				{
					return string.Empty;
				}
				return base.StringData;
			}
		}

		private static readonly PropertyDefinition[] prefetchProps = new PropertyDefinition[]
		{
			MessageItemSchema.SharingInstanceGuid,
			ItemSchema.CloudId
		};
	}
}
