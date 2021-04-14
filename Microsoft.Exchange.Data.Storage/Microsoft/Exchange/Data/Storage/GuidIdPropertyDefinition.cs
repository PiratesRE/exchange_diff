using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class GuidIdPropertyDefinition : NamedPropertyDefinition
	{
		private GuidIdPropertyDefinition(string displayName, Type propertyType, PropType mapiPropertyType, GuidIdPropertyDefinition.GuidIdKey key, PropertyFlags flags, bool isCustom, PropertyDefinitionConstraint[] constraints) : base(PropertyTypeSpecifier.GuidId, displayName, propertyType, mapiPropertyType, NativeStorePropertyDefinition.CalculatePropertyTagPropertyFlags(flags, isCustom), constraints)
		{
			this.InternalKey = key;
			this.hashCode = (this.Guid.GetHashCode() ^ this.Id ^ (int)base.MapiPropertyType);
		}

		public Guid Guid
		{
			get
			{
				return this.InternalKey.PropertyGuid;
			}
		}

		public int Id
		{
			get
			{
				return this.InternalKey.PropertyId;
			}
		}

		private GuidIdPropertyDefinition.GuidIdKey InternalKey
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

		public static GuidIdPropertyDefinition CreateCustom(string displayName, Type propertyType, Guid propertyGuid, int dispId, PropertyFlags flags)
		{
			return GuidIdPropertyDefinition.CreateCustom(displayName, propertyType, propertyGuid, dispId, flags, PropertyDefinitionConstraint.None);
		}

		public static GuidIdPropertyDefinition CreateCustom(string displayName, ushort mapiPropertyType, Guid propertyGuid, int dispId, PropertyFlags flags)
		{
			return GuidIdPropertyDefinition.CreateCustom(displayName, InternalSchema.ClrTypeFromPropTagType((PropType)mapiPropertyType), propertyGuid, dispId, flags, PropertyDefinitionConstraint.None);
		}

		public static GuidIdPropertyDefinition CreateCustom(string displayName, Type propertyType, Guid propertyGuid, int dispId, PropertyFlags flags, params PropertyDefinitionConstraint[] constraints)
		{
			PropType mapiPropType = InternalSchema.PropTagTypeFromClrType(propertyType);
			return GuidIdPropertyDefinition.InternalCreate(displayName, propertyType, mapiPropType, propertyGuid, dispId, flags | PropertyFlags.Custom, NativeStorePropertyDefinition.TypeCheckingFlag.ThrowOnInvalidType, true, constraints);
		}

		internal static bool TryFindEquivalentDefinition(GuidIdPropertyDefinition.GuidIdKey key, bool isCustom, PropType type, NativeStorePropertyDefinition.TypeCheckingFlag typeCheckingFlag, out GuidIdPropertyDefinition definition, out bool createNewDefinition)
		{
			createNewDefinition = true;
			if (!isCustom)
			{
				definition = null;
				return false;
			}
			switch (NativeStorePropertyDefinitionDictionary.TryFindInstance(key, type, typeCheckingFlag == NativeStorePropertyDefinition.TypeCheckingFlag.AllowCompatibleType, out definition))
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

		internal static GuidIdPropertyDefinition InternalCreateCustom(string displayName, PropType mapiPropType, Guid propertyGuid, int dispId, PropertyFlags flags, NativeStorePropertyDefinition.TypeCheckingFlag typeCheckingFlag, params PropertyDefinitionConstraint[] constraints)
		{
			Type propertyType = InternalSchema.ClrTypeFromPropTagType(mapiPropType);
			return GuidIdPropertyDefinition.InternalCreate(displayName, propertyType, mapiPropType, propertyGuid, dispId, flags | PropertyFlags.Custom, typeCheckingFlag, true, constraints);
		}

		internal static GuidIdPropertyDefinition InternalCreate(string displayName, Type propertyType, PropType mapiPropType, Guid propertyGuid, int dispId, PropertyFlags flags, NativeStorePropertyDefinition.TypeCheckingFlag typeCheckingFlag, bool isCustom, params PropertyDefinitionConstraint[] constraints)
		{
			if (mapiPropType == PropType.AnsiString)
			{
				mapiPropType = PropType.String;
				propertyType = typeof(string);
			}
			else if (mapiPropType == PropType.AnsiStringArray)
			{
				mapiPropType = PropType.StringArray;
				propertyType = typeof(string[]);
			}
			NamedProp namedProp = new NamedProp(propertyGuid, dispId);
			NamedProp namedProp2 = WellKnownNamedProperties.Find(namedProp);
			if (namedProp2 != null)
			{
				namedProp = namedProp2;
			}
			else
			{
				namedProp = NamedPropertyDefinition.NamedPropertyKey.GetSingleton(namedProp);
			}
			GuidIdPropertyDefinition.GuidIdKey guidIdKey = new GuidIdPropertyDefinition.GuidIdKey(namedProp);
			bool flag;
			if (propertyGuid == WellKnownPropertySet.InternetHeaders)
			{
				NativeStorePropertyDefinition.OnFailedPropertyTypeCheck(guidIdKey, mapiPropType, typeCheckingFlag, out flag);
				return null;
			}
			GuidIdPropertyDefinition result;
			if (GuidIdPropertyDefinition.TryFindEquivalentDefinition(guidIdKey, isCustom, mapiPropType, typeCheckingFlag, out result, out flag))
			{
				return result;
			}
			if (!flag)
			{
				return null;
			}
			return new GuidIdPropertyDefinition(displayName, propertyType, mapiPropType, guidIdKey, flags, isCustom, constraints);
		}

		protected override string GetPropertyDefinitionString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{{{0}}}:0x{1:x4}", new object[]
			{
				this.Guid,
				this.Id
			});
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(obj, this))
			{
				return true;
			}
			GuidIdPropertyDefinition guidIdPropertyDefinition = obj as GuidIdPropertyDefinition;
			return guidIdPropertyDefinition != null && this.GetHashCode() == guidIdPropertyDefinition.GetHashCode() && this.Guid == guidIdPropertyDefinition.Guid && this.Id == guidIdPropertyDefinition.Id && base.MapiPropertyType == guidIdPropertyDefinition.MapiPropertyType;
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		public override NamedPropertyDefinition.NamedPropertyKey GetKey()
		{
			return this.InternalKey;
		}

		private GuidIdPropertyDefinition.GuidIdKey key;

		private int hashCode;

		[Serializable]
		public sealed class GuidIdKey : NamedPropertyDefinition.NamedPropertyKey, IEquatable<GuidIdPropertyDefinition.GuidIdKey>
		{
			internal GuidIdKey(NamedProp namedProp) : base(namedProp)
			{
			}

			public GuidIdKey(Guid propGuid, int propId) : base(propGuid, propId)
			{
			}

			public override bool Equals(object obj)
			{
				GuidIdPropertyDefinition.GuidIdKey other = obj as GuidIdPropertyDefinition.GuidIdKey;
				return this.Equals(other);
			}

			public bool Equals(GuidIdPropertyDefinition.GuidIdKey other)
			{
				return other != null && this.PropertyId == other.PropertyId && this.PropertyGuid == other.PropertyGuid;
			}

			public override int GetHashCode()
			{
				return this.PropertyGuid.GetHashCode() ^ this.PropertyId;
			}

			public override string ToString()
			{
				return string.Format("{{{0}}}:0x{1:x4}", this.PropertyGuid, this.PropertyId);
			}

			public Guid PropertyGuid
			{
				get
				{
					return base.NamedProp.Guid;
				}
			}

			public int PropertyId
			{
				get
				{
					return base.NamedProp.Id;
				}
			}
		}
	}
}
