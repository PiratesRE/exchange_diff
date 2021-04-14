using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class PropertyTagPropertyDefinition : NativeStorePropertyDefinition
	{
		private PropertyTagPropertyDefinition(string displayName, PropTag propertyTag, PropertyTagPropertyDefinition.PropTagKey key, PropertyFlags flags, bool isCustom, PropertyDefinitionConstraint[] constraints) : base(PropertyTypeSpecifier.PropertyTag, displayName, InternalSchema.ClrTypeFromPropTag(propertyTag), propertyTag.ValueType(), PropertyTagPropertyDefinition.CalculatePropertyTagPropertyFlags(propertyTag, flags), constraints)
		{
			if (propertyTag.IsNamedProperty() || !propertyTag.IsValid())
			{
				throw new ArgumentException("Invalid property tag", "propertyTag");
			}
			this.InternalKey = key;
			this.propertyTag = propertyTag;
		}

		public uint PropertyTag
		{
			get
			{
				return (uint)this.propertyTag;
			}
		}

		public bool IsApplicationSpecific
		{
			get
			{
				return this.propertyTag.IsApplicationSpecific();
			}
		}

		public bool IsTransmittable
		{
			get
			{
				return this.propertyTag.IsTransmittable();
			}
		}

		private PropertyTagPropertyDefinition.PropTagKey InternalKey
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		public static PropertyTagPropertyDefinition CreateCustom(string displayName, uint propertyTag)
		{
			return PropertyTagPropertyDefinition.CreateCustom(displayName, propertyTag, PropertyDefinitionConstraint.None);
		}

		public static PropertyTagPropertyDefinition CreateCustom(string displayName, uint propertyTag, params PropertyDefinitionConstraint[] constraints)
		{
			return PropertyTagPropertyDefinition.CreateCustom(displayName, propertyTag, PropertyFlags.None, constraints);
		}

		public static PropertyTagPropertyDefinition CreateCustom(string displayName, uint propertyTag, PropertyFlags flags, params PropertyDefinitionConstraint[] constraints)
		{
			return PropertyTagPropertyDefinition.InternalCreateCustom(displayName, (PropTag)propertyTag, flags, NativeStorePropertyDefinition.TypeCheckingFlag.ThrowOnInvalidType, constraints);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(obj, this))
			{
				return true;
			}
			PropertyTagPropertyDefinition propertyTagPropertyDefinition = obj as PropertyTagPropertyDefinition;
			return propertyTagPropertyDefinition != null && this.propertyTag == propertyTagPropertyDefinition.propertyTag;
		}

		public override int GetHashCode()
		{
			return (int)this.propertyTag;
		}

		public PropertyTagPropertyDefinition.PropTagKey GetKey()
		{
			return this.InternalKey;
		}

		internal static bool TryFindEquivalentDefinition(PropertyTagPropertyDefinition.PropTagKey key, bool isCustom, PropType type, NativeStorePropertyDefinition.TypeCheckingFlag typeCheckingFlag, out PropertyTagPropertyDefinition definition, out bool createNewDefinition)
		{
			createNewDefinition = true;
			if (!isCustom)
			{
				definition = null;
				return false;
			}
			switch (NativeStorePropertyDefinitionDictionary.TryFindInstance(key, type, out definition))
			{
			case PropertyMatchResult.Found:
				createNewDefinition = false;
				return true;
			case PropertyMatchResult.TypeMismatch:
				NativeStorePropertyDefinition.OnFailedPropertyTypeCheck(key, type, typeCheckingFlag, out createNewDefinition);
				break;
			}
			return false;
		}

		internal static PropertyTagPropertyDefinition InternalCreate(string displayName, PropTag propertyTag)
		{
			return PropertyTagPropertyDefinition.InternalCreate(displayName, propertyTag, PropertyFlags.None);
		}

		internal static PropertyTagPropertyDefinition InternalCreate(string displayName, PropTag propertyTag, params PropertyDefinitionConstraint[] constraints)
		{
			return PropertyTagPropertyDefinition.InternalCreate(displayName, propertyTag, PropertyFlags.None, NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck, false, constraints);
		}

		internal static PropertyTagPropertyDefinition InternalCreate(string displayName, PropTag propertyTag, PropertyFlags propertyFlags)
		{
			return PropertyTagPropertyDefinition.InternalCreate(displayName, propertyTag, propertyFlags, NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck, false, PropertyDefinitionConstraint.None);
		}

		internal static PropertyTagPropertyDefinition InternalCreateCustom(string displayName, PropTag propertyTag, PropertyFlags flags, NativeStorePropertyDefinition.TypeCheckingFlag typeCheckingFlag)
		{
			return PropertyTagPropertyDefinition.InternalCreateCustom(displayName, propertyTag, flags, typeCheckingFlag, PropertyDefinitionConstraint.None);
		}

		internal static PropertyTagPropertyDefinition InternalCreateCustom(string displayName, PropTag propertyTag, PropertyFlags flags, NativeStorePropertyDefinition.TypeCheckingFlag typeCheckingFlag, PropertyDefinitionConstraint[] constraints)
		{
			return PropertyTagPropertyDefinition.InternalCreate(displayName, propertyTag, flags | PropertyFlags.Custom, typeCheckingFlag, true, constraints);
		}

		private static PropertyTagPropertyDefinition InternalCreate(string displayName, PropTag propertyTag, PropertyFlags flags, NativeStorePropertyDefinition.TypeCheckingFlag typeCheckingFlag, bool isCustom, PropertyDefinitionConstraint[] constraints)
		{
			if (!propertyTag.IsValid())
			{
				throw new ArgumentException("Invalid property tag", "propertyTag");
			}
			PropType propType = propertyTag.ValueType();
			if (propType == PropType.AnsiString || propType == PropType.AnsiStringArray)
			{
				propertyTag = ((propertyTag & (PropTag)4294967265U) | (PropTag)31U);
				propType = propertyTag.ValueType();
			}
			PropertyTagPropertyDefinition.PropTagKey propTagKey = new PropertyTagPropertyDefinition.PropTagKey(propertyTag);
			PropertyTagPropertyDefinition result;
			bool flag;
			if (PropertyTagPropertyDefinition.TryFindEquivalentDefinition(propTagKey, isCustom, propType, typeCheckingFlag, out result, out flag))
			{
				return result;
			}
			if (flag)
			{
				try
				{
					return new PropertyTagPropertyDefinition(displayName, propertyTag, propTagKey, flags, isCustom, constraints);
				}
				catch (InvalidPropertyTypeException)
				{
					if (typeCheckingFlag == NativeStorePropertyDefinition.TypeCheckingFlag.ThrowOnInvalidType)
					{
						throw;
					}
				}
			}
			return null;
		}

		private static PropertyFlags CalculatePropertyTagPropertyFlags(PropTag propertyTag, PropertyFlags userFlags)
		{
			PropertyFlags propertyFlags = userFlags & (PropertyFlags)(-2147418113);
			if (propertyTag.IsTransmittable())
			{
				propertyFlags |= PropertyFlags.Transmittable;
			}
			return propertyFlags;
		}

		[Obsolete("Use propertyTag.IsPropertyTransmittable().")]
		internal static bool IsPropertyTransmittable(PropTag propertyTag)
		{
			return propertyTag.IsTransmittable();
		}

		protected override string GetPropertyDefinitionString()
		{
			return "0x" + this.PropertyTag.ToString("x8", CultureInfo.InvariantCulture);
		}

		[Obsolete("Use propertyTag.IsApplicationSpecific().")]
		internal static bool IsApplicationSpecificPropertyTag(PropTag propertyTag)
		{
			return propertyTag.IsApplicationSpecific();
		}

		private readonly PropTag propertyTag;

		private PropertyTagPropertyDefinition.PropTagKey key;

		[Serializable]
		public struct PropTagKey : IEquatable<PropertyTagPropertyDefinition.PropTagKey>
		{
			internal PropTagKey(PropTag propertyTag)
			{
				if (propertyTag.IsApplicationSpecific())
				{
					this.propertyTag = propertyTag;
					return;
				}
				this.propertyTag = PropTagHelper.PropTagFromIdAndType(propertyTag.Id(), PropType.Unspecified);
			}

			public bool Equals(PropertyTagPropertyDefinition.PropTagKey other)
			{
				return this.propertyTag == other.propertyTag;
			}

			public override int GetHashCode()
			{
				return (int)this.propertyTag;
			}

			public override string ToString()
			{
				return string.Format("PropertyTagPropertyDefinition:{0:x8}", (uint)this.propertyTag);
			}

			private readonly PropTag propertyTag;
		}
	}
}
