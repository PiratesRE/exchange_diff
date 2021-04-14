using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	public interface IReadOnlyPropertyBag
	{
		object this[PropertyDefinition propertyDefinition]
		{
			get;
		}

		object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray);
	}
}
