using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IReadOnlyPropertyBag
	{
		TPropertyValue GetProperty<TPropertyValue>(PropertyDefinition property);

		bool TryGetProperty(PropertyDefinition property, out object value);
	}
}
