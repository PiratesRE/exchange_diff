using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReadonlyMemoryPropertyBag : MemoryPropertyBag
	{
		public ReadonlyMemoryPropertyBag(IList<StorePropertyDefinition> propertyDefs, object[] propertyValues)
		{
			base.PreLoadStoreProperty<StorePropertyDefinition>(propertyDefs, propertyValues);
			this.locked = !this.locked;
		}

		protected override void DeleteStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			if (this.locked)
			{
				throw new InvalidOperationException("Can't modify ReadonlyPropertyBag");
			}
			base.DeleteStoreProperty(propertyDefinition);
		}

		protected override void SetValidatedStoreProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
		{
			if (this.locked)
			{
				throw new InvalidOperationException("Can't modify ReadonlyPropertyBag");
			}
			base.SetValidatedStoreProperty(propertyDefinition, propertyValue);
		}

		public override void Load(ICollection<PropertyDefinition> properties)
		{
			if (this.locked)
			{
				throw new InvalidOperationException("Can't modify ReadonlyPropertyBag");
			}
			base.Load(properties);
		}

		private readonly bool locked;
	}
}
