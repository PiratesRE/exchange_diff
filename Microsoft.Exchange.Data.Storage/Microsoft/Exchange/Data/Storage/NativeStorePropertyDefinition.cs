using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal abstract class NativeStorePropertyDefinition : AtomicStorePropertyDefinition
	{
		internal NativeStorePropertyDefinition(PropertyTypeSpecifier propertyTypeSpecifier, string displayName, Type type, PropType mapiPropertyType, PropertyFlags childFlags, PropertyDefinitionConstraint[] constraints) : base(propertyTypeSpecifier, displayName, type, childFlags, constraints)
		{
			this.mapiPropertyType = mapiPropertyType;
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.All;
			}
		}

		protected static void OnFailedPropertyTypeCheck(object key, PropType type, NativeStorePropertyDefinition.TypeCheckingFlag typeCheckingFlag, out bool createNewDefinition)
		{
			switch (typeCheckingFlag)
			{
			case NativeStorePropertyDefinition.TypeCheckingFlag.DoNotCreateInvalidType:
			case NativeStorePropertyDefinition.TypeCheckingFlag.AllowCompatibleType:
				createNewDefinition = false;
				return;
			case NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck:
				createNewDefinition = true;
				return;
			}
			throw new InvalidPropertyTypeException(ServerStrings.ExInvalidPropertyType(key.ToString(), type.ToString()));
		}

		protected static PropertyFlags CalculatePropertyTagPropertyFlags(PropertyFlags userFlags, bool isCustom)
		{
			PropertyFlags propertyFlags = userFlags & (PropertyFlags)(-2147418113);
			if (isCustom)
			{
				propertyFlags |= PropertyFlags.Custom;
			}
			return propertyFlags | PropertyFlags.Transmittable;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			if (propertyBag.CanIgnoreUnchangedProperties && (base.PropertyFlags & PropertyFlags.SetIfNotChanged) != PropertyFlags.SetIfNotChanged && propertyBag.IsLoaded(this) && Util.ValueEquals(value, propertyBag.GetValue(this)))
			{
				return;
			}
			propertyBag.SetValue(this, value);
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			return propertyBag.GetValue(this);
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.Delete(this);
		}

		internal override SortBy[] GetNativeSortBy(SortOrder sortOrder)
		{
			return new SortBy[]
			{
				new SortBy(this, sortOrder)
			};
		}

		internal override NativeStorePropertyDefinition GetNativeGroupBy()
		{
			return this;
		}

		internal override GroupSort GetNativeGroupSort(SortOrder sortOrder, Aggregate aggregate)
		{
			return new GroupSort(this, sortOrder, aggregate);
		}

		public PropType MapiPropertyType
		{
			get
			{
				return this.mapiPropertyType;
			}
		}

		protected override void ForEachMatch(PropertyDependencyType targetDependencyType, Action<NativeStorePropertyDefinition> action)
		{
			if ((targetDependencyType & PropertyDependencyType.NeedForRead) != PropertyDependencyType.None)
			{
				action(this);
			}
		}

		public static Type ClrTypeFromPropertyTag(uint propertyTag)
		{
			return InternalSchema.ClrTypeFromPropTag((PropTag)propertyTag);
		}

		private readonly PropType mapiPropertyType;

		internal enum TypeCheckingFlag
		{
			ThrowOnInvalidType,
			DoNotCreateInvalidType,
			DisableTypeCheck,
			AllowCompatibleType
		}
	}
}
