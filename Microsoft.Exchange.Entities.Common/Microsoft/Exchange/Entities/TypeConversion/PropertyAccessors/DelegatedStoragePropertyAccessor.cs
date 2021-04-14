using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors
{
	internal class DelegatedStoragePropertyAccessor<TStoreObject, TValue> : DelegatedPropertyAccessor<TStoreObject, TValue>, IStoragePropertyAccessor<TStoreObject, TValue>, IPropertyValueCollectionAccessor<TStoreObject, Microsoft.Exchange.Data.PropertyDefinition, TValue>, IPropertyAccessor<TStoreObject, TValue>
	{
		public DelegatedStoragePropertyAccessor(DelegatedPropertyAccessor<TStoreObject, TValue>.TryGetValueFunc getterDelegate, Action<TStoreObject, TValue> setterDelegate = null, DelegatedStoragePropertyAccessor<TStoreObject, TValue>.TryGetValueFromCollectionFunc propertyValueCollectionGetterDelegate = null, PropertyChangeMetadata.PropertyGroup propertyChangeMetadataGroup = null, params Microsoft.Exchange.Data.PropertyDefinition[] dependencies) : base(getterDelegate, setterDelegate)
		{
			this.propertyValueCollectionGetterDelegate = propertyValueCollectionGetterDelegate;
			this.PropertyChangeMetadataGroup = propertyChangeMetadataGroup;
			this.Dependencies = ((dependencies.Length == 0) ? ((IEnumerable<Microsoft.Exchange.Data.PropertyDefinition>)propertyChangeMetadataGroup) : dependencies);
		}

		public PropertyChangeMetadata.PropertyGroup PropertyChangeMetadataGroup { get; private set; }

		public IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> Dependencies { get; private set; }

		public bool TryGetValue(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, out TValue value)
		{
			value = default(TValue);
			return this.propertyValueCollectionGetterDelegate != null && this.propertyValueCollectionGetterDelegate(propertyIndices, values, out value);
		}

		private readonly DelegatedStoragePropertyAccessor<TStoreObject, TValue>.TryGetValueFromCollectionFunc propertyValueCollectionGetterDelegate;

		public delegate bool TryGetValueFromCollectionFunc(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, out TValue value);
	}
}
