using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal abstract class SortByAndFilterStrategy
	{
		public static SortByAndFilterStrategy CreateSimpleSort(NativeStorePropertyDefinition property)
		{
			return new SortByAndFilterStrategy.SimpleSortStrategy(property);
		}

		public virtual StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.None;
			}
		}

		public virtual NativeStorePropertyDefinition GetSortProperty()
		{
			return null;
		}

		public virtual SortBy[] GetNativeSortBy(SortOrder sortOrder)
		{
			return null;
		}

		public virtual QueryFilter NativeFilterToSmartFilter(QueryFilter filter, ApplicationAggregatedProperty aggregatedProperty)
		{
			return null;
		}

		public virtual QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter, ApplicationAggregatedProperty aggregatedProperty)
		{
			return null;
		}

		public static readonly SortByAndFilterStrategy None = new SortByAndFilterStrategy.NoneStrategy();

		public static readonly SortByAndFilterStrategy SimpleCanQuery = new SortByAndFilterStrategy.SimpleCanQueryStrategy();

		public static readonly SortByAndFilterStrategy PersonType = new SortByAndFilterStrategy.PersonTypeStrategy();

		private sealed class NoneStrategy : SortByAndFilterStrategy
		{
		}

		private sealed class SimpleCanQueryStrategy : SortByAndFilterStrategy
		{
			public override StorePropertyCapabilities Capabilities
			{
				get
				{
					return StorePropertyCapabilities.CanQuery;
				}
			}
		}

		private sealed class SimpleSortStrategy : SortByAndFilterStrategy
		{
			public SimpleSortStrategy(NativeStorePropertyDefinition property)
			{
				this.property = property;
			}

			public override StorePropertyCapabilities Capabilities
			{
				get
				{
					return StorePropertyCapabilities.CanQuery | StorePropertyCapabilities.CanSortBy;
				}
			}

			public override NativeStorePropertyDefinition GetSortProperty()
			{
				return this.property;
			}

			public override SortBy[] GetNativeSortBy(SortOrder sortOrder)
			{
				return new SortBy[]
				{
					new SortBy(this.property, sortOrder)
				};
			}

			public override QueryFilter NativeFilterToSmartFilter(QueryFilter filter, ApplicationAggregatedProperty aggregatedProperty)
			{
				SinglePropertyFilter singlePropertyFilter = filter as SinglePropertyFilter;
				if (singlePropertyFilter != null && singlePropertyFilter.Property.Equals(this.property))
				{
					return singlePropertyFilter.CloneWithAnotherProperty(aggregatedProperty);
				}
				return base.NativeFilterToSmartFilter(filter, aggregatedProperty);
			}

			public override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter, ApplicationAggregatedProperty aggregatedProperty)
			{
				if (filter != null && filter.Property.Equals(aggregatedProperty))
				{
					return filter.CloneWithAnotherProperty(this.property);
				}
				return base.SmartFilterToNativeFilter(filter, aggregatedProperty);
			}

			private NativeStorePropertyDefinition property;
		}

		private sealed class PersonTypeStrategy : SortByAndFilterStrategy
		{
			public override StorePropertyCapabilities Capabilities
			{
				get
				{
					return StorePropertyCapabilities.CanQuery;
				}
			}

			public override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter, ApplicationAggregatedProperty aggregatedProperty)
			{
				ComparisonFilter comparisonFilter = filter as ComparisonFilter;
				if (comparisonFilter == null || !comparisonFilter.Property.Equals(PersonSchema.PersonType) || comparisonFilter.ComparisonOperator != ComparisonOperator.Equal)
				{
					return base.SmartFilterToNativeFilter(filter, aggregatedProperty);
				}
				string text;
				switch ((PersonType)comparisonFilter.PropertyValue)
				{
				case Microsoft.Exchange.Data.PersonType.Person:
					text = "IPM.Contact";
					break;
				case Microsoft.Exchange.Data.PersonType.DistributionList:
					text = "IPM.DistList";
					break;
				default:
					return base.SmartFilterToNativeFilter(filter, aggregatedProperty);
				}
				return new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, AggregatedContactSchema.MessageClass, text),
					new TextFilter(AggregatedContactSchema.MessageClass, text + ".", MatchOptions.Prefix, MatchFlags.IgnoreCase)
				});
			}
		}
	}
}
