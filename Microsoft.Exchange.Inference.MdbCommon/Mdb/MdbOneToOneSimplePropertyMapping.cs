using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Mdb
{
	internal sealed class MdbOneToOneSimplePropertyMapping : MdbOneToOnePropertyMapping
	{
		internal MdbOneToOneSimplePropertyMapping(PropertyDefinition propertyDefinition, StorePropertyDefinition storePropertyDefinition) : base(propertyDefinition, storePropertyDefinition, new MdbOneToOnePropertyMapping.ItemGetterDelegate(MdbOneToOnePropertyMapping.DefaultItemGetter), new MdbOneToOnePropertyMapping.ItemSetterDelegate(MdbOneToOnePropertyMapping.DefaultItemSetter), new MdbOneToOnePropertyMapping.DictionaryGetterDelegate(MdbOneToOnePropertyMapping.DefaultDictionaryGetter), new MdbOneToOnePropertyMapping.DictionarySetterDelegate(MdbOneToOnePropertyMapping.DefaultDictionarySetter))
		{
			if (propertyDefinition.Type != storePropertyDefinition.Type)
			{
				throw new ArgumentException(string.Format("Types of generic property doesn't match with underlying property for mapping {0}", propertyDefinition.Name));
			}
		}
	}
}
