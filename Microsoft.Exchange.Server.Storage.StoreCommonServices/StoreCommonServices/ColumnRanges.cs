using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class ColumnRanges
	{
		public ColumnRanges()
		{
		}

		public ColumnRanges(SearchCriteria criteria, CompareInfo compareInfo)
		{
			if (criteria != null)
			{
				criteria = criteria.InspectAndFix(null, compareInfo, true);
				if (criteria is SearchCriteriaAnd || criteria is SearchCriteriaNear || criteria is SearchCriteriaCompare)
				{
					criteria.InspectAndFix(delegate(SearchCriteria criterion, CompareInfo innerCompareInfo)
					{
						if (criterion is SearchCriteriaAnd || criterion is SearchCriteriaNear)
						{
							return criterion;
						}
						if (criterion is SearchCriteriaOr)
						{
							ColumnRanges columnRanges = new ColumnRanges(criterion, innerCompareInfo);
							if (columnRanges.columnSet == null)
							{
								goto IL_1D5;
							}
							if (columnRanges.columnSet.Count == 0)
							{
								this.columnSet = new Dictionary<Column, List<ValueRange>>(0);
								return null;
							}
							using (Dictionary<Column, List<ValueRange>>.Enumerator enumerator2 = columnRanges.columnSet.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									KeyValuePair<Column, List<ValueRange>> keyValuePair2 = enumerator2.Current;
									List<ValueRange> list = null;
									if (this.columnSet != null)
									{
										this.columnSet.TryGetValue(keyValuePair2.Key, out list);
									}
									list = ColumnRanges.IntersectRanges(list, keyValuePair2.Value, keyValuePair2.Key, innerCompareInfo);
									if (list != null)
									{
										if (this.columnSet == null)
										{
											this.columnSet = new Dictionary<Column, List<ValueRange>>();
										}
										this.columnSet[keyValuePair2.Key] = list;
									}
								}
								goto IL_1D5;
							}
						}
						if (criterion is SearchCriteriaCompare)
						{
							SearchCriteriaCompare searchCriteriaCompare = (SearchCriteriaCompare)criterion;
							ConstantColumn constantColumn = searchCriteriaCompare.Rhs as ConstantColumn;
							if (constantColumn != null && (byte)(searchCriteriaCompare.Lhs.ExtendedTypeCode & ExtendedTypeCode.MVFlag) == 0 && (byte)(constantColumn.ExtendedTypeCode & ExtendedTypeCode.MVFlag) == 0)
							{
								Column lhs = searchCriteriaCompare.Lhs;
								List<ValueRange> list2 = null;
								if (this.columnSet != null)
								{
									this.columnSet.TryGetValue(lhs, out list2);
								}
								if (ColumnRanges.IsSingleValueRange(searchCriteriaCompare.RelOp, constantColumn.Value))
								{
									ValueRange newRange = ColumnRanges.BuildValueRange(searchCriteriaCompare.RelOp, constantColumn.Value, lhs, innerCompareInfo);
									list2 = ColumnRanges.IntersectRanges(list2, newRange, lhs, innerCompareInfo);
								}
								else
								{
									List<ValueRange> newRanges = ColumnRanges.BuildValueRanges(searchCriteriaCompare.RelOp, constantColumn.Value, lhs, innerCompareInfo);
									list2 = ColumnRanges.IntersectRanges(list2, newRanges, lhs, innerCompareInfo);
								}
								if (list2 != null)
								{
									if (this.columnSet == null)
									{
										this.columnSet = new Dictionary<Column, List<ValueRange>>();
									}
									this.columnSet[lhs] = list2;
								}
							}
						}
						IL_1D5:
						return null;
					}, compareInfo, true);
				}
				else if (criteria is SearchCriteriaOr)
				{
					Column column = null;
					List<ValueRange> ranges = null;
					bool tooComplex = false;
					criteria.InspectAndFix(delegate(SearchCriteria criterion, CompareInfo innerCompareInfo)
					{
						if (tooComplex)
						{
							return null;
						}
						if (criterion is SearchCriteriaOr)
						{
							return criterion;
						}
						if (criterion is SearchCriteriaAnd)
						{
							SearchCriteriaAnd searchCriteriaAnd = (SearchCriteriaAnd)criterion;
							List<ValueRange> list = null;
							for (int i = 0; i < searchCriteriaAnd.NestedCriteria.Length; i++)
							{
								SearchCriteriaCompare searchCriteriaCompare = searchCriteriaAnd.NestedCriteria[i] as SearchCriteriaCompare;
								if (searchCriteriaCompare != null)
								{
									if (column == null)
									{
										if ((byte)(searchCriteriaCompare.Lhs.ExtendedTypeCode & ExtendedTypeCode.MVFlag) == 16)
										{
											goto IL_11B;
										}
										column = searchCriteriaCompare.Lhs;
									}
									ConstantColumn constantColumn = searchCriteriaCompare.Rhs as ConstantColumn;
									if (!(constantColumn == null) && !(searchCriteriaCompare.Lhs != column) && (byte)(constantColumn.ExtendedTypeCode & ExtendedTypeCode.MVFlag) != 16)
									{
										if (ColumnRanges.IsSingleValueRange(searchCriteriaCompare.RelOp, constantColumn.Value))
										{
											ValueRange newRange = ColumnRanges.BuildValueRange(searchCriteriaCompare.RelOp, constantColumn.Value, column, innerCompareInfo);
											list = ColumnRanges.IntersectRanges(list, newRange, column, innerCompareInfo);
										}
										else
										{
											List<ValueRange> newRanges = ColumnRanges.BuildValueRanges(searchCriteriaCompare.RelOp, constantColumn.Value, column, innerCompareInfo);
											list = ColumnRanges.IntersectRanges(list, newRanges, column, innerCompareInfo);
										}
									}
								}
								IL_11B:;
							}
							if (ranges == null && list != null)
							{
								ranges = new List<ValueRange>();
							}
							ranges = ColumnRanges.UnionRanges(ranges, list, column, innerCompareInfo);
							tooComplex = (ranges == null);
							return null;
						}
						if (!(criterion is SearchCriteriaCompare))
						{
							tooComplex = true;
							return null;
						}
						SearchCriteriaCompare searchCriteriaCompare2 = (SearchCriteriaCompare)criterion;
						if (column == null)
						{
							if ((byte)(searchCriteriaCompare2.Lhs.ExtendedTypeCode & ExtendedTypeCode.MVFlag) == 16)
							{
								tooComplex = true;
								return null;
							}
							column = searchCriteriaCompare2.Lhs;
						}
						ConstantColumn constantColumn2 = searchCriteriaCompare2.Rhs as ConstantColumn;
						if (constantColumn2 == null || searchCriteriaCompare2.Lhs != column || (byte)(constantColumn2.ExtendedTypeCode & ExtendedTypeCode.MVFlag) == 16)
						{
							tooComplex = true;
							return null;
						}
						if (ranges == null)
						{
							ranges = new List<ValueRange>();
						}
						if (ColumnRanges.IsSingleValueRange(searchCriteriaCompare2.RelOp, constantColumn2.Value))
						{
							ValueRange newRange2 = ColumnRanges.BuildValueRange(searchCriteriaCompare2.RelOp, constantColumn2.Value, column, innerCompareInfo);
							ranges = ColumnRanges.UnionRanges(ranges, newRange2, column, innerCompareInfo);
						}
						else
						{
							List<ValueRange> newRanges2 = ColumnRanges.BuildValueRanges(searchCriteriaCompare2.RelOp, constantColumn2.Value, column, innerCompareInfo);
							ranges = ColumnRanges.UnionRanges(ranges, newRanges2, column, innerCompareInfo);
						}
						tooComplex = (ranges == null);
						return null;
					}, compareInfo, true);
					if (!tooComplex && column != null && ranges != null)
					{
						this.columnSet = new Dictionary<Column, List<ValueRange>>(1)
						{
							{
								column,
								ranges
							}
						};
					}
				}
				if (this.columnSet != null)
				{
					ColumnRanges.MinRangeComparer minRangeComparer = null;
					foreach (KeyValuePair<Column, List<ValueRange>> keyValuePair in this.columnSet)
					{
						if (keyValuePair.Value.Count == 0)
						{
							this.columnSet.Clear();
							break;
						}
						if (minRangeComparer == null)
						{
							minRangeComparer = new ColumnRanges.MinRangeComparer();
						}
						minRangeComparer.Reconfigure(keyValuePair.Key, compareInfo);
						keyValuePair.Value.Sort(minRangeComparer);
					}
				}
			}
		}

		public bool IsEquivalentToFalse
		{
			get
			{
				return this.columnSet != null && this.columnSet.Count == 0;
			}
		}

		public bool IsEquivalentToTrue
		{
			get
			{
				return this.columnSet == null;
			}
		}

		public int Count
		{
			get
			{
				if (this.columnSet != null)
				{
					return this.columnSet.Count;
				}
				return 0;
			}
		}

		public List<ValueRange> this[Column column]
		{
			get
			{
				List<ValueRange> result;
				if (this.columnSet == null || !this.columnSet.TryGetValue(column, out result))
				{
					return null;
				}
				return result;
			}
		}

		public SearchCriteria Criteria
		{
			get
			{
				if (this.columnSet == null)
				{
					return Factory.CreateSearchCriteriaTrue();
				}
				if (this.columnSet.Count == 0)
				{
					return Factory.CreateSearchCriteriaFalse();
				}
				List<SearchCriteria> list = new List<SearchCriteria>(this.columnSet.Count);
				foreach (KeyValuePair<Column, List<ValueRange>> keyValuePair in this.columnSet)
				{
					List<SearchCriteria> list2 = new List<SearchCriteria>(this.columnSet.Count);
					foreach (ValueRange valueRange in keyValuePair.Value)
					{
						if (!valueRange.MinValue.IsInfinity && !valueRange.MaxValue.IsInfinity && valueRange.MinValue.IsInclusive && valueRange.MaxValue.IsInclusive && ValueHelper.ValuesEqual(valueRange.MinValue.Value, valueRange.MaxValue.Value))
						{
							list2.Add(Factory.CreateSearchCriteriaCompare(keyValuePair.Key, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(valueRange.MinValue.Value, keyValuePair.Key)));
						}
						else
						{
							SearchCriteria searchCriteria = null;
							if (!valueRange.MinValue.IsInfinity)
							{
								SearchCriteriaCompare.SearchRelOp op = valueRange.MinValue.IsInclusive ? SearchCriteriaCompare.SearchRelOp.GreaterThanEqual : SearchCriteriaCompare.SearchRelOp.GreaterThan;
								searchCriteria = Factory.CreateSearchCriteriaCompare(keyValuePair.Key, op, Factory.CreateConstantColumn(valueRange.MinValue.Value, keyValuePair.Key));
							}
							SearchCriteria searchCriteria2 = null;
							if (!valueRange.MaxValue.IsInfinity)
							{
								SearchCriteriaCompare.SearchRelOp op2 = valueRange.MaxValue.IsInclusive ? SearchCriteriaCompare.SearchRelOp.LessThanEqual : SearchCriteriaCompare.SearchRelOp.LessThan;
								searchCriteria2 = Factory.CreateSearchCriteriaCompare(keyValuePair.Key, op2, Factory.CreateConstantColumn(valueRange.MaxValue.Value, keyValuePair.Key));
							}
							if (searchCriteria != null && searchCriteria2 != null)
							{
								list2.Add(Factory.CreateSearchCriteriaAnd(new SearchCriteria[]
								{
									searchCriteria,
									searchCriteria2
								}));
							}
							else if (searchCriteria != null)
							{
								list2.Add(searchCriteria);
							}
							else
							{
								list2.Add(searchCriteria2);
							}
						}
					}
					if (list2.Count == 1)
					{
						list.Add(list2[0]);
					}
					else
					{
						list.Add(Factory.CreateSearchCriteriaOr(list2.ToArray()));
					}
				}
				if (list.Count == 1)
				{
					return list[0];
				}
				return Factory.CreateSearchCriteriaAnd(list.ToArray()).InspectAndFix(null, null, true);
			}
		}

		public void Add(Column column, List<ValueRange> ranges)
		{
			if (this.columnSet == null)
			{
				this.columnSet = new Dictionary<Column, List<ValueRange>>(4);
				if (ranges.Count != 0)
				{
					this.columnSet.Add(column, ranges);
					return;
				}
			}
			else if (this.columnSet.Count != 0)
			{
				if (ranges.Count != 0)
				{
					this.columnSet.Add(column, ranges);
					return;
				}
				this.columnSet.Clear();
			}
		}

		public SearchCriteria SimplifyCriteria(SearchCriteria criteria, CompareInfo compareInfo)
		{
			if (criteria == null)
			{
				return null;
			}
			if (this.columnSet != null && this.columnSet.Count == 0)
			{
				return Factory.CreateSearchCriteriaFalse();
			}
			criteria = criteria.InspectAndFix(null, compareInfo, true);
			return criteria.InspectAndFix(delegate(SearchCriteria criterion, CompareInfo innerCompareInfo)
			{
				if (criterion is SearchCriteriaAnd)
				{
					return criterion;
				}
				if (criterion is SearchCriteriaOr)
				{
					ColumnRanges columnRanges = new ColumnRanges(criterion, innerCompareInfo);
					if (columnRanges.columnSet != null)
					{
						if (columnRanges.columnSet.Count == 0)
						{
							return Factory.CreateSearchCriteriaFalse();
						}
						using (Dictionary<Column, List<ValueRange>>.Enumerator enumerator = columnRanges.columnSet.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								KeyValuePair<Column, List<ValueRange>> keyValuePair = enumerator.Current;
								List<ValueRange> ranges = null;
								if (this.columnSet != null && this.columnSet.TryGetValue(keyValuePair.Key, out ranges))
								{
									bool flag;
									if (!ColumnRanges.TestIntersectRanges(ranges, keyValuePair.Value, keyValuePair.Key, innerCompareInfo, out flag))
									{
										if (ColumnRanges.IsSimpleOrCriteria(criterion, innerCompareInfo))
										{
											return Factory.CreateSearchCriteriaTrue();
										}
									}
									else if (flag)
									{
										return Factory.CreateSearchCriteriaFalse();
									}
								}
							}
							return criterion;
						}
					}
					if (ColumnRanges.IsSimpleOrCriteria(criterion, innerCompareInfo))
					{
						return Factory.CreateSearchCriteriaTrue();
					}
					return criterion;
				}
				if (criterion is SearchCriteriaCompare)
				{
					SearchCriteriaCompare searchCriteriaCompare = (SearchCriteriaCompare)criterion;
					ConstantColumn constantColumn = searchCriteriaCompare.Rhs as ConstantColumn;
					if (constantColumn != null)
					{
						Column lhs = searchCriteriaCompare.Lhs;
						List<ValueRange> ranges2 = null;
						if (this.columnSet != null && this.columnSet.TryGetValue(lhs, out ranges2))
						{
							bool flag3;
							bool flag2;
							if (ColumnRanges.IsSingleValueRange(searchCriteriaCompare.RelOp, constantColumn.Value))
							{
								ValueRange newRange = ColumnRanges.BuildValueRange(searchCriteriaCompare.RelOp, constantColumn.Value, lhs, innerCompareInfo);
								flag2 = ColumnRanges.TestIntersectRanges(ranges2, newRange, lhs, innerCompareInfo, out flag3);
							}
							else
							{
								List<ValueRange> newRanges = ColumnRanges.BuildValueRanges(searchCriteriaCompare.RelOp, constantColumn.Value, lhs, innerCompareInfo);
								flag2 = ColumnRanges.TestIntersectRanges(ranges2, newRanges, lhs, innerCompareInfo, out flag3);
							}
							if (!flag2)
							{
								return Factory.CreateSearchCriteriaTrue();
							}
							if (flag3)
							{
								return Factory.CreateSearchCriteriaFalse();
							}
						}
						else if (ColumnRanges.IsSingleValueRange(searchCriteriaCompare.RelOp, constantColumn.Value))
						{
							ValueRange valueRange = ColumnRanges.BuildValueRange(searchCriteriaCompare.RelOp, constantColumn.Value, lhs, innerCompareInfo);
							if (valueRange.IsFull)
							{
								return Factory.CreateSearchCriteriaTrue();
							}
							if (valueRange.IsEmpty)
							{
								return Factory.CreateSearchCriteriaFalse();
							}
						}
					}
				}
				return null;
			}, compareInfo, true);
		}

		private static bool IsSimpleOrCriteria(SearchCriteria criteria, CompareInfo compareInfo)
		{
			bool flag = true;
			Column col = null;
			SearchCriteriaOr searchCriteriaOr = (SearchCriteriaOr)criteria;
			foreach (SearchCriteria searchCriteria in searchCriteriaOr.NestedCriteria)
			{
				if (searchCriteria is SearchCriteriaCompare)
				{
					SearchCriteriaCompare searchCriteriaCompare = (SearchCriteriaCompare)searchCriteria;
					ConstantColumn col2 = searchCriteriaCompare.Rhs as ConstantColumn;
					if (col2 == null)
					{
						flag = false;
						break;
					}
					if (col == null)
					{
						col = searchCriteriaCompare.Lhs;
					}
					if (col != searchCriteriaCompare.Lhs)
					{
						flag = false;
						break;
					}
				}
				else
				{
					if (!(searchCriteria is SearchCriteriaAnd))
					{
						flag = false;
						break;
					}
					SearchCriteriaAnd searchCriteriaAnd = (SearchCriteriaAnd)searchCriteria;
					foreach (SearchCriteria searchCriteria2 in searchCriteriaAnd.NestedCriteria)
					{
						if (!(searchCriteria2 is SearchCriteriaCompare))
						{
							flag = false;
							break;
						}
						SearchCriteriaCompare searchCriteriaCompare2 = (SearchCriteriaCompare)searchCriteria2;
						ConstantColumn col3 = searchCriteriaCompare2.Rhs as ConstantColumn;
						if (col3 == null)
						{
							flag = false;
							break;
						}
						if (col == null)
						{
							col = searchCriteriaCompare2.Lhs;
						}
						if (col != searchCriteriaCompare2.Lhs)
						{
							flag = false;
							break;
						}
					}
					if (!flag)
					{
						break;
					}
				}
			}
			return flag;
		}

		private static bool IsSingleValueRange(SearchCriteriaCompare.SearchRelOp relop, object rhs)
		{
			return relop != SearchCriteriaCompare.SearchRelOp.NotEqual || rhs == null;
		}

		private static ValueRange BuildValueRange(SearchCriteriaCompare.SearchRelOp relop, object rhs, Column column, CompareInfo compareInfo)
		{
			switch (relop)
			{
			case SearchCriteriaCompare.SearchRelOp.Equal:
				return new ValueRange(new MinValue(true, rhs), new MaxValue(true, rhs), column, compareInfo);
			case SearchCriteriaCompare.SearchRelOp.NotEqual:
				return new ValueRange(new MinValue(false, rhs), MaxValue.Infinity, column, compareInfo);
			case SearchCriteriaCompare.SearchRelOp.LessThan:
				return new ValueRange(MinValue.Infinity, new MaxValue(false, rhs), column, compareInfo);
			case SearchCriteriaCompare.SearchRelOp.LessThanEqual:
				return new ValueRange(MinValue.Infinity, new MaxValue(true, rhs), column, compareInfo);
			case SearchCriteriaCompare.SearchRelOp.GreaterThan:
				return new ValueRange(new MinValue(false, rhs), MaxValue.Infinity, column, compareInfo);
			case SearchCriteriaCompare.SearchRelOp.GreaterThanEqual:
				return new ValueRange(new MinValue(true, rhs), MaxValue.Infinity, column, compareInfo);
			default:
				return ValueRange.Empty;
			}
		}

		private static List<ValueRange> BuildValueRanges(SearchCriteriaCompare.SearchRelOp relop, object rhs, Column column, CompareInfo compareInfo)
		{
			return new List<ValueRange>(2)
			{
				new ValueRange(MinValue.Infinity, new MaxValue(false, rhs), column, compareInfo),
				new ValueRange(new MinValue(false, rhs), MaxValue.Infinity, column, compareInfo)
			};
		}

		private static List<ValueRange> UnionRanges(List<ValueRange> ranges, List<ValueRange> newRanges, Column column, CompareInfo compareInfo)
		{
			if (ranges == null || newRanges == null)
			{
				return null;
			}
			if (ranges.Count == 0)
			{
				return newRanges;
			}
			if (newRanges.Count == 0)
			{
				return ranges;
			}
			for (int i = 0; i < newRanges.Count; i++)
			{
				ranges = ColumnRanges.UnionRanges(ranges, newRanges[i], column, compareInfo);
			}
			return ranges;
		}

		private static List<ValueRange> UnionRanges(List<ValueRange> ranges, ValueRange newRange, Column column, CompareInfo compareInfo)
		{
			if (ranges == null)
			{
				return null;
			}
			if (newRange.IsEmpty)
			{
				return ranges;
			}
			for (int i = 0; i < ranges.Count; i++)
			{
				if (ValueRange.AreOverlappingOrAdjacent(ranges[i], newRange, column, compareInfo))
				{
					newRange = ValueRange.UnionOverlappingOrAdjacent(ranges[i], newRange, column, compareInfo);
					if (ValueRange.Equal(newRange, ranges[i], compareInfo))
					{
						return ranges;
					}
					ranges.RemoveAt(i--);
				}
			}
			if (newRange.IsFull)
			{
				return null;
			}
			ranges.Add(newRange);
			return ranges;
		}

		private static List<ValueRange> IntersectRanges(List<ValueRange> ranges, List<ValueRange> newRanges, Column column, CompareInfo compareInfo)
		{
			if (ranges == null)
			{
				return newRanges;
			}
			if (newRanges == null)
			{
				return ranges;
			}
			if (ranges.Count == 0)
			{
				return ranges;
			}
			if (newRanges.Count == 0)
			{
				return newRanges;
			}
			int num = ranges.Count;
			for (int i = 0; i < num; i++)
			{
				bool flag = false;
				for (int j = 0; j < newRanges.Count; j++)
				{
					ValueRange valueRange = ValueRange.Intersect(newRanges[j], ranges[i], column, compareInfo);
					if (!valueRange.IsEmpty)
					{
						if (!ValueRange.Equal(valueRange, ranges[i], compareInfo))
						{
							ranges.Add(valueRange);
						}
						else
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					ranges.RemoveAt(i);
					num--;
					i--;
				}
			}
			return ranges;
		}

		private static List<ValueRange> IntersectRanges(List<ValueRange> ranges, ValueRange newRange, Column column, CompareInfo compareInfo)
		{
			if (newRange.IsEmpty)
			{
				if (ranges != null)
				{
					ranges.Clear();
					return ranges;
				}
				return new List<ValueRange>();
			}
			else if (ranges == null)
			{
				if (newRange.IsFull)
				{
					return ranges;
				}
				return new List<ValueRange>
				{
					newRange
				};
			}
			else
			{
				if (ranges.Count == 0)
				{
					return ranges;
				}
				for (int i = 0; i < ranges.Count; i++)
				{
					ValueRange valueRange = ValueRange.Intersect(ranges[i], newRange, column, compareInfo);
					if (!ValueRange.Equal(valueRange, ranges[i], compareInfo))
					{
						if (valueRange.IsEmpty)
						{
							ranges.RemoveAt(i--);
						}
						else
						{
							ranges[i] = valueRange;
						}
					}
				}
				return ranges;
			}
		}

		private static bool TestIntersectRanges(List<ValueRange> ranges, List<ValueRange> newRanges, Column column, CompareInfo compareInfo, out bool empty)
		{
			if (ranges == null)
			{
				empty = (newRanges != null && newRanges.Count == 0);
				return newRanges != null;
			}
			if (newRanges == null)
			{
				empty = (ranges.Count == 0);
				return false;
			}
			empty = true;
			if (ranges.Count == 0)
			{
				return false;
			}
			if (newRanges.Count == 0)
			{
				return true;
			}
			bool flag = false;
			for (int i = 0; i < ranges.Count; i++)
			{
				bool flag2 = false;
				for (int j = 0; j < newRanges.Count; j++)
				{
					ValueRange range = ValueRange.Intersect(newRanges[j], ranges[i], column, compareInfo);
					if (!range.IsEmpty)
					{
						empty = false;
						if (!ValueRange.Equal(range, ranges[i], compareInfo))
						{
							return true;
						}
						if (flag)
						{
							return true;
						}
						flag2 = true;
					}
				}
				if (!flag2)
				{
					if (!empty)
					{
						return true;
					}
					flag = true;
				}
			}
			return flag;
		}

		private static bool TestIntersectRanges(List<ValueRange> ranges, ValueRange newRange, Column column, CompareInfo compareInfo, out bool empty)
		{
			empty = true;
			if (newRange.IsEmpty)
			{
				return ranges == null || ranges.Count != 0;
			}
			if (ranges == null)
			{
				empty = false;
				return !newRange.IsFull;
			}
			if (ranges.Count == 0)
			{
				return false;
			}
			bool flag = false;
			for (int i = 0; i < ranges.Count; i++)
			{
				ValueRange range = ValueRange.Intersect(ranges[i], newRange, column, compareInfo);
				if (!ValueRange.Equal(range, ranges[i], compareInfo))
				{
					if (!range.IsEmpty)
					{
						empty = false;
					}
					if (!empty)
					{
						return true;
					}
					flag = true;
				}
				else
				{
					empty = false;
					if (flag)
					{
						return true;
					}
				}
			}
			return flag;
		}

		[Conditional("DEBUG")]
		private static void AssertNonOverlappingNonAdjacent(List<ValueRange> ranges, Column column, CompareInfo compareInfo)
		{
			if (ranges != null)
			{
				for (int i = 0; i < ranges.Count; i++)
				{
					for (int j = i + 1; j < ranges.Count; j++)
					{
					}
				}
			}
		}

		private Dictionary<Column, List<ValueRange>> columnSet;

		private class MinRangeComparer : IComparer<ValueRange>
		{
			public void Reconfigure(Column column, CompareInfo compareInfo)
			{
				this.column = column;
				this.compareInfo = compareInfo;
			}

			public int Compare(ValueRange range1, ValueRange range2)
			{
				return MinValue.Compare(range1.MinValue, range2.MinValue, this.column, this.compareInfo);
			}

			private Column column;

			private CompareInfo compareInfo;
		}
	}
}
