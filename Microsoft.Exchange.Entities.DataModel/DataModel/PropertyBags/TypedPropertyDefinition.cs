using System;

namespace Microsoft.Exchange.Entities.DataModel.PropertyBags
{
	public class TypedPropertyDefinition<TValue> : PropertyDefinition
	{
		public TypedPropertyDefinition(string name, TValue defaultValue = default(TValue), bool isLoggable = true) : base(name, typeof(TValue), isLoggable)
		{
			this.DefaultValue = defaultValue;
		}

		public TValue DefaultValue { get; private set; }
	}
}
