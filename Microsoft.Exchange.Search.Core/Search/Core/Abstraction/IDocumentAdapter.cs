using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IDocumentAdapter
	{
		bool ContainsPropertyMapping(PropertyDefinition propertyDefinition);

		bool TryGetProperty(PropertyDefinition propertyDefinition, out object value);

		void SetProperty(PropertyDefinition propertyDefinition, object value);
	}
}
