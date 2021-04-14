using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Mdb
{
	internal class MdbOneToOneTransformPropertyMapping : MdbOneToOnePropertyMapping
	{
		public MdbOneToOneTransformPropertyMapping(PropertyDefinition propertyDefinition, StorePropertyDefinition storePropertyDefinition, MdbOneToOneTransformPropertyMapping.TransformDelegate getterTransform, MdbOneToOneTransformPropertyMapping.TransformDelegate setterTransform) : base(propertyDefinition, storePropertyDefinition, (getterTransform != null) ? new MdbOneToOnePropertyMapping.ItemGetterDelegate(MdbOneToOnePropertyMapping.DefaultItemGetter) : null, (setterTransform != null) ? new MdbOneToOnePropertyMapping.ItemSetterDelegate(MdbOneToOnePropertyMapping.DefaultItemSetter) : null, (getterTransform != null) ? new MdbOneToOnePropertyMapping.DictionaryGetterDelegate(MdbOneToOnePropertyMapping.DefaultDictionaryGetter) : null, (setterTransform != null) ? new MdbOneToOnePropertyMapping.DictionarySetterDelegate(MdbOneToOnePropertyMapping.DefaultDictionarySetter) : null)
		{
			if (getterTransform == null && setterTransform == null)
			{
				throw new ArgumentException("Both getter and setter transforms are null");
			}
			this.GetterTransform = getterTransform;
			this.SetterTransform = setterTransform;
		}

		public MdbOneToOneTransformPropertyMapping(PropertyDefinition propertyDefinition, StorePropertyDefinition storePropertyDefinition, MdbOneToOnePropertyMapping.ItemGetterDelegate itemGetter, MdbOneToOnePropertyMapping.ItemSetterDelegate itemSetter) : base(propertyDefinition, storePropertyDefinition, itemGetter, itemSetter, null, null)
		{
			if (itemGetter == null && itemSetter == null)
			{
				throw new ArgumentException("Both getter and setter delegates are null");
			}
		}

		private protected MdbOneToOneTransformPropertyMapping.TransformDelegate GetterTransform { protected get; private set; }

		private protected MdbOneToOneTransformPropertyMapping.TransformDelegate SetterTransform { protected get; private set; }

		public override object GetPropertyValue(IItem item, IMdbPropertyMappingContext context)
		{
			object propertyValue = base.GetPropertyValue(item, context);
			if (this.GetterTransform == null)
			{
				return propertyValue;
			}
			return this.GetterTransform(this.PropertyDefinition, base.StorePropertyDefinition, propertyValue);
		}

		public override void SetPropertyValue(IItem item, object value, IMdbPropertyMappingContext context)
		{
			if (this.SetterTransform != null)
			{
				value = this.SetterTransform(this.PropertyDefinition, base.StorePropertyDefinition, value);
			}
			base.SetPropertyValue(item, value, context);
		}

		public override object GetPropertyValue(IDictionary<StorePropertyDefinition, object> dictionary)
		{
			object propertyValue = base.GetPropertyValue(dictionary);
			if (this.GetterTransform == null)
			{
				return propertyValue;
			}
			return this.GetterTransform(this.PropertyDefinition, base.StorePropertyDefinition, propertyValue);
		}

		public override void SetPropertyValue(IDictionary<StorePropertyDefinition, object> dictionary, object value)
		{
			if (this.SetterTransform != null)
			{
				value = this.SetterTransform(this.PropertyDefinition, base.StorePropertyDefinition, value);
			}
			base.SetPropertyValue(dictionary, value);
		}

		internal static object FixTypeGetterTranform(PropertyDefinition genericPropertyDefinition, StorePropertyDefinition storePropertyDefinition, object storePropertyValue)
		{
			return MdbOneToOneTransformPropertyMapping.FixType(genericPropertyDefinition.Type, storePropertyValue);
		}

		internal static object FixTypeSetterTranform(PropertyDefinition genericPropertyDefinition, StorePropertyDefinition storePropertyDefinition, object genericPropertyValue)
		{
			return MdbOneToOneTransformPropertyMapping.FixType(storePropertyDefinition.Type, genericPropertyValue);
		}

		private static object FixType(Type expectedType, object value)
		{
			if (value == null)
			{
				return null;
			}
			Type type = value.GetType();
			if (expectedType == type)
			{
				return value;
			}
			if (expectedType.IsPrimitive && type.IsEnum)
			{
				if (expectedType == Enum.GetUnderlyingType(type))
				{
					return Convert.ChangeType(value, expectedType);
				}
			}
			else if (expectedType.IsEnum && type.IsPrimitive && Enum.GetUnderlyingType(expectedType) == type)
			{
				return Enum.ToObject(expectedType, value);
			}
			return value;
		}

		public delegate object TransformDelegate(PropertyDefinition genericPropertyDefinition, StorePropertyDefinition storePropertyDefinition, object value);
	}
}
