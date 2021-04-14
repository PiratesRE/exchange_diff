using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IPropertyBag : IReadOnlyPropertyBag
	{
		void SetProperty(PropertyDefinition property, object value);
	}
}
