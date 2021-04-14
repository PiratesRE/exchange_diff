using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Entities.EntitySets.Linq.ExtensionMethods
{
	public static class PropertyDefinitionExtensions
	{
		internal static ComparisonFilter AsBooleanComparisonQueryFilter(this PropertyDefinition propertyDefinition)
		{
			if (propertyDefinition != null && propertyDefinition.Type == typeof(bool))
			{
				return new ComparisonFilter(ComparisonOperator.Equal, propertyDefinition, true);
			}
			return null;
		}
	}
}
