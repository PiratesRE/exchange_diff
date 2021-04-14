using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Common
{
	internal class QueryHelper
	{
		internal static SortBy GetSortBy(string sortString, PropertyDefinition[] allowedProperties)
		{
			TaskLogger.LogEnter();
			if (sortString == null)
			{
				TaskLogger.LogExit();
				return null;
			}
			PropertyDefinition columnDefinition = QueryHelper.MapPropertyName(sortString, allowedProperties, false);
			TaskLogger.LogExit();
			return new SortBy(columnDefinition, SortOrder.Ascending);
		}

		private static PropertyDefinition MapPropertyName(string propertyName, PropertyDefinition[] allowedProperties, bool forFilter)
		{
			foreach (PropertyDefinition propertyDefinition in allowedProperties)
			{
				if (string.Compare(propertyDefinition.Name, propertyName, true) == 0)
				{
					return propertyDefinition;
				}
			}
			if (forFilter)
			{
				throw new RecipientTaskException(Strings.ErrorInvalidFilterProperty(propertyName));
			}
			throw new RecipientTaskException(Strings.ErrorInvalidSortProperty(propertyName));
		}
	}
}
