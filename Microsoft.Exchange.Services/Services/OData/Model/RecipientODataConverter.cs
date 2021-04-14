using System;
using System.Collections.Generic;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class RecipientODataConverter : IODataPropertyValueConverter
	{
		public object FromODataPropertyValue(object odataPropertyValue)
		{
			return RecipientODataConverter.ODataValueToRecipient((ODataComplexValue)odataPropertyValue);
		}

		public object ToODataPropertyValue(object rawValue)
		{
			return RecipientODataConverter.RecipientToODataValue((Recipient)rawValue);
		}

		internal static ODataValue RecipientToODataValue(Recipient recipient)
		{
			if (recipient == null)
			{
				return null;
			}
			ODataComplexValue odataComplexValue = new ODataComplexValue();
			odataComplexValue.TypeName = typeof(Recipient).FullName;
			List<ODataProperty> properties = new List<ODataProperty>
			{
				new ODataProperty
				{
					Name = "Name",
					Value = recipient.Name
				},
				new ODataProperty
				{
					Name = "Address",
					Value = recipient.Address
				}
			};
			odataComplexValue.Properties = properties;
			return odataComplexValue;
		}

		internal static Recipient ODataValueToRecipient(ODataComplexValue complexValue)
		{
			if (complexValue == null)
			{
				return null;
			}
			return new Recipient
			{
				Name = complexValue.GetPropertyValue("Name", null),
				Address = complexValue.GetPropertyValue("Address", null)
			};
		}
	}
}
