using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.OData.Model;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal static class ODataObjectModelConverter
	{
		public static ODataEntry ConvertToODataEntry(Entity element, Uri entryUri)
		{
			ArgumentValidator.ThrowIfNull("element", element);
			string etag = null;
			object arg = null;
			if (element.PropertyBag.TryGetValue(ItemSchema.ChangeKey, out arg))
			{
				etag = string.Format("W/\"{0}\"", arg);
			}
			ODataEntry odataEntry = new ODataEntry();
			odataEntry.EditLink = entryUri;
			odataEntry.ReadLink = entryUri;
			odataEntry.Id = entryUri;
			odataEntry.ETag = etag;
			odataEntry.TypeName = element.GetType().FullName;
			odataEntry.Properties = from p in element.PropertyBag.GetProperties()
			where !p.Flags.HasFlag(PropertyDefinitionFlags.Navigation)
			select ODataObjectModelConverter.ConvertToODataProperty(p, element[p]);
			return odataEntry;
		}

		public static ODataProperty ConvertToODataProperty(PropertyDefinition propertyDefinition, object clrValue)
		{
			ArgumentValidator.ThrowIfNull("propertyDefinition", propertyDefinition);
			string name = propertyDefinition.Name;
			Type type = propertyDefinition.Type;
			object obj = clrValue;
			if (propertyDefinition.ODataPropertyValueConverter != null)
			{
				obj = propertyDefinition.ODataPropertyValueConverter.ToODataPropertyValue(clrValue);
			}
			if (type.IsEnum && obj != null)
			{
				obj = EnumConverter.ToODataEnumValue((Enum)obj);
			}
			return new ODataProperty
			{
				Name = name,
				Value = obj
			};
		}

		public static object ConvertFromPropertyValue(PropertyDefinition entityProperty, object odataPropertyValue)
		{
			ArgumentValidator.ThrowIfNull("entityProperty", entityProperty);
			Type type = entityProperty.Type;
			object result = odataPropertyValue;
			if (entityProperty.ODataPropertyValueConverter != null)
			{
				result = entityProperty.ODataPropertyValueConverter.FromODataPropertyValue(odataPropertyValue);
			}
			if (type.IsEnum)
			{
				result = EnumConverter.FromODataEnumValue(type, (ODataEnumValue)odataPropertyValue);
			}
			return result;
		}
	}
}
