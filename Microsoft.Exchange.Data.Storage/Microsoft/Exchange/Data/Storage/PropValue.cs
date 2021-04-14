using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct PropValue : IEquatable<PropValue>
	{
		public PropValue(StorePropertyDefinition propDef, object value)
		{
			if (propDef == null)
			{
				throw new ArgumentNullException("propDef");
			}
			if (value == null)
			{
				throw PropertyError.ToException(new PropertyError[]
				{
					new PropertyError(propDef, PropertyErrorCode.NullValue)
				});
			}
			this.propDef = propDef;
			this.value = value;
		}

		public static PropValue CreatePropValue<T>(T propDef, object value) where T : StorePropertyDefinition
		{
			return new PropValue(propDef, value);
		}

		public StorePropertyDefinition Property
		{
			get
			{
				return this.propDef;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public static implicit operator KeyValuePair<StorePropertyDefinition, object>(PropValue propValue)
		{
			return new KeyValuePair<StorePropertyDefinition, object>(propValue.propDef, propValue.value);
		}

		public static implicit operator KeyValuePair<PropertyDefinition, object>(PropValue propValue)
		{
			return new KeyValuePair<PropertyDefinition, object>(propValue.propDef, propValue.value);
		}

		public static implicit operator PropValue(KeyValuePair<StorePropertyDefinition, object> kvp)
		{
			return new PropValue(kvp.Key, kvp.Value);
		}

		public static explicit operator PropValue(KeyValuePair<PropertyDefinition, object> kvp)
		{
			return new PropValue(InternalSchema.ToStorePropertyDefinition(kvp.Key), kvp.Value);
		}

		public static IEnumerable<PropValue> ConvertEnumerator<PropDefType>(IEnumerable<KeyValuePair<PropDefType, object>> sourceEnumerator) where PropDefType : PropertyDefinition
		{
			if (sourceEnumerator != null)
			{
				foreach (KeyValuePair<PropDefType, object> pair in sourceEnumerator)
				{
					KeyValuePair<PropDefType, object> keyValuePair = pair;
					StorePropertyDefinition storePropertyDefinition = InternalSchema.ToStorePropertyDefinition(keyValuePair.Key);
					KeyValuePair<PropDefType, object> keyValuePair2 = pair;
					yield return new PropValue(storePropertyDefinition, keyValuePair2.Value);
				}
			}
			yield break;
		}

		public bool Equals(PropValue other)
		{
			return this.propDef.Equals(other.propDef) && Util.ValueEquals(this.value, other.value);
		}

		public override int GetHashCode()
		{
			return this.propDef.GetHashCode() ^ this.value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is PropValue && this.Equals((PropValue)obj);
		}

		private readonly StorePropertyDefinition propDef;

		private readonly object value;
	}
}
