using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects
{
	internal class CoreObjectPropertyDefinitionFactory : IPropertyDefinitionFactory
	{
		public CoreObjectPropertyDefinitionFactory(StoreSession session, ICorePropertyBag propertyMappingReference)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(propertyMappingReference, "propertyMappingReference");
			this.session = session;
			this.propertyMappingReference = propertyMappingReference;
		}

		public bool TryGetPropertyDefinitionsFromPropertyTags(PropertyTag[] propertyTags, out NativeStorePropertyDefinition[] propertyDefinitions)
		{
			return MEDSPropertyTranslator.TryGetPropertyDefinitionsFromPropertyTags(this.session, this.propertyMappingReference, propertyTags, out propertyDefinitions);
		}

		public bool TryGetPropertyDefinitionsFromPropertyTags(PropertyTag[] propertyTags, bool supportsCompatibleType, out NativeStorePropertyDefinition[] propertyDefinitions)
		{
			return MEDSPropertyTranslator.TryGetPropertyDefinitionsFromPropertyTags(this.session, this.propertyMappingReference, propertyTags, supportsCompatibleType, out propertyDefinitions);
		}

		private readonly StoreSession session;

		private readonly ICorePropertyBag propertyMappingReference;
	}
}
