using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Mdb
{
	internal abstract class MdbOneToOnePropertyMapping : MdbPropertyMapping
	{
		protected MdbOneToOnePropertyMapping(PropertyDefinition propertyDefinition, StorePropertyDefinition storePropertyDefinition, MdbOneToOnePropertyMapping.ItemGetterDelegate itemGetter, MdbOneToOnePropertyMapping.ItemSetterDelegate itemSetter, MdbOneToOnePropertyMapping.DictionaryGetterDelegate dictionaryGetter, MdbOneToOnePropertyMapping.DictionarySetterDelegate dictionarySetter) : base(propertyDefinition, new StorePropertyDefinition[]
		{
			storePropertyDefinition
		})
		{
			this.ItemGetter = itemGetter;
			this.ItemSetter = itemSetter;
			this.DictionaryGetter = dictionaryGetter;
			this.DictionarySetter = dictionarySetter;
		}

		public StorePropertyDefinition StorePropertyDefinition
		{
			get
			{
				return base.StorePropertyDefinitions[0];
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return PropertyFlags.ReadOnly == (this.StorePropertyDefinition.PropertyFlags & PropertyFlags.ReadOnly);
			}
		}

		public override bool IsStreamable
		{
			get
			{
				return PropertyFlags.Streamable == (this.StorePropertyDefinition.PropertyFlags & PropertyFlags.Streamable);
			}
		}

		private protected MdbOneToOnePropertyMapping.ItemGetterDelegate ItemGetter { protected get; private set; }

		private protected MdbOneToOnePropertyMapping.ItemSetterDelegate ItemSetter { protected get; private set; }

		private protected MdbOneToOnePropertyMapping.DictionaryGetterDelegate DictionaryGetter { protected get; private set; }

		private protected MdbOneToOnePropertyMapping.DictionarySetterDelegate DictionarySetter { protected get; private set; }

		public override object GetPropertyValue(IItem item, IMdbPropertyMappingContext context)
		{
			if (this.ItemGetter == null)
			{
				throw new NotImplementedException(this.PropertyDefinition.Name);
			}
			return this.ItemGetter(item, this.StorePropertyDefinition, context);
		}

		public override void SetPropertyValue(IItem item, object value, IMdbPropertyMappingContext context)
		{
			if (this.ItemSetter == null)
			{
				throw new NotImplementedException(this.PropertyDefinition.Name);
			}
			this.ItemSetter(item, this.StorePropertyDefinition, value, context);
		}

		public override object GetPropertyValue(IDictionary<StorePropertyDefinition, object> dictionary)
		{
			if (this.DictionaryGetter == null)
			{
				throw new NotImplementedException(this.PropertyDefinition.Name);
			}
			return this.DictionaryGetter(dictionary, this.StorePropertyDefinition);
		}

		public override void SetPropertyValue(IDictionary<StorePropertyDefinition, object> dictionary, object value)
		{
			if (this.DictionarySetter == null)
			{
				throw new NotImplementedException(this.PropertyDefinition.Name);
			}
			this.DictionarySetter(dictionary, this.StorePropertyDefinition, value);
		}

		internal static object DefaultItemGetter(IItem item, StorePropertyDefinition propertyDefinition, IMdbPropertyMappingContext context)
		{
			return item.TryGetProperty(propertyDefinition);
		}

		internal static void DefaultItemSetter(IItem item, StorePropertyDefinition propertyDefinition, object value, IMdbPropertyMappingContext context)
		{
			item.SetOrDeleteProperty(propertyDefinition, value);
		}

		internal static object DefaultDictionaryGetter(IDictionary<StorePropertyDefinition, object> dictionary, StorePropertyDefinition propertyDefinition)
		{
			object result = null;
			dictionary.TryGetValue(propertyDefinition, out result);
			return result;
		}

		internal static void DefaultDictionarySetter(IDictionary<StorePropertyDefinition, object> dictionary, StorePropertyDefinition propertyDefinition, object value)
		{
			if (value == null)
			{
				if (dictionary.ContainsKey(propertyDefinition))
				{
					dictionary.Remove(propertyDefinition);
					return;
				}
			}
			else
			{
				dictionary[propertyDefinition] = value;
			}
		}

		public delegate object ItemGetterDelegate(IItem item, StorePropertyDefinition propertyDefinition, IMdbPropertyMappingContext context);

		public delegate void ItemSetterDelegate(IItem item, StorePropertyDefinition propertyDefinition, object value, IMdbPropertyMappingContext context);

		public delegate object DictionaryGetterDelegate(IDictionary<StorePropertyDefinition, object> dictionary, StorePropertyDefinition propertyDefinition);

		public delegate void DictionarySetterDelegate(IDictionary<StorePropertyDefinition, object> dictionary, StorePropertyDefinition propertyDefinition, object value);
	}
}
