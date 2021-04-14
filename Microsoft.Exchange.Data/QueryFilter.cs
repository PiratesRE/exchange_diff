using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public abstract class QueryFilter
	{
		internal static int[] InitializeFilterStringSizeEstimates()
		{
			int[] array = new int[256];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = 1024;
			}
			return array;
		}

		internal static string ConvertPropertyName(string propertyName)
		{
			string result = null;
			switch (propertyName)
			{
			case "SubjectProperty":
				result = DataStrings.SubjectProperty;
				break;
			case "TextBody":
				result = DataStrings.TextBody;
				break;
			case "AttachmentContent":
			case "Attachments":
			case "AttachFileName":
			case "AttachLongFileName":
			case "AttachExtension":
			case "DisplayName":
				result = DataStrings.AttachmentContent;
				break;
			case "SentTime":
				result = DataStrings.SentTime;
				break;
			case "ReceivedTime":
				result = DataStrings.ReceivedTime;
				break;
			case "SearchRecipientsTo":
				result = DataStrings.SearchRecipientsTo;
				break;
			case "SearchRecipientsCc":
				result = DataStrings.SearchRecipientsCc;
				break;
			case "SearchRecipientsBcc":
				result = DataStrings.SearchRecipientsBcc;
				break;
			case "SearchRecipients":
				result = DataStrings.SearchRecipients;
				break;
			case "SearchSender":
				result = DataStrings.SearchSender;
				break;
			case "Recipients":
				result = DataStrings.Recipients;
				break;
			case "ItemClass":
				result = DataStrings.ItemClass;
				break;
			case "ToGroupExpansionRecipients":
				result = DataStrings.ToGroupExpansionRecipients;
				break;
			case "CcGroupExpansionRecipients":
				result = DataStrings.CcGroupExpansionRecipients;
				break;
			case "BccGroupExpansionRecipients":
				result = DataStrings.BccGroupExpansionRecipients;
				break;
			case "GroupExpansionRecipients":
				result = DataStrings.GroupExpansionRecipients;
				break;
			}
			return result;
		}

		internal string GenerateInfixString(FilterLanguage language)
		{
			FilterSchema filterSchema;
			switch (language)
			{
			case FilterLanguage.Monad:
				filterSchema = QueryFilter.MonadLanguageSchema;
				goto IL_2C;
			case FilterLanguage.Kql:
				filterSchema = QueryFilter.KqlLanguageSchema;
				goto IL_2C;
			}
			filterSchema = QueryFilter.AdoLanguageSchema;
			IL_2C:
			StringBuilder stringBuilder = new StringBuilder();
			QueryFilter.GenerateInfixString(this, stringBuilder, filterSchema);
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			int num = this.Size % QueryFilter.filterStringSizeEstimates.Length;
			int num2 = QueryFilter.filterStringSizeEstimates[num];
			StringBuilder stringBuilder = new StringBuilder(num2);
			this.ToString(stringBuilder);
			if (stringBuilder.Length != num2)
			{
				Interlocked.Exchange(ref QueryFilter.filterStringSizeEstimates[num], stringBuilder.Length);
			}
			return stringBuilder.ToString();
		}

		public virtual IEnumerable<string> Keywords()
		{
			return new List<string>();
		}

		internal virtual IEnumerable<PropertyDefinition> FilterProperties()
		{
			return new List<PropertyDefinition>();
		}

		public virtual QueryFilter CloneWithPropertyReplacement(IDictionary<PropertyDefinition, PropertyDefinition> propertyMap)
		{
			throw new NotSupportedException("Cannot map filter of type" + base.GetType().ToString());
		}

		public abstract void ToString(StringBuilder sb);

		public virtual string PropertyName
		{
			get
			{
				return null;
			}
		}

		internal int Size
		{
			get
			{
				if (this.size == 0)
				{
					this.size = this.GetSize();
				}
				return this.size;
			}
		}

		internal bool IsAtomic { get; set; }

		protected virtual int GetSize()
		{
			return 1;
		}

		private static void GenerateInfixString(QueryFilter filter, StringBuilder sb, FilterSchema filterSchema)
		{
			if (filter is CompositeFilter)
			{
				sb.Append("(");
				CompositeFilter compositeFilter = (CompositeFilter)filter;
				int filterCount = compositeFilter.FilterCount;
				for (int i = 0; i < filterCount - 1; i++)
				{
					sb.Append("(");
					QueryFilter.GenerateInfixString(compositeFilter.Filters[i], sb, filterSchema);
					sb.Append(") ");
					if (filter is AndFilter)
					{
						sb.Append(filterSchema.And);
					}
					else if (filter is OrFilter)
					{
						sb.Append(filterSchema.Or);
					}
					sb.Append(" ");
				}
				sb.Append("(");
				QueryFilter.GenerateInfixString(compositeFilter.Filters[filterCount - 1], sb, filterSchema);
				sb.Append("))");
				return;
			}
			if (filter is NotFilter)
			{
				NotFilter notFilter = filter as NotFilter;
				sb.Append(filterSchema.Not);
				sb.Append("(");
				QueryFilter.GenerateInfixString(notFilter.Filter, sb, filterSchema);
				sb.Append(")");
				return;
			}
			if (filter is TextFilter)
			{
				TextFilter textFilter = filter as TextFilter;
				string propertyName = filterSchema.GetPropertyName(textFilter.Property.Name);
				if (!string.IsNullOrEmpty(propertyName))
				{
					sb.Append(propertyName);
					sb.Append(filterSchema.Like);
				}
				if (textFilter.MatchOptions == MatchOptions.FullString || textFilter.MatchOptions == MatchOptions.ExactPhrase || filterSchema.SupportQuotedPrefix)
				{
					sb.Append(filterSchema.QuotationMark);
				}
				if (textFilter.MatchOptions == MatchOptions.Suffix || textFilter.MatchOptions == MatchOptions.SubString)
				{
					sb.Append("*");
				}
				sb.Append(filterSchema.EscapeStringValue(textFilter.Text));
				if (textFilter.MatchOptions == MatchOptions.Prefix || textFilter.MatchOptions == MatchOptions.SubString || textFilter.MatchOptions == MatchOptions.PrefixOnWords)
				{
					sb.Append("*");
				}
				if (textFilter.MatchOptions == MatchOptions.FullString || textFilter.MatchOptions == MatchOptions.ExactPhrase || filterSchema.SupportQuotedPrefix)
				{
					sb.Append(filterSchema.QuotationMark);
					return;
				}
			}
			else
			{
				if (filter is ComparisonFilter)
				{
					ComparisonFilter comparisonFilter = filter as ComparisonFilter;
					sb.Append(filterSchema.GetPropertyName(comparisonFilter.Property.Name));
					sb.Append(filterSchema.GetRelationalOperator(comparisonFilter.ComparisonOperator));
					sb.Append(filterSchema.QuotationMark);
					sb.Append(filterSchema.EscapeStringValue(comparisonFilter.PropertyValue));
					sb.Append(filterSchema.QuotationMark);
					return;
				}
				if (filter is ExistsFilter)
				{
					sb.Append(filterSchema.GetExistsFilter(filter as ExistsFilter));
					return;
				}
				if (filter is FalseFilter)
				{
					sb.Append(filterSchema.GetFalseFilter());
				}
			}
		}

		private static QueryFilter SimplifyCompositeFilter<TFilter, TOther>(TFilter filter) where TFilter : CompositeFilter where TOther : CompositeFilter
		{
			List<QueryFilter> list = new List<QueryFilter>();
			Stack<QueryFilter> stack = new Stack<QueryFilter>();
			for (int i = filter.Filters.Count - 1; i >= 0; i--)
			{
				stack.Push(filter.Filters[i]);
			}
			while (stack.Count > 0)
			{
				QueryFilter queryFilter = stack.Pop();
				NotFilter notFilter = queryFilter as NotFilter;
				if (queryFilter.IsAtomic || (notFilter != null && notFilter.Filter.IsAtomic))
				{
					if (!list.Contains(queryFilter))
					{
						list.Add(queryFilter);
					}
				}
				else if (notFilter != null)
				{
					if (notFilter.Filter is TOther)
					{
						TOther tother = notFilter.Filter as TOther;
						for (int j = tother.Filters.Count - 1; j >= 0; j--)
						{
							QueryFilter filter2 = tother.Filters[j];
							stack.Push(QueryFilter.NotFilter(filter2));
						}
					}
					else if (notFilter.Filter is NotFilter)
					{
						stack.Push((notFilter.Filter as NotFilter).Filter);
					}
					else
					{
						QueryFilter item = QueryFilter.SimplifyFilter(queryFilter);
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
				else if (queryFilter is TFilter)
				{
					TFilter tfilter = queryFilter as TFilter;
					for (int k = tfilter.Filters.Count - 1; k >= 0; k--)
					{
						QueryFilter item2 = tfilter.Filters[k];
						stack.Push(item2);
					}
				}
				else
				{
					QueryFilter item3 = QueryFilter.SimplifyFilter(queryFilter);
					if (!list.Contains(item3))
					{
						list.Add(item3);
					}
				}
			}
			if (typeof(TFilter).Equals(typeof(AndFilter)))
			{
				return QueryFilter.AndTogether(list.ToArray());
			}
			return QueryFilter.OrTogether(list.ToArray());
		}

		internal static QueryFilter SimplifyFilter(QueryFilter filter)
		{
			if (filter == null)
			{
				return null;
			}
			if (filter.IsAtomic)
			{
				return filter;
			}
			if (!(filter is NotFilter) && !(filter is CompositeFilter))
			{
				return filter;
			}
			NotFilter notFilter = filter as NotFilter;
			if (notFilter != null)
			{
				QueryFilter filter2 = notFilter.Filter;
				if (filter2.IsAtomic)
				{
					return filter;
				}
				if (filter2 is NotFilter)
				{
					return QueryFilter.SimplifyFilter(((NotFilter)filter2).Filter);
				}
				if (filter2 is CompositeFilter)
				{
					CompositeFilter compositeFilter = (CompositeFilter)filter2;
					QueryFilter[] array = new QueryFilter[compositeFilter.Filters.Count];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = QueryFilter.NotFilter(compositeFilter.Filters[i]);
					}
					if (filter2 is AndFilter)
					{
						return QueryFilter.SimplifyFilter(QueryFilter.OrTogether(array));
					}
					if (filter2 is OrFilter)
					{
						return QueryFilter.SimplifyFilter(QueryFilter.AndTogether(array));
					}
				}
				if (filter2 is ComparisonFilter)
				{
					ComparisonFilter comparisonFilter = (ComparisonFilter)filter2;
					ComparisonOperator comparisonOperator;
					switch (comparisonFilter.ComparisonOperator)
					{
					case ComparisonOperator.Equal:
						comparisonOperator = ComparisonOperator.NotEqual;
						break;
					case ComparisonOperator.NotEqual:
						comparisonOperator = ComparisonOperator.Equal;
						break;
					case ComparisonOperator.LessThan:
						comparisonOperator = ComparisonOperator.GreaterThanOrEqual;
						break;
					case ComparisonOperator.LessThanOrEqual:
						comparisonOperator = ComparisonOperator.GreaterThan;
						break;
					case ComparisonOperator.GreaterThan:
						comparisonOperator = ComparisonOperator.LessThanOrEqual;
						break;
					case ComparisonOperator.GreaterThanOrEqual:
						comparisonOperator = ComparisonOperator.LessThan;
						break;
					default:
						return filter;
					}
					return new ComparisonFilter(comparisonOperator, comparisonFilter.Property, comparisonFilter.PropertyValue);
				}
				return QueryFilter.NotFilter(QueryFilter.SimplifyFilter(filter2));
			}
			else
			{
				AndFilter andFilter = filter as AndFilter;
				if (andFilter != null)
				{
					return QueryFilter.SimplifyCompositeFilter<AndFilter, OrFilter>(andFilter);
				}
				OrFilter orFilter = filter as OrFilter;
				if (orFilter != null)
				{
					return QueryFilter.SimplifyCompositeFilter<OrFilter, AndFilter>(orFilter);
				}
				return filter;
			}
		}

		internal static QueryFilter AndTogether(params QueryFilter[] filters)
		{
			return QueryFilter.AndOrTogether((QueryFilter[] list) => new AndFilter(list), filters);
		}

		internal static QueryFilter OrTogether(params QueryFilter[] filters)
		{
			return QueryFilter.AndOrTogether((QueryFilter[] list) => new OrFilter(list), filters);
		}

		private static QueryFilter AndOrTogether(Func<QueryFilter[], QueryFilter> ctor, params QueryFilter[] filters)
		{
			if (filters == null)
			{
				throw new ArgumentNullException("filters");
			}
			List<QueryFilter> list = new List<QueryFilter>(filters.Length);
			foreach (QueryFilter queryFilter in filters)
			{
				if (queryFilter != null)
				{
					list.Add(queryFilter);
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			if (list.Count == 1)
			{
				return list[0];
			}
			return ctor(list.ToArray());
		}

		internal static QueryFilter NotFilter(QueryFilter filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (filter is NotFilter)
			{
				return (filter as NotFilter).Filter;
			}
			return new NotFilter(filter);
		}

		private const int defaultFilterStringSizeEstimate = 1024;

		private const int numberOfFilterStringSizeEstimates = 256;

		public static QueryFilter True = new TrueFilter();

		public static QueryFilter False = new FalseFilter();

		private static FilterSchema MonadLanguageSchema = new MonadFilterSchema();

		private static FilterSchema AdoLanguageSchema = new AdoFilterSchema();

		private static FilterSchema KqlLanguageSchema = new KqlFilterSchema();

		private int size;

		internal static int[] filterStringSizeEstimates = QueryFilter.InitializeFilterStringSizeEstimates();
	}
}
