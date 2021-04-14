using System;

namespace Microsoft.Exchange.Entities.DataModel.PropertyBags
{
	public abstract class EntityPropertyAccessorBase<TEntity, TValue> : PropertyAccessor<TEntity, TValue>
	{
		protected EntityPropertyAccessorBase(TypedPropertyDefinition<TValue> propertyDefinition, Func<TEntity, TValue> getterDelegate, Action<TEntity, TValue> setterDelegate) : base(false)
		{
			this.PropertyDefinition = propertyDefinition;
			this.getterDelegate = getterDelegate;
			this.setterDelegate = setterDelegate;
		}

		public TypedPropertyDefinition<TValue> PropertyDefinition { get; private set; }

		protected abstract bool IsPropertySet(TEntity container);

		protected override void PerformSet(TEntity container, TValue value)
		{
			this.setterDelegate(container, value);
		}

		protected override bool PerformTryGetValue(TEntity container, out TValue value)
		{
			if (this.IsPropertySet(container))
			{
				value = this.getterDelegate(container);
				return true;
			}
			value = this.PropertyDefinition.DefaultValue;
			return false;
		}

		private readonly Func<TEntity, TValue> getterDelegate;

		private readonly Action<TEntity, TValue> setterDelegate;
	}
}
