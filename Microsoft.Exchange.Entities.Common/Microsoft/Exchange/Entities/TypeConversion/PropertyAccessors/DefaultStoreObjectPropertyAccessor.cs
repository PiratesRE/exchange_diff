using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors
{
	internal class DefaultStoreObjectPropertyAccessor<TStoreObject, TValue> : DefaultStoragePropertyAccessor<TStoreObject, TValue> where TStoreObject : IStoreObject
	{
		public DefaultStoreObjectPropertyAccessor(uint locationIdentifier, StorePropertyDefinition property, bool forceReadonly = false) : base(property, forceReadonly)
		{
			this.locationIdentifier = locationIdentifier;
		}

		protected override void PerformSet(TStoreObject container, TValue value)
		{
			LocationIdentifierHelper locationIdentifierHelperInstance = container.LocationIdentifierHelperInstance;
			if (locationIdentifierHelperInstance != null)
			{
				locationIdentifierHelperInstance.SetLocationIdentifier(this.locationIdentifier);
			}
			base.PerformSet(container, value);
		}

		private readonly uint locationIdentifier;
	}
}
