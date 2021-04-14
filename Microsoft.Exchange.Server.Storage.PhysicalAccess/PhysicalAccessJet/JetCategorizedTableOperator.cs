using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetCategorizedTableOperator : CategorizedTableOperator, IJetRecordCounter
	{
		internal JetCategorizedTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, CategorizedTableParams categorizedTableParams, CategorizedTableCollapseState collapseState, IList<Column> columnsToFetch, IReadOnlyDictionary<Column, Column> additionalHeaderRenameDictionary, IReadOnlyDictionary<Column, Column> additionalLeafRenameDictionary, SearchCriteria restriction, int skipTo, int maxRows, KeyRange keyRange, bool backwards, bool frequentOperation) : this(connectionProvider, new CategorizedTableOperator.CategorizedTableOperatorDefinition(culture, table, categorizedTableParams, collapseState, columnsToFetch, additionalHeaderRenameDictionary, additionalLeafRenameDictionary, (restriction is SearchCriteriaTrue) ? null : restriction, skipTo, maxRows, keyRange, backwards, frequentOperation))
		{
		}

		internal JetCategorizedTableOperator(IConnectionProvider connectionProvider, CategorizedTableOperator.CategorizedTableOperatorDefinition definition) : base(connectionProvider, definition)
		{
		}

		int IJetRecordCounter.GetCount()
		{
			Column contentCountColumn = base.GetContentCountColumn();
			int num = 0;
			int num2;
			bool flag = base.MoveFirst(out num2);
			while (flag)
			{
				num++;
				if (base.IsLeafVisible())
				{
					num += base.HeaderReader.GetInt32(contentCountColumn);
				}
				flag = base.MoveNext("MoveNextForCount", 0, true, ref num2);
			}
			return num;
		}

		int IJetRecordCounter.GetOrdinalPosition(SortOrder sortOrder, StartStopKey stopKey, CompareInfo compareInfo)
		{
			Column contentCountColumn = base.GetContentCountColumn();
			bool flag = this.IsLeafBookmark(sortOrder, stopKey);
			Func<SortOrder, StartStopKey, CompareInfo, int> func = flag ? new Func<SortOrder, StartStopKey, CompareInfo, int>(this.CompareHeaderForLeafBookmark) : new Func<SortOrder, StartStopKey, CompareInfo, int>(this.CompareToStopKey);
			int num = 0;
			int num2;
			bool flag2 = base.MoveFirst(out num2);
			while (flag2)
			{
				int num3 = func(sortOrder, stopKey, compareInfo);
				if (num3 < 0)
				{
					if (base.IsLeafVisible())
					{
						num += base.HeaderReader.GetInt32(contentCountColumn);
					}
					num++;
					flag2 = base.MoveNext("MoveNextForHeaderOrdinalPosition", 0, true, ref num2);
				}
				else
				{
					if (num3 > 0)
					{
						break;
					}
					if (flag)
					{
						num++;
						if (base.IsLeafVisible())
						{
							int num4 = 0;
							while (base.MoveNext("MoveNextForLeafOrdinalPosition", 0, false, ref num2))
							{
								if (base.LeafReader != null)
								{
									num3 = this.CompareToStopKey(sortOrder, stopKey, compareInfo);
									if (num3 > 0 || (num3 == 0 && stopKey.Inclusive))
									{
										break;
									}
									num4++;
								}
							}
							num += num4;
							break;
						}
						break;
					}
					else
					{
						if (!stopKey.Inclusive)
						{
							num++;
							break;
						}
						break;
					}
				}
			}
			return num;
		}

		private bool IsLeafBookmark(SortOrder sortOrder, StartStopKey stopKey)
		{
			if (stopKey.Count >= base.HeaderLogicalSortOrder.Count)
			{
				int num = (int)stopKey.Values[base.HeaderLogicalSortOrder.Count - 1];
				return num == base.CategoryCount;
			}
			return false;
		}

		private int CompareToStopKey(SortOrder sortOrder, StartStopKey stopKey, CompareInfo compareInfo)
		{
			int i = 0;
			while (i < stopKey.Count)
			{
				Column column = sortOrder.Columns[i];
				object value = base.CurrentReader(column).GetValue(column);
				int num = ValueHelper.ValuesCompare(value, stopKey.Values[i], compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
				if (num != 0)
				{
					if (!sortOrder.Ascending[i])
					{
						return -num;
					}
					return num;
				}
				else
				{
					i++;
				}
			}
			if (stopKey.Count >= sortOrder.Count)
			{
				return 0;
			}
			return 1;
		}

		private int CompareHeaderForLeafBookmark(SortOrder sortOrder, StartStopKey stopKey, CompareInfo compareInfo)
		{
			int i = 0;
			while (i < base.HeaderLogicalSortOrder.Count - 1)
			{
				object value = base.HeaderReader.GetValue(sortOrder.Columns[i]);
				int num = ValueHelper.ValuesCompare(value, stopKey.Values[i], compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
				if (num != 0)
				{
					if (!sortOrder.Ascending[i])
					{
						return -num;
					}
					return num;
				}
				else
				{
					i++;
				}
			}
			if (!base.IsLowestHeaderLevel())
			{
				return -1;
			}
			return 0;
		}
	}
}
