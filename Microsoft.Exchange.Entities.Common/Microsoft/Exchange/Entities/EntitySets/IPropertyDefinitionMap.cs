using System;
using System.Reflection;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Entities.EntitySets
{
	public interface IPropertyDefinitionMap
	{
		bool TryGetPropertyDefinition(PropertyInfo propertyInfo, out PropertyDefinition propertyDefinition);
	}
}
