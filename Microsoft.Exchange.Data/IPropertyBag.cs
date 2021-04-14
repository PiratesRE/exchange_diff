using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	public interface IPropertyBag : IReadOnlyPropertyBag
	{
		object this[PropertyDefinition propertyDefinition]
		{
			get;
			set;
		}

		void SetProperties(ICollection<PropertyDefinition> propertyDefinitionArray, object[] propertyValuesArray);
	}
}
