using System;

namespace Microsoft.Exchange.Data
{
	internal static class ObjectOutputHelper
	{
		internal static bool TryConvertOutputProperty(object value, ConfigurableObject configurableObject, PropertyDefinition property, string propertyInStr, Delegate[] handlers, out object convertedValue)
		{
			convertedValue = null;
			bool result = false;
			object value2 = value;
			foreach (ConvertOutputPropertyDelegate convertOutputPropertyDelegate in handlers)
			{
				ConvertOutputPropertyEventArgs args = new ConvertOutputPropertyEventArgs(value2, configurableObject, property, propertyInStr);
				object obj;
				if (convertOutputPropertyDelegate(args, out obj))
				{
					value2 = obj;
					convertedValue = obj;
					result = true;
				}
			}
			return result;
		}
	}
}
