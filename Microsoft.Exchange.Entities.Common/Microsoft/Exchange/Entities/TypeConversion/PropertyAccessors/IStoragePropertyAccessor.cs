using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors
{
	internal interface IStoragePropertyAccessor<in TStoreObject, TValue> : IPropertyAccessor<TStoreObject, TValue>
	{
		PropertyChangeMetadata.PropertyGroup PropertyChangeMetadataGroup { get; }

		IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> Dependencies { get; }
	}
}
