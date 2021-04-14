using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoEmailBodyContent14Property : XsoEmailBodyContentProperty, IContent14Property, IContentProperty, IMIMEDataProperty, IMIMERelatedProperty, IProperty
	{
		public XsoEmailBodyContent14Property(PropertyType propertyType = PropertyType.ReadOnly) : base(propertyType)
		{
		}

		public string Preview
		{
			get
			{
				Item item = base.XsoItem as Item;
				if (item == null)
				{
					return null;
				}
				if (BodyConversionUtilities.IsMessageRestrictedAndDecoded(item))
				{
					return ((RightsManagedMessageItem)item).ProtectedBody.PreviewText;
				}
				return item.Body.PreviewText;
			}
		}
	}
}
