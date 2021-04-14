using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.OData.Model;

namespace Microsoft.Exchange.Services.OData
{
	internal static class PropertyProviderExtensions
	{
		public static EwsPropertyProvider GetEwsPropertyProvider(this PropertyProvider baseProvider, EntitySchema schema)
		{
			ArgumentValidator.ThrowIfNull("baseProvider", baseProvider);
			ArgumentValidator.ThrowIfNull("schema", schema);
			if (baseProvider is EwsPropertyProvider)
			{
				return baseProvider as EwsPropertyProvider;
			}
			if (baseProvider is AggregatedPropertyProvider)
			{
				AggregatedPropertyProvider aggregatedPropertyProvider = baseProvider as AggregatedPropertyProvider;
				PropertyProvider propertyProvider = aggregatedPropertyProvider.SelectProvider(schema);
				if (propertyProvider is EwsPropertyProvider)
				{
					return propertyProvider as EwsPropertyProvider;
				}
			}
			throw new InvalidOperationException("Invalid provider type.");
		}
	}
}
