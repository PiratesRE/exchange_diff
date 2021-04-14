using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class Query
	{
		public static Query BuildQuery(QueryFilter filter, IList<PropertyDefinition> rows, int maxDepth)
		{
			if (rows == null)
			{
				throw new ArgumentNullException("rows");
			}
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (maxDepth <= 0)
			{
				throw new ArgumentException("maxDepth");
			}
			Dictionary<PropertyDefinition, int> dictionary = new Dictionary<PropertyDefinition, int>();
			for (int i = 0; i < rows.Count; i++)
			{
				dictionary[rows[i]] = i;
			}
			return Query.BuildQueryInternal(filter, dictionary, maxDepth - 1);
		}

		public abstract bool IsMatch(object[] row);

		private static Query BuildQueryInternal(QueryFilter filter, Dictionary<PropertyDefinition, int> propertyDefinitionToColumnMap, int depth)
		{
			if (depth == 0)
			{
				throw new ArgumentException("Depth greater then max depth specified");
			}
			SinglePropertyFilter singlePropertyFilter = filter as SinglePropertyFilter;
			if (singlePropertyFilter != null)
			{
				return Query.BuildSinglePropertyQuery(singlePropertyFilter, propertyDefinitionToColumnMap);
			}
			CompositeFilter compositeFilter = filter as CompositeFilter;
			if (compositeFilter != null)
			{
				return Query.BuildCompositeFilter(compositeFilter, propertyDefinitionToColumnMap, depth - 1);
			}
			NotFilter notFilter = filter as NotFilter;
			if (notFilter != null)
			{
				return new NotQuery(Query.BuildQueryInternal(notFilter.Filter, propertyDefinitionToColumnMap, depth - 1));
			}
			return null;
		}

		private static Query BuildCompositeFilter(CompositeFilter compositeFilter, Dictionary<PropertyDefinition, int> propertyDefinitionToColumnMap, int depth)
		{
			List<Query> list = new List<Query>(compositeFilter.FilterCount);
			foreach (QueryFilter filter in compositeFilter.Filters)
			{
				list.Add(Query.BuildQueryInternal(filter, propertyDefinitionToColumnMap, depth));
			}
			if (compositeFilter is AndFilter)
			{
				return new AndQuery(list);
			}
			return new OrQuery(list);
		}

		private static Query BuildSinglePropertyQuery(SinglePropertyFilter singlePropertyFilter, Dictionary<PropertyDefinition, int> propertyDefinitionToColumnMap)
		{
			int index;
			if (!propertyDefinitionToColumnMap.TryGetValue(singlePropertyFilter.Property, out index))
			{
				throw new ArgumentException(string.Format("{0} specified in filter is not present in propertyDefinition columns", singlePropertyFilter.Property));
			}
			ComparisonFilter comparisonFilter = singlePropertyFilter as ComparisonFilter;
			if (comparisonFilter != null)
			{
				return Query.BuildComparisionQuery(comparisonFilter, index);
			}
			ExistsFilter existsFilter = singlePropertyFilter as ExistsFilter;
			if (existsFilter != null)
			{
				return new ExistsQuery(index);
			}
			throw new NotSupportedException(string.Format("Filter:({0}) not supported", singlePropertyFilter));
		}

		private static Query BuildComparisionQuery(ComparisonFilter comparisionFilter, int index)
		{
			IComparable comparable = comparisionFilter.PropertyValue as IComparable;
			switch (comparisionFilter.ComparisonOperator)
			{
			case ComparisonOperator.Equal:
				return new EqualsToComparisionQuery(index, comparisionFilter.PropertyValue);
			case ComparisonOperator.NotEqual:
				return new NotEqualsToComparisionQuery(index, comparisionFilter.PropertyValue);
			case ComparisonOperator.LessThan:
				if (comparable == null)
				{
					throw new ArgumentException(string.Format("comparisionFilter:{0} should have comparable type as value", comparisionFilter));
				}
				return new LessThenComparisionQuery(index, comparable);
			case ComparisonOperator.LessThanOrEqual:
				if (comparable == null)
				{
					throw new ArgumentException(string.Format("comparisionFilter:{0} should have comparable type as value", comparisionFilter));
				}
				return new LessThenOrEqualToComparisionQuery(index, comparable);
			case ComparisonOperator.GreaterThan:
				if (comparable == null)
				{
					throw new ArgumentException(string.Format("comparisionFilter:{0} should have comparable type as value", comparisionFilter));
				}
				return new GreaterThenComparisionQuery(index, comparable);
			case ComparisonOperator.GreaterThanOrEqual:
				if (comparable == null)
				{
					throw new ArgumentException(string.Format("comparisionFilter:{0} should have comparable type as value", comparisionFilter));
				}
				return new GreaterThenOrEqualToComparisionQuery(index, comparable);
			default:
				throw new InvalidProgramException();
			}
		}
	}
}
