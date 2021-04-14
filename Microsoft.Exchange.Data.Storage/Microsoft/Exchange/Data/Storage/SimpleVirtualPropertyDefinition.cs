using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SimpleVirtualPropertyDefinition : AtomicStorePropertyDefinition
	{
		internal SimpleVirtualPropertyDefinition(string displayName, Type propertyValueType, PropertyFlags flags, params PropertyDefinitionConstraint[] constraints) : this(PropertyTypeSpecifier.SimpleVirtual, displayName, propertyValueType, flags, constraints)
		{
		}

		protected SimpleVirtualPropertyDefinition(PropertyTypeSpecifier specifiedWith, string displayName, Type propertyValueType, PropertyFlags flags, params PropertyDefinitionConstraint[] constraints) : base(specifiedWith, displayName, propertyValueType, flags, constraints)
		{
			EnumValidator.AssertValid<PropertyFlags>(flags);
			this.hashCode = new LazilyInitialized<int>(new Func<int>(this.ComputeHashCode));
		}

		protected override string GetPropertyDefinitionString()
		{
			return "SV:" + base.Name;
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.None;
			}
		}

		internal override SortBy[] GetNativeSortBy(SortOrder sortOrder)
		{
			throw new UnsupportedPropertyForSortGroupException(ServerStrings.ExFilterAndSortNotSupportedInSimpleVirtualPropertyDefinition, this);
		}

		internal override NativeStorePropertyDefinition GetNativeGroupBy()
		{
			throw new UnsupportedPropertyForSortGroupException(ServerStrings.ExFilterAndSortNotSupportedInSimpleVirtualPropertyDefinition, this);
		}

		internal override GroupSort GetNativeGroupSort(SortOrder sortOrder, Aggregate aggregate)
		{
			throw new UnsupportedPropertyForSortGroupException(ServerStrings.ExFilterAndSortNotSupportedInSimpleVirtualPropertyDefinition, this);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
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

		protected virtual int ComputeHashCode()
		{
			return base.Name.GetHashCode() ^ base.Type.GetHashCode();
		}

		public sealed override int GetHashCode()
		{
			return this.hashCode;
		}

		public sealed override bool Equals(object obj)
		{
			if (object.ReferenceEquals(obj, this))
			{
				return true;
			}
			SimpleVirtualPropertyDefinition simpleVirtualPropertyDefinition = obj as SimpleVirtualPropertyDefinition;
			return simpleVirtualPropertyDefinition != null && this.GetHashCode() == simpleVirtualPropertyDefinition.GetHashCode() && base.GetType() == simpleVirtualPropertyDefinition.GetType() && base.Name == simpleVirtualPropertyDefinition.Name && base.Type.Equals(simpleVirtualPropertyDefinition.Type);
		}

		protected sealed override void ForEachMatch(PropertyDependencyType targetDependencyType, Action<NativeStorePropertyDefinition> action)
		{
		}

		private readonly LazilyInitialized<int> hashCode;
	}
}
