using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Mdb
{
	internal abstract class MdbPropertyMapping : ProviderPropertyMapping<StorePropertyDefinition, IItem, IMdbPropertyMappingContext>
	{
		protected MdbPropertyMapping(PropertyDefinition propertyDefinition, StorePropertyDefinition[] providerPropertyDefinitions) : base(propertyDefinition, providerPropertyDefinitions)
		{
		}

		public StorePropertyDefinition[] StorePropertyDefinitions
		{
			get
			{
				return this.ProviderSpecificPropertyDefinitions;
			}
		}
	}
}
