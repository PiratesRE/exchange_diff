using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Entities.DataModel.PropertyBags
{
	public interface IPropertyValueCollectionAccessor<in TContainer, TProperty, TValue> : IPropertyAccessor<TContainer, TValue>
	{
		bool TryGetValue(IDictionary<TProperty, int> propertyIndices, IList values, out TValue value);
	}
}
