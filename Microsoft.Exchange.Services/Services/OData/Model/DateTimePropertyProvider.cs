using System;
using System.Xml;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DateTimePropertyProvider : EwsPropertyProvider
	{
		public DateTimePropertyProvider(PropertyInformation propertyInformation) : base(propertyInformation)
		{
		}

		protected override void GetProperty(Entity entity, PropertyDefinition property, ServiceObject ewsObject)
		{
			string valueOrDefault = ewsObject.GetValueOrDefault<string>(base.PropertyInformation);
			if (!string.IsNullOrEmpty(valueOrDefault))
			{
				entity[property] = new DateTimeOffset(DateTime.Parse(valueOrDefault).ToUniversalTime());
				return;
			}
			if (property.EdmType.IsNullable)
			{
				entity[property] = null;
			}
		}

		protected override void SetProperty(Entity entity, PropertyDefinition property, ServiceObject ewsObject)
		{
			DateTimeOffset left = (DateTimeOffset)entity[property];
			if (left != DateTimeOffset.MinValue)
			{
				ewsObject[base.PropertyInformation] = XmlConvert.ToString(left.UtcDateTime, XmlDateTimeSerializationMode.Utc);
			}
		}

		public override string GetQueryConstant(object value)
		{
			if (value is DateTimeOffset)
			{
				DateTimeOffset value2 = (DateTimeOffset)value;
				return XmlConvert.ToString(value2);
			}
			return null;
		}
	}
}
