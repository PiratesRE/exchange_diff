using System;

namespace Microsoft.Exchange.Entities.DataModel.PropertyBags
{
	public interface IPropertyAccessor<in TContainer, TValue>
	{
		bool Readonly { get; }

		bool TryGetValue(TContainer container, out TValue value);

		void Set(TContainer container, TValue value);
	}
}
