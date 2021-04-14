using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects
{
	internal interface IPropertyDefinitionFactory
	{
		bool TryGetPropertyDefinitionsFromPropertyTags(PropertyTag[] propertyTags, out NativeStorePropertyDefinition[] propertyDefinitions);

		bool TryGetPropertyDefinitionsFromPropertyTags(PropertyTag[] propertyTags, bool supportsCompatibleType, out NativeStorePropertyDefinition[] propertyDefinitions);
	}
}
