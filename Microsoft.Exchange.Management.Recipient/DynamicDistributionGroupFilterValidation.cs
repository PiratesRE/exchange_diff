using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal static class DynamicDistributionGroupFilterValidation
	{
		internal static bool ContainsNonOptimizedCondition(QueryFilter filter, out LocalizedString? errorMessage)
		{
			TextFilter[] array = DynamicDistributionGroupFilterValidation.ExtractNonOptimizedCondition(filter);
			errorMessage = ((array == null) ? null : new LocalizedString?(Strings.DDGFilterIsNonoptimized));
			return errorMessage != null;
		}

		internal static bool IsFullOptimizedOrImproved(QueryFilter oldFilter, QueryFilter newFilter, out LocalizedString? errorMessage)
		{
			errorMessage = null;
			TextFilter[] array = DynamicDistributionGroupFilterValidation.ExtractNonOptimizedCondition(newFilter);
			if (array != null)
			{
				TextFilter[] array2 = DynamicDistributionGroupFilterValidation.ExtractNonOptimizedCondition(oldFilter);
				if (array2 == null)
				{
					errorMessage = new LocalizedString?(Strings.DDGFilterIsNonoptimized);
				}
				else if (array2.Length <= array.Length)
				{
					errorMessage = new LocalizedString?(Strings.NewFilterNeitherOptimizedNorImproved);
				}
			}
			return errorMessage == null;
		}

		private static TextFilter[] ExtractNonOptimizedCondition(QueryFilter filter)
		{
			List<TextFilter> list = null;
			Queue<QueryFilter> queue = new Queue<QueryFilter>();
			while (filter != null)
			{
				if (filter is CompositeFilter)
				{
					using (ReadOnlyCollection<QueryFilter>.Enumerator enumerator = ((CompositeFilter)filter).Filters.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							QueryFilter item = enumerator.Current;
							queue.Enqueue(item);
						}
						goto IL_D0;
					}
					goto IL_54;
				}
				goto IL_54;
				IL_D0:
				filter = ((queue.Count > 0) ? queue.Dequeue() : null);
				continue;
				IL_54:
				if (filter is NotFilter)
				{
					queue.Enqueue(((NotFilter)filter).Filter);
					goto IL_D0;
				}
				if (filter is TextFilter)
				{
					TextFilter textFilter = (TextFilter)filter;
					if (textFilter.MatchOptions == MatchOptions.SubString || textFilter.MatchOptions == MatchOptions.Suffix)
					{
						if (list == null)
						{
							list = new List<TextFilter>();
						}
						list.Add(textFilter);
						goto IL_D0;
					}
					goto IL_D0;
				}
				else
				{
					if (!(filter is ComparisonFilter) && !(filter is ExistsFilter) && !(filter is FalseFilter))
					{
						throw new NotSupportedException("Unsupported filter type: " + filter.GetType());
					}
					goto IL_D0;
				}
			}
			if (list == null)
			{
				return null;
			}
			return list.ToArray();
		}

		private static string FiltersToString(TextFilter[] filters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (TextFilter textFilter in filters)
			{
				stringBuilder.Append(textFilter.GenerateInfixString(FilterLanguage.Monad));
				stringBuilder.Append(", ");
			}
			stringBuilder.Length -= ", ".Length;
			return stringBuilder.ToString();
		}

		private const string InfixDelimiter = ", ";
	}
}
