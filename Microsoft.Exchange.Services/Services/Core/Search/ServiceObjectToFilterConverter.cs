using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal class ServiceObjectToFilterConverter : BaseFilterConverter<SearchExpressionType, QueryFilter>
	{
		static ServiceObjectToFilterConverter()
		{
			ServiceObjectToFilterConverter.elementNameToConverterMap.Add("And", new AndFilterConverter());
			ServiceObjectToFilterConverter.elementNameToConverterMap.Add("Or", new OrFilterConverter());
			ServiceObjectToFilterConverter.elementNameToConverterMap.Add("Not", new NotFilterConverter());
			ServiceObjectToFilterConverter.elementNameToConverterMap.Add("Excludes", new BitmaskFilterConverter());
			ComparisonFilterConverter value = new ComparisonFilterConverter();
			ServiceObjectToFilterConverter.elementNameToConverterMap.Add("IsEqualTo", value);
			ServiceObjectToFilterConverter.elementNameToConverterMap.Add("IsNotEqualTo", value);
			ServiceObjectToFilterConverter.elementNameToConverterMap.Add("IsGreaterThan", value);
			ServiceObjectToFilterConverter.elementNameToConverterMap.Add("IsGreaterThanOrEqualTo", value);
			ServiceObjectToFilterConverter.elementNameToConverterMap.Add("IsLessThan", value);
			ServiceObjectToFilterConverter.elementNameToConverterMap.Add("IsLessThanOrEqualTo", value);
			ServiceObjectToFilterConverter.elementNameToConverterMap.Add("Exists", new ExistsFilterConverter());
			ServiceObjectToFilterConverter.elementNameToConverterMap.Add("Contains", new TextFilterConverter());
		}

		private static BaseSingleFilterConverter GetConverter(string elementName)
		{
			BaseSingleFilterConverter result = null;
			if (!ServiceObjectToFilterConverter.elementNameToConverterMap.TryGetValue(elementName, out result))
			{
				throw new InvalidRestrictionException(CoreResources.IDs.ErrorInvalidRestriction);
			}
			return result;
		}

		public QueryFilter Convert(SearchExpressionType restrictionElement)
		{
			if (restrictionElement == null)
			{
				return null;
			}
			this.unrolledBitmasks.Clear();
			return base.InternalConvert(restrictionElement);
		}

		protected override int GetFilterChildCount(SearchExpressionType parentFilter)
		{
			INonLeafSearchExpressionType nonLeafSearchExpressionType = parentFilter as INonLeafSearchExpressionType;
			if (nonLeafSearchExpressionType == null)
			{
				return 0;
			}
			return nonLeafSearchExpressionType.Items.Length;
		}

		protected override SearchExpressionType GetFilterChild(SearchExpressionType parentFilter, int childIndex)
		{
			INonLeafSearchExpressionType nonLeafSearchExpressionType = parentFilter as INonLeafSearchExpressionType;
			if (nonLeafSearchExpressionType == null || childIndex >= nonLeafSearchExpressionType.Items.Length)
			{
				return null;
			}
			return nonLeafSearchExpressionType.Items[childIndex];
		}

		protected override void ThrowTooLongException()
		{
			throw new RestrictionTooLongException();
		}

		protected override QueryFilter ConvertSingleElement(SearchExpressionType currentElement, Stack<QueryFilter> workingStack)
		{
			BaseSingleFilterConverter converter = ServiceObjectToFilterConverter.GetConverter(currentElement.FilterType);
			BaseLeafFilterConverter baseLeafFilterConverter = converter as BaseLeafFilterConverter;
			if (baseLeafFilterConverter != null)
			{
				return baseLeafFilterConverter.ConvertToQueryFilter(currentElement);
			}
			if (currentElement.FilterType == "Not" && workingStack.Peek() is BitMaskFilter)
			{
				BitMaskFilter bitMaskFilter = (BitMaskFilter)workingStack.Peek();
				if (!this.HasBeenUnrolled(bitMaskFilter))
				{
					this.unrolledBitmasks.Add(bitMaskFilter);
					return bitMaskFilter;
				}
			}
			BaseNonLeafFilterConverter baseNonLeafFilterConverter = (BaseNonLeafFilterConverter)converter;
			return baseNonLeafFilterConverter.ConvertToQueryFilter(workingStack);
		}

		private bool HasBeenUnrolled(BitMaskFilter bitmaskFilter)
		{
			foreach (BitMaskFilter objA in this.unrolledBitmasks)
			{
				if (object.ReferenceEquals(objA, bitmaskFilter))
				{
					return true;
				}
			}
			return false;
		}

		protected override bool IsLeafExpression(SearchExpressionType expressionElement)
		{
			return !(expressionElement is INonLeafSearchExpressionType);
		}

		private static Dictionary<string, BaseSingleFilterConverter> elementNameToConverterMap = new Dictionary<string, BaseSingleFilterConverter>();

		private List<BitMaskFilter> unrolledBitmasks = new List<BitMaskFilter>();
	}
}
