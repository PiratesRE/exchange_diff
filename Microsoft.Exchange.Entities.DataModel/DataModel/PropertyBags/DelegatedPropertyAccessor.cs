using System;

namespace Microsoft.Exchange.Entities.DataModel.PropertyBags
{
	public class DelegatedPropertyAccessor<TContainer, TValue> : PropertyAccessor<TContainer, TValue>
	{
		public DelegatedPropertyAccessor(DelegatedPropertyAccessor<TContainer, TValue>.TryGetValueFunc getterDelegate, Action<TContainer, TValue> setterDelegate = null) : base(setterDelegate == null)
		{
			this.getterDelegate = getterDelegate;
			this.setterDelegate = setterDelegate;
		}

		protected override bool PerformTryGetValue(TContainer container, out TValue value)
		{
			return this.getterDelegate(container, out value);
		}

		protected override void PerformSet(TContainer container, TValue value)
		{
			this.setterDelegate(container, value);
		}

		private readonly DelegatedPropertyAccessor<TContainer, TValue>.TryGetValueFunc getterDelegate;

		private readonly Action<TContainer, TValue> setterDelegate;

		public delegate bool TryGetValueFunc(TContainer container, out TValue value);
	}
}
