using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.OData.Model;

namespace Microsoft.Exchange.Services.OData
{
	internal static class SchemaExtensions
	{
		public static PropertyDefinition ResolveProperty(this Schema schema, string propertyName)
		{
			ArgumentValidator.ThrowIfNull("schema", schema);
			ArgumentValidator.ThrowIfNullOrEmpty("propertyName", propertyName);
			PropertyDefinition result;
			if (!schema.TryGetPropertyByName(propertyName, out result))
			{
				throw new InvalidPropertyException(propertyName);
			}
			return result;
		}
	}
}
