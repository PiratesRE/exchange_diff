using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Entities.EntitySets.Linq.ExtensionMethods
{
	public static class PropertyValueExtensions
	{
		internal static QueryFilter AsBooleanQueryFilter(this object propertyValue)
		{
			if (!(propertyValue is bool))
			{
				return null;
			}
			if ((bool)propertyValue)
			{
				return new TrueFilter();
			}
			return new FalseFilter();
		}
	}
}
