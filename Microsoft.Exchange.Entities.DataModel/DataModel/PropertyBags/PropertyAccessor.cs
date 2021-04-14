using System;

namespace Microsoft.Exchange.Entities.DataModel.PropertyBags
{
	public abstract class PropertyAccessor<TContainer, TValue> : IPropertyAccessor<TContainer, TValue>
	{
		protected PropertyAccessor(bool readOnly)
		{
			this.Readonly = readOnly;
		}

		public bool Readonly { get; private set; }

		public bool TryGetValue(TContainer container, out TValue value)
		{
			return this.PerformTryGetValue(container, out value);
		}

		public void Set(TContainer container, TValue value)
		{
			this.PerformSet(container, value);
		}

		protected abstract bool PerformTryGetValue(TContainer container, out TValue value);

		protected abstract void PerformSet(TContainer container, TValue value);
	}
}
