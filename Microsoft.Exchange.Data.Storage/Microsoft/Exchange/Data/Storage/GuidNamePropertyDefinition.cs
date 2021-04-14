using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class GuidNamePropertyDefinition : NamedPropertyDefinition
	{
		private GuidNamePropertyDefinition(string displayName, Type propertyType, PropType mapiPropertyType, GuidNamePropertyDefinition.GuidNameKey key, PropertyFlags flags, bool isCustom, PropertyDefinitionConstraint[] constraints) : base(PropertyTypeSpecifier.GuidString, displayName, propertyType, mapiPropertyType, GuidNamePropertyDefinition.CalculatePropertyTagPropertyFlags(key.PropertyName, key.PropertyGuid, flags, isCustom), constraints)
		{
			this.InternalKey = key;
			this.hashCode = (this.Guid.GetHashCode() ^ this.PropertyName.GetHashCode() ^ (int)base.MapiPropertyType);
		}

		public static bool IsValidName(Guid propertyNamespace, string propertyName)
		{
			return !string.IsNullOrEmpty(propertyName) && (!(propertyNamespace == WellKnownNamedPropertyGuid.InternetHeaders) || NamedProp.IsValidInternetHeadersName(propertyName)) && propertyName.Length <= StorageLimits.Instance.NamedPropertyNameMaximumLength;
		}

		public Guid Guid
		{
			get
			{
				return this.InternalKey.PropertyGuid;
			}
		}

		public string PropertyName
		{
			get
			{
				return this.InternalKey.PropertyName;
			}
		}

		private GuidNamePropertyDefinition.GuidNameKey InternalKey
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

		public static GuidNamePropertyDefinition CreateCustom(string displayName, Type propertyType, Guid propertyGuid, string propertyName, PropertyFlags flags)
		{
			return GuidNamePropertyDefinition.CreateCustom(displayName, propertyType, propertyGuid, propertyName, flags, PropertyDefinitionConstraint.None);
		}

		public static GuidNamePropertyDefinition CreateCustom(string displayName, Type propertyType, Guid propertyGuid, string propertyName, PropertyFlags flags, params PropertyDefinitionConstraint[] constraints)
		{
			PropType mapiPropType = InternalSchema.PropTagTypeFromClrType(propertyType);
			return GuidNamePropertyDefinition.InternalCreate(displayName, propertyType, mapiPropType, propertyGuid, propertyName, flags | PropertyFlags.Custom, NativeStorePropertyDefinition.TypeCheckingFlag.ThrowOnInvalidType, true, constraints);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(obj, this))
			{
				return true;
			}
			GuidNamePropertyDefinition guidNamePropertyDefinition = obj as GuidNamePropertyDefinition;
			return guidNamePropertyDefinition != null && this.GetHashCode() == guidNamePropertyDefinition.GetHashCode() && this.Guid == guidNamePropertyDefinition.Guid && this.PropertyName == guidNamePropertyDefinition.PropertyName && base.MapiPropertyType == guidNamePropertyDefinition.MapiPropertyType;
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		public override NamedPropertyDefinition.NamedPropertyKey GetKey()
		{
			return this.InternalKey;
		}

		internal static bool TryFindEquivalentDefinition(GuidNamePropertyDefinition.GuidNameKey key, bool isCustom, PropType type, NativeStorePropertyDefinition.TypeCheckingFlag typeCheckingFlag, out GuidNamePropertyDefinition definition, out bool createNewDefinition)
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

		private static PropertyFlags CalculatePropertyTagPropertyFlags(string propertyName, Guid guid, PropertyFlags userFlags, bool isCustom)
		{
			PropertyFlags propertyFlags = NativeStorePropertyDefinition.CalculatePropertyTagPropertyFlags(userFlags, isCustom);
			if (guid == WellKnownPropertySet.InternetHeaders && MimeConstants.IsInReservedHeaderNamespace(propertyName))
			{
				propertyFlags &= ~PropertyFlags.Transmittable;
			}
			return propertyFlags;
		}

		internal static GuidNamePropertyDefinition InternalCreateCustom(string displayName, PropType mapiPropType, Guid propertyGuid, string propertyName, PropertyFlags flags, NativeStorePropertyDefinition.TypeCheckingFlag typeCheckingFlag, params PropertyDefinitionConstraint[] constraints)
		{
			Type propertyType = InternalSchema.ClrTypeFromPropTagType(mapiPropType);
			return GuidNamePropertyDefinition.InternalCreate(displayName, propertyType, mapiPropType, propertyGuid, propertyName, flags | PropertyFlags.Custom, typeCheckingFlag, true, constraints);
		}

		internal static GuidNamePropertyDefinition InternalCreate(string displayName, Type propertyType, PropType mapiPropType, Guid propertyGuid, string propertyName, PropertyFlags flags, NativeStorePropertyDefinition.TypeCheckingFlag typeCheckingFlag, bool isCustom, params PropertyDefinitionConstraint[] constraints)
		{
			if (!GuidNamePropertyDefinition.IsValidName(propertyGuid, propertyName))
			{
				throw new ArgumentException("Invalid property name for property", "propertyName");
			}
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
			NamedProp namedProp = new NamedProp(propertyGuid, propertyName);
			NamedProp namedProp2 = WellKnownNamedProperties.Find(namedProp);
			if (namedProp2 != null)
			{
				namedProp = namedProp2;
			}
			else
			{
				namedProp = NamedPropertyDefinition.NamedPropertyKey.GetSingleton(namedProp);
			}
			GuidNamePropertyDefinition.GuidNameKey guidNameKey = new GuidNamePropertyDefinition.GuidNameKey(namedProp);
			GuidNamePropertyDefinition result;
			bool flag;
			if (GuidNamePropertyDefinition.TryFindEquivalentDefinition(guidNameKey, isCustom, mapiPropType, typeCheckingFlag, out result, out flag))
			{
				return result;
			}
			if (!flag)
			{
				return null;
			}
			return new GuidNamePropertyDefinition(displayName, propertyType, mapiPropType, guidNameKey, flags, isCustom, constraints);
		}

		protected override string GetPropertyDefinitionString()
		{
			return string.Format("{{{0}}}:'{1}'", this.Guid, this.PropertyName);
		}

		private const int MaxNameLength = 125;

		private GuidNamePropertyDefinition.GuidNameKey key;

		private int hashCode;

		[Serializable]
		public sealed class GuidNameKey : NamedPropertyDefinition.NamedPropertyKey, IEquatable<GuidNamePropertyDefinition.GuidNameKey>
		{
			internal GuidNameKey(NamedProp namedProp) : base(namedProp)
			{
			}

			public GuidNameKey(Guid propGuid, string propName) : base(propGuid, propName)
			{
			}

			public override bool Equals(object obj)
			{
				GuidNamePropertyDefinition.GuidNameKey other = obj as GuidNamePropertyDefinition.GuidNameKey;
				return this.Equals(other);
			}

			public bool Equals(GuidNamePropertyDefinition.GuidNameKey other)
			{
				return other != null && this.PropertyGuid == other.PropertyGuid && this.PropertyName == other.PropertyName;
			}

			public override int GetHashCode()
			{
				return this.PropertyGuid.GetHashCode() ^ this.PropertyName.GetHashCode();
			}

			public override string ToString()
			{
				return string.Format("{{{0}}}:'{1}'", this.PropertyGuid, this.PropertyName);
			}

			public Guid PropertyGuid
			{
				get
				{
					return base.NamedProp.Guid;
				}
			}

			public string PropertyName
			{
				get
				{
					return base.NamedProp.Name;
				}
			}
		}
	}
}
