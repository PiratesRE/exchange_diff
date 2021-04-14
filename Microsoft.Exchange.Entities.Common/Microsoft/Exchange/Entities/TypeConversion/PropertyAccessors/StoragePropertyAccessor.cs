using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors
{
	internal abstract class StoragePropertyAccessor<TStoreObject, TValue> : PropertyAccessor<TStoreObject, TValue>, IStoragePropertyAccessor<TStoreObject, TValue>, IPropertyAccessor<TStoreObject, TValue> where TStoreObject : IStorePropertyBag
	{
		protected StoragePropertyAccessor(bool readOnly, PropertyChangeMetadata.PropertyGroup propertyChangeMetadataGroup = null, IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> dependencies = null) : base(readOnly)
		{
			this.PropertyChangeMetadataGroup = propertyChangeMetadataGroup;
			this.Dependencies = (dependencies ?? ((IEnumerable<Microsoft.Exchange.Data.PropertyDefinition>)propertyChangeMetadataGroup));
		}

		public PropertyChangeMetadata.PropertyGroup PropertyChangeMetadataGroup { get; protected set; }

		public IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> Dependencies { get; private set; }
	}
}
