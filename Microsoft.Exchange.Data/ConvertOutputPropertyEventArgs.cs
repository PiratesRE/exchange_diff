using System;

namespace Microsoft.Exchange.Data
{
	internal class ConvertOutputPropertyEventArgs : EventArgs
	{
		public ConvertOutputPropertyEventArgs(object value, ConfigurableObject configurableObject, PropertyDefinition property, string propertyInStr)
		{
			this.Value = value;
			this.ConfigurableObject = configurableObject;
			this.Property = property;
			this.PropertyInStr = propertyInStr;
		}

		public object Value { get; set; }

		public PropertyDefinition Property { get; set; }

		public string PropertyInStr { get; set; }

		public ConfigurableObject ConfigurableObject { get; set; }
	}
}
