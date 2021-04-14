using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class ApplicationAggregatedProperty : SmartPropertyDefinition
	{
		public ApplicationAggregatedProperty(string displayName, Type valueType, PropertyFlags propertyFlags, PropertyAggregationStrategy propertyAggregationStrategy, SortByAndFilterStrategy sortByAndFilterStrategy) : this(displayName, valueType, propertyFlags, propertyAggregationStrategy, sortByAndFilterStrategy, new SimpleVirtualPropertyDefinition("InternalStorage:" + displayName, valueType, propertyFlags, new PropertyDefinitionConstraint[0]))
		{
		}

		public ApplicationAggregatedProperty(ApplicationAggregatedProperty basePropertyDefinition, PropertyAggregationStrategy propertyAggregationStrategy) : this(basePropertyDefinition.Name, basePropertyDefinition.Type, basePropertyDefinition.PropertyFlags, propertyAggregationStrategy, basePropertyDefinition.sortByAndFilterStrategy, basePropertyDefinition.aggregatedProperty)
		{
		}

		private ApplicationAggregatedProperty(string displayName, Type valueType, PropertyFlags propertyFlags, PropertyAggregationStrategy propertyAggregationStrategy, SortByAndFilterStrategy sortByAndFilterStrategy, SimpleVirtualPropertyDefinition aggregatedProperty) : base(displayName, valueType, propertyFlags, PropertyDefinitionConstraint.None, propertyAggregationStrategy.Dependencies)
		{
			this.propertyAggregationStrategy = propertyAggregationStrategy;
			this.sortByAndFilterStrategy = sortByAndFilterStrategy;
			this.aggregatedProperty = aggregatedProperty;
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return this.sortByAndFilterStrategy.Capabilities;
			}
		}

		public static IStorePropertyBag Aggregate(PropertyAggregationContext context, IEnumerable<PropertyDefinition> properties)
		{
			return ApplicationAggregatedProperty.AggregateAsPropertyBag(context, properties).AsIStorePropertyBag();
		}

		internal static PropertyBag AggregateAsPropertyBag(PropertyAggregationContext context, IEnumerable<PropertyDefinition> properties)
		{
			MemoryPropertyBag memoryPropertyBag = new MemoryPropertyBag();
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				ApplicationAggregatedProperty applicationAggregatedProperty = propertyDefinition as ApplicationAggregatedProperty;
				if (applicationAggregatedProperty != null)
				{
					applicationAggregatedProperty.Aggregate(context, memoryPropertyBag);
				}
			}
			memoryPropertyBag.SetAllPropertiesLoaded();
			return memoryPropertyBag;
		}

		protected override NativeStorePropertyDefinition GetSortProperty()
		{
			NativeStorePropertyDefinition sortProperty = this.sortByAndFilterStrategy.GetSortProperty();
			if (sortProperty == null)
			{
				return base.GetSortProperty();
			}
			return sortProperty;
		}

		internal override SortBy[] GetNativeSortBy(SortOrder sortOrder)
		{
			SortBy[] nativeSortBy = this.sortByAndFilterStrategy.GetNativeSortBy(sortOrder);
			if (nativeSortBy == null)
			{
				return base.GetNativeSortBy(sortOrder);
			}
			return nativeSortBy;
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			QueryFilter queryFilter = this.sortByAndFilterStrategy.NativeFilterToSmartFilter(filter, this);
			if (queryFilter == null)
			{
				return base.NativeFilterToSmartFilter(filter);
			}
			return queryFilter;
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			QueryFilter queryFilter = this.sortByAndFilterStrategy.SmartFilterToNativeFilter(filter, this);
			if (queryFilter == null)
			{
				return base.SmartFilterToNativeFilter(filter);
			}
			return queryFilter;
		}

		internal void Aggregate(PropertyAggregationContext context, PropertyBag target)
		{
			this.propertyAggregationStrategy.Aggregate(this.aggregatedProperty, context, target);
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			return propertyBag.GetValue(this.aggregatedProperty);
		}

		private readonly SimpleVirtualPropertyDefinition aggregatedProperty;

		private readonly SortByAndFilterStrategy sortByAndFilterStrategy;

		private readonly PropertyAggregationStrategy propertyAggregationStrategy;
	}
}
