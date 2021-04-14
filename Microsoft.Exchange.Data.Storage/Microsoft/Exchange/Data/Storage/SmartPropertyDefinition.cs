using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal abstract class SmartPropertyDefinition : StorePropertyDefinition
	{
		protected SmartPropertyDefinition(string displayName, Type valueType, PropertyFlags flags, PropertyDefinitionConstraint[] constraints, params PropertyDependency[] dependencies) : base(PropertyTypeSpecifier.Calculated, displayName, valueType, SmartPropertyDefinition.CalculateSmartPropertyFlags(flags), constraints)
		{
			for (int i = 0; i < dependencies.Length; i++)
			{
			}
			this.dependencies = dependencies;
			this.RegisterFilterTranslation();
		}

		public override ICollection<PropertyDefinition> RequiredPropertyDefinitionsWhenReading
		{
			get
			{
				if (this.requiredPropertyDefinitionsWhenReading == null)
				{
					this.requiredPropertyDefinitionsWhenReading = new List<PropertyDefinition>();
					foreach (PropertyDependency propertyDependency in this.Dependencies)
					{
						if ((propertyDependency.Type & PropertyDependencyType.NeedForRead) == PropertyDependencyType.NeedForRead)
						{
							this.requiredPropertyDefinitionsWhenReading.Add(propertyDependency.Property);
						}
					}
				}
				return this.requiredPropertyDefinitionsWhenReading;
			}
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
			return new SortBy[]
			{
				new SortBy(this.GetSortProperty(), sortOrder)
			};
		}

		internal override NativeStorePropertyDefinition GetNativeGroupBy()
		{
			return this.GetSortProperty();
		}

		internal override GroupSort GetNativeGroupSort(SortOrder sortOrder, Aggregate aggregate)
		{
			return new GroupSort(this.GetSortProperty(), sortOrder, aggregate);
		}

		internal virtual QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			throw this.CreateInvalidFilterConversionException(filter);
		}

		internal virtual QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			throw this.CreateInvalidFilterConversionException(filter);
		}

		internal virtual void RegisterFilterTranslation()
		{
		}

		public override bool Equals(object obj)
		{
			SmartPropertyDefinition smartPropertyDefinition = obj as SmartPropertyDefinition;
			return smartPropertyDefinition != null && this.GetHashCode() == smartPropertyDefinition.GetHashCode() && base.Name == smartPropertyDefinition.Name && base.Type.Equals(smartPropertyDefinition.Type);
		}

		public override int GetHashCode()
		{
			if (this.calcHashCode)
			{
				this.hashCode = (base.Name.GetHashCode() ^ base.Type.GetHashCode());
				this.calcHashCode = false;
			}
			return this.hashCode;
		}

		protected virtual NativeStorePropertyDefinition GetSortProperty()
		{
			Exception ex = new UnsupportedPropertyForSortGroupException(ServerStrings.ExSortGroupNotSupportedForProperty(base.Name), this);
			ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), ex.Message);
			throw ex;
		}

		protected override string GetPropertyDefinitionString()
		{
			return "Calc:" + base.Name;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			throw new NotSupportedException(ServerStrings.ExSetNotSupportedForCalculatedProperty(this));
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			throw new NotSupportedException(ServerStrings.ExGetNotSupportedForCalculatedProperty(this));
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			throw new NotSupportedException(ServerStrings.ExDeleteNotSupportedForCalculatedProperty(this));
		}

		protected override bool InternalIsDirty(PropertyBag.BasicPropertyStore propertyBag)
		{
			foreach (PropertyDependency propertyDependency in this.dependencies)
			{
				if ((propertyDependency.Type & PropertyDependencyType.NeedForRead) == PropertyDependencyType.NeedForRead && propertyBag.IsDirty(propertyDependency.Property))
				{
					return true;
				}
			}
			return false;
		}

		private static PropertyFlags CalculateSmartPropertyFlags(PropertyFlags flags)
		{
			return flags & (PropertyFlags)(-2147418113);
		}

		protected QueryFilter SinglePropertySmartFilterToNativeFilter(QueryFilter filter, PropertyDefinition nativeProperty)
		{
			MultivaluedInstanceComparisonFilter multivaluedInstanceComparisonFilter = filter as MultivaluedInstanceComparisonFilter;
			if (multivaluedInstanceComparisonFilter != null)
			{
				return new MultivaluedInstanceComparisonFilter(multivaluedInstanceComparisonFilter.ComparisonOperator, nativeProperty, multivaluedInstanceComparisonFilter.PropertyValue);
			}
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter != null)
			{
				return new ComparisonFilter(comparisonFilter.ComparisonOperator, nativeProperty, comparisonFilter.PropertyValue);
			}
			ExistsFilter existsFilter = filter as ExistsFilter;
			if (existsFilter != null)
			{
				return new ExistsFilter(nativeProperty);
			}
			TextFilter textFilter = filter as TextFilter;
			if (textFilter != null)
			{
				return new TextFilter(nativeProperty, textFilter.Text, textFilter.MatchOptions, textFilter.MatchFlags);
			}
			BitMaskFilter bitMaskFilter = filter as BitMaskFilter;
			if (bitMaskFilter != null)
			{
				return new BitMaskFilter(nativeProperty, bitMaskFilter.Mask, bitMaskFilter.IsNonZero);
			}
			BitMaskAndFilter bitMaskAndFilter = filter as BitMaskAndFilter;
			if (bitMaskAndFilter != null)
			{
				return new BitMaskAndFilter(nativeProperty, bitMaskAndFilter.Mask);
			}
			BitMaskOrFilter bitMaskOrFilter = filter as BitMaskOrFilter;
			if (bitMaskOrFilter != null)
			{
				return new BitMaskOrFilter(nativeProperty, bitMaskOrFilter.Mask);
			}
			throw this.CreateInvalidFilterConversionException(filter);
		}

		protected QueryFilter SinglePropertyNativeFilterToSmartFilter(QueryFilter filter, PropertyDefinition nativeProperty)
		{
			SinglePropertyFilter singlePropertyFilter = filter as SinglePropertyFilter;
			if (singlePropertyFilter != null && singlePropertyFilter.Property.Equals(nativeProperty))
			{
				MultivaluedInstanceComparisonFilter multivaluedInstanceComparisonFilter = filter as MultivaluedInstanceComparisonFilter;
				if (multivaluedInstanceComparisonFilter != null)
				{
					return new MultivaluedInstanceComparisonFilter(multivaluedInstanceComparisonFilter.ComparisonOperator, this, multivaluedInstanceComparisonFilter.PropertyValue);
				}
				ComparisonFilter comparisonFilter = filter as ComparisonFilter;
				if (comparisonFilter != null)
				{
					return new ComparisonFilter(comparisonFilter.ComparisonOperator, this, comparisonFilter.PropertyValue);
				}
				ExistsFilter existsFilter = filter as ExistsFilter;
				if (existsFilter != null)
				{
					return new ExistsFilter(this);
				}
				TextFilter textFilter = filter as TextFilter;
				if (textFilter != null)
				{
					return new TextFilter(this, textFilter.Text, textFilter.MatchOptions, textFilter.MatchFlags);
				}
				BitMaskFilter bitMaskFilter = filter as BitMaskFilter;
				if (bitMaskFilter != null)
				{
					return new BitMaskFilter(this, bitMaskFilter.Mask, bitMaskFilter.IsNonZero);
				}
				BitMaskAndFilter bitMaskAndFilter = filter as BitMaskAndFilter;
				if (bitMaskAndFilter != null)
				{
					return new BitMaskAndFilter(this, bitMaskAndFilter.Mask);
				}
				BitMaskOrFilter bitMaskOrFilter = filter as BitMaskOrFilter;
				if (bitMaskOrFilter != null)
				{
					return new BitMaskOrFilter(this, bitMaskOrFilter.Mask);
				}
			}
			return null;
		}

		internal Exception CreateInvalidFilterConversionException(QueryFilter filter)
		{
			Exception ex = new FilterNotSupportedException(ServerStrings.ExFilterNotSupportedForProperty(filter.ToString(), base.Name), filter, new PropertyDefinition[]
			{
				this
			});
			ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), ex.Message);
			return ex;
		}

		internal PropertyDependency[] Dependencies
		{
			get
			{
				return this.dependencies;
			}
		}

		internal virtual PropertyDependency[] LegalTrackingDependencies
		{
			get
			{
				return this.dependencies;
			}
		}

		protected override void ForEachMatch(PropertyDependencyType targetDependencyType, Action<NativeStorePropertyDefinition> action)
		{
			for (int i = 0; i < this.dependencies.Length; i++)
			{
				PropertyDependency propertyDependency = this.dependencies[i];
				if ((propertyDependency.Type & targetDependencyType) != PropertyDependencyType.None)
				{
					action(propertyDependency.Property);
				}
			}
		}

		private ICollection<PropertyDefinition> requiredPropertyDefinitionsWhenReading;

		private readonly PropertyDependency[] dependencies;

		private bool calcHashCode = true;

		private int hashCode;
	}
}
