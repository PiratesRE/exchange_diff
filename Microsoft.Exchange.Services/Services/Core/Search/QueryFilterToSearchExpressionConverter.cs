using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal class QueryFilterToSearchExpressionConverter : BaseFilterConverter<QueryFilter, SearchExpressionType>
	{
		static QueryFilterToSearchExpressionConverter()
		{
			QueryFilterToSearchExpressionConverter.queryFilterToConverterMap.Add(typeof(AndFilter), new AndFilterConverter());
			QueryFilterToSearchExpressionConverter.queryFilterToConverterMap.Add(typeof(OrFilter), new OrFilterConverter());
			QueryFilterToSearchExpressionConverter.queryFilterToConverterMap.Add(typeof(NotFilter), new NotFilterConverter());
			QueryFilterToSearchExpressionConverter.queryFilterToConverterMap.Add(typeof(BitMaskFilter), new BitmaskFilterConverter());
			QueryFilterToSearchExpressionConverter.queryFilterToConverterMap.Add(typeof(ExistsFilter), new ExistsFilterConverter());
			QueryFilterToSearchExpressionConverter.queryFilterToConverterMap.Add(typeof(SubFilter), new SubFilterConverter());
			QueryFilterToSearchExpressionConverter.queryFilterToConverterMap.Add(typeof(TextFilter), new TextFilterConverter());
			ComparisonFilterConverter value = new ComparisonFilterConverter();
			QueryFilterToSearchExpressionConverter.queryFilterToConverterMap.Add(typeof(ComparisonFilter), value);
			QueryFilterToSearchExpressionConverter.queryFilterToConverterMap.Add(typeof(PropertyComparisonFilter), value);
		}

		private static BaseSingleFilterConverter GetConverter(QueryFilter incomingFilter)
		{
			BaseSingleFilterConverter result = null;
			if (!QueryFilterToSearchExpressionConverter.queryFilterToConverterMap.TryGetValue(incomingFilter.GetType(), out result))
			{
				throw new UnsupportedQueryFilterException(CoreResources.IDs.ErrorUnsupportedQueryFilter);
			}
			return result;
		}

		public SearchExpressionType Convert(QueryFilter inputFilter)
		{
			if (inputFilter == null)
			{
				return null;
			}
			return base.InternalConvert(inputFilter);
		}

		protected override bool IsLeafExpression(QueryFilter inputFilter)
		{
			return QueryFilterToSearchExpressionConverter.GetConverter(inputFilter).IsLeafFilter;
		}

		protected override int GetFilterChildCount(QueryFilter inputFilter)
		{
			BaseNonLeafFilterConverter baseNonLeafFilterConverter = QueryFilterToSearchExpressionConverter.GetConverter(inputFilter) as BaseNonLeafFilterConverter;
			if (baseNonLeafFilterConverter == null)
			{
				return 0;
			}
			return baseNonLeafFilterConverter.GetQueryFilterChildCount(inputFilter);
		}

		protected override QueryFilter GetFilterChild(QueryFilter parentFilter, int childIndex)
		{
			BaseNonLeafFilterConverter baseNonLeafFilterConverter = QueryFilterToSearchExpressionConverter.GetConverter(parentFilter) as BaseNonLeafFilterConverter;
			if (baseNonLeafFilterConverter != null)
			{
				return baseNonLeafFilterConverter.GetQueryFilterChild(parentFilter, childIndex);
			}
			return null;
		}

		protected override void ThrowTooLongException()
		{
			throw new QueryFilterTooLongException();
		}

		protected override SearchExpressionType ConvertSingleElement(QueryFilter inputFilter, Stack<SearchExpressionType> workingStack)
		{
			BaseSingleFilterConverter converter = QueryFilterToSearchExpressionConverter.GetConverter(inputFilter);
			BaseLeafFilterConverter baseLeafFilterConverter = converter as BaseLeafFilterConverter;
			if (baseLeafFilterConverter != null)
			{
				return baseLeafFilterConverter.ConvertToSearchExpression(inputFilter);
			}
			BaseNonLeafFilterConverter baseNonLeafFilterConverter = (BaseNonLeafFilterConverter)converter;
			return baseNonLeafFilterConverter.ConvertToSearchExpresson(workingStack);
		}

		private static Dictionary<Type, BaseSingleFilterConverter> queryFilterToConverterMap = new Dictionary<Type, BaseSingleFilterConverter>();
	}
}
