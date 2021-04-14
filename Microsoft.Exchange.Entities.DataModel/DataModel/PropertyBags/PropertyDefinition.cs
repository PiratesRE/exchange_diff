using System;

namespace Microsoft.Exchange.Entities.DataModel.PropertyBags
{
	public abstract class PropertyDefinition
	{
		protected PropertyDefinition(string name, Type valueType, bool isLoggable)
		{
			this.Name = name;
			this.ValueType = valueType;
			this.IsLoggable = isLoggable;
		}

		public string Name { get; private set; }

		public Type ValueType { get; private set; }

		public bool IsLoggable { get; private set; }
	}
}
