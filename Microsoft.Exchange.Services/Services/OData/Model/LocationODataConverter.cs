using System;
using System.Collections.Generic;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class LocationODataConverter : IODataPropertyValueConverter
	{
		public object FromODataPropertyValue(object odataPropertyValue)
		{
			if (odataPropertyValue == null)
			{
				return null;
			}
			ODataComplexValue odataComplexValue = (ODataComplexValue)odataPropertyValue;
			return new Location
			{
				DisplayName = odataComplexValue.GetPropertyValue("DisplayName", null)
			};
		}

		public object ToODataPropertyValue(object rawValue)
		{
			if (rawValue == null)
			{
				return null;
			}
			Location location = (Location)rawValue;
			ODataComplexValue odataComplexValue = new ODataComplexValue();
			odataComplexValue.TypeName = typeof(Location).FullName;
			List<ODataProperty> properties = new List<ODataProperty>
			{
				new ODataProperty
				{
					Name = "DisplayName",
					Value = location.DisplayName
				}
			};
			odataComplexValue.Properties = properties;
			return odataComplexValue;
		}
	}
}
