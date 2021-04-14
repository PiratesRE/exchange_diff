using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.TypeConversion
{
	internal interface IStorageTranslationRule<in TLeft, in TRight> : ITranslationRule<TLeft, TRight>
	{
		IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> StorageDependencies { get; }

		PropertyChangeMetadata.PropertyGroup StoragePropertyGroup { get; }

		IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition> EntityProperties { get; }
	}
}
