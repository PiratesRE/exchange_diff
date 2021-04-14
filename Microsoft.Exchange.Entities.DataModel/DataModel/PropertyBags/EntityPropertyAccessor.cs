using System;

namespace Microsoft.Exchange.Entities.DataModel.PropertyBags
{
	public class EntityPropertyAccessor<TEntity, TValue> : EntityPropertyAccessorBase<TEntity, TValue> where TEntity : IPropertyChangeTracker<PropertyDefinition>
	{
		public EntityPropertyAccessor(TypedPropertyDefinition<TValue> propertyDefinition, Func<TEntity, TValue> getterDelegate, Action<TEntity, TValue> setterDelegate) : base(propertyDefinition, getterDelegate, setterDelegate)
		{
		}

		protected override bool IsPropertySet(TEntity container)
		{
			return container.IsPropertySet(base.PropertyDefinition);
		}
	}
}
