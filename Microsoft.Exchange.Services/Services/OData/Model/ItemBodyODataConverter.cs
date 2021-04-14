using System;
using System.Collections.Generic;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class ItemBodyODataConverter : IODataPropertyValueConverter
	{
		public object FromODataPropertyValue(object odataPropertyValue)
		{
			if (odataPropertyValue == null)
			{
				return null;
			}
			ODataComplexValue odataComplexValue = (ODataComplexValue)odataPropertyValue;
			return new ItemBody
			{
				ContentType = EnumConverter.FromODataEnumValue<BodyType>(odataComplexValue.GetPropertyValue("ContentType", null)),
				Content = odataComplexValue.GetPropertyValue("Content", null)
			};
		}

		public object ToODataPropertyValue(object rawValue)
		{
			if (rawValue == null)
			{
				return null;
			}
			ItemBody itemBody = (ItemBody)rawValue;
			ODataComplexValue odataComplexValue = new ODataComplexValue();
			odataComplexValue.TypeName = itemBody.GetType().FullName;
			List<ODataProperty> properties = new List<ODataProperty>
			{
				new ODataProperty
				{
					Name = "ContentType",
					Value = EnumConverter.ToODataEnumValue(itemBody.ContentType)
				},
				new ODataProperty
				{
					Name = "Content",
					Value = itemBody.Content
				}
			};
			odataComplexValue.Properties = properties;
			return odataComplexValue;
		}
	}
}
