using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxSyncQueryProcessor
	{
		public static MailboxSyncQueryProcessor.IQueryResult ItemQuery(Folder folder, ItemQueryType itemQueryType, QueryFilter filter, QueryFilter optimizationFilter, SortBy[] sortBy, PropertyDefinition[] queryColumns, bool allowTableRestrict, bool useSortOrder)
		{
			if (!(optimizationFilter is FalseFilter))
			{
				return new MailboxSyncQueryProcessor.InMemoryFilterQueryResult(folder, itemQueryType, filter, optimizationFilter, sortBy, queryColumns, useSortOrder);
			}
			if (MailboxSyncQueryProcessor.InMemoryFilterQueryResult.Match(filter, sortBy))
			{
				return new MailboxSyncQueryProcessor.InMemoryFilterQueryResult(folder, itemQueryType, filter, sortBy, queryColumns, useSortOrder);
			}
			if (allowTableRestrict)
			{
				return new MailboxSyncQueryProcessor.SlowQueryResult(folder, itemQueryType, filter, sortBy, queryColumns);
			}
			throw new InvalidOperationException(ServerStrings.ExNoOptimizedCodePath);
		}

		public delegate int ComparisonDelegate(object x, object y);

		internal interface IQueryResult : ITableView, IPagedView, IDisposeTrackable, IDisposable
		{
		}

		internal class InMemoryFilterQueryResult : MailboxSyncQueryProcessor.IQueryResult, ITableView, IPagedView, IDisposeTrackable, IDisposable
		{
			public InMemoryFilterQueryResult(Folder folder, ItemQueryType itemQueryType, QueryFilter filter, SortBy[] sortBy, PropertyDefinition[] queryColumns, bool useSortOrder)
			{
				ComparisonFilter comparisonFilter = filter as ComparisonFilter;
				if (comparisonFilter == null)
				{
					AndFilter andFilter = filter as AndFilter;
					if (andFilter != null)
					{
						comparisonFilter = (andFilter.Filters[0] as ComparisonFilter);
						if (comparisonFilter == null)
						{
							comparisonFilter = (ComparisonFilter)andFilter.Filters[1];
						}
					}
				}
				this.BuildResultSet(folder, itemQueryType, filter, comparisonFilter, sortBy, queryColumns, useSortOrder);
			}

			public InMemoryFilterQueryResult(Folder folder, ItemQueryType itemQueryType, QueryFilter filter, QueryFilter optimizationFilter, SortBy[] sortBy, PropertyDefinition[] queryColumns, bool useSortOrder)
			{
				this.BuildResultSet(folder, itemQueryType, filter, optimizationFilter, sortBy, queryColumns, useSortOrder);
			}

			public int CurrentRow
			{
				get
				{
					return this.idxCurrentRow;
				}
			}

			public int EstimatedRowCount
			{
				get
				{
					if (this.result != null)
					{
						return this.result.Count;
					}
					return 0;
				}
			}

			public static bool Match(QueryFilter queryFilter, SortBy[] sortBy)
			{
				if (!(queryFilter is ComparisonFilter))
				{
					if (queryFilter is OrFilter)
					{
						if (!MailboxSyncQueryProcessor.InMemoryFilterQueryResult.MatchOrFilter((OrFilter)queryFilter))
						{
							return false;
						}
					}
					else if (queryFilter is TextFilter)
					{
						if (!MailboxSyncQueryProcessor.InMemoryFilterQueryResult.MatchItemClassTextFilter((TextFilter)queryFilter))
						{
							return false;
						}
					}
					else
					{
						if (!(queryFilter is AndFilter))
						{
							return false;
						}
						AndFilter andFilter = (AndFilter)queryFilter;
						if (andFilter.FilterCount != 2)
						{
							return false;
						}
						int index;
						if (andFilter.Filters[0] is ComparisonFilter)
						{
							index = 1;
						}
						else
						{
							if (!(andFilter.Filters[1] is ComparisonFilter))
							{
								return false;
							}
							index = 0;
						}
						OrFilter orFilter = andFilter.Filters[index] as OrFilter;
						if (orFilter != null)
						{
							if (!MailboxSyncQueryProcessor.InMemoryFilterQueryResult.MatchOrFilter(orFilter))
							{
								return false;
							}
						}
						else
						{
							if (!(andFilter.Filters[index] is TextFilter))
							{
								return false;
							}
							if (!MailboxSyncQueryProcessor.InMemoryFilterQueryResult.MatchItemClassTextFilter((TextFilter)andFilter.Filters[index]))
							{
								return false;
							}
						}
					}
				}
				return sortBy != null && 1 == sortBy.Length && sortBy[0].ColumnDefinition.Type == typeof(int);
			}

			public void Dispose()
			{
			}

			public virtual DisposeTracker GetDisposeTracker()
			{
				return null;
			}

			public void SuppressDisposeTracker()
			{
			}

			public object[][] GetRows(int numRows)
			{
				if (this.result == null || this.idxCurrentRow >= this.result.Count)
				{
					return MailboxSyncQueryProcessor.InMemoryFilterQueryResult.EmptyResultSet;
				}
				int num = this.result.Count - this.idxCurrentRow;
				if (num > numRows)
				{
					num = numRows;
				}
				object[][] array = new object[num][];
				for (int i = 0; i < num; i++)
				{
					array[i] = this.result[this.idxCurrentRow + i];
				}
				this.idxCurrentRow += num;
				return array;
			}

			public bool SeekToCondition(SeekReference seekReference, QueryFilter seekCondition)
			{
				ComparisonFilter comparisonFilter = seekCondition as ComparisonFilter;
				if (comparisonFilter == null || seekReference != SeekReference.OriginBeginning)
				{
					throw new InvalidOperationException(ServerStrings.ExCannotSeekRow);
				}
				this.idxCurrentRow = 0;
				if (this.result == null || this.result.Count == 0)
				{
					return false;
				}
				MailboxSyncQueryProcessor.InMemoryFilterQueryResult.FoundResultDelegate foundResultDelegate;
				switch (comparisonFilter.ComparisonOperator)
				{
				case ComparisonOperator.Equal:
					foundResultDelegate = ((int result) => result == 0);
					break;
				case ComparisonOperator.NotEqual:
					foundResultDelegate = ((int result) => result != 0);
					break;
				case ComparisonOperator.LessThan:
					foundResultDelegate = ((int result) => result < 0);
					break;
				case ComparisonOperator.LessThanOrEqual:
					foundResultDelegate = ((int result) => result <= 0);
					break;
				case ComparisonOperator.GreaterThan:
					foundResultDelegate = ((int result) => result > 0);
					break;
				case ComparisonOperator.GreaterThanOrEqual:
					foundResultDelegate = ((int result) => result >= 0);
					break;
				default:
					throw new InvalidOperationException(ServerStrings.ExInvalidComparisonOperatorInComparisonFilter);
				}
				while (!foundResultDelegate(this.compareDelegate(this.result[this.idxCurrentRow][this.idxSortColumn], comparisonFilter.PropertyValue)))
				{
					this.idxCurrentRow++;
					if (this.idxCurrentRow >= this.result.Count)
					{
						return false;
					}
				}
				return true;
			}

			public int SeekToOffset(SeekReference seekReference, int offset)
			{
				if (seekReference != SeekReference.OriginBeginning)
				{
					throw new InvalidOperationException(ServerStrings.ExUnsupportedSeekReference);
				}
				if (this.result == null)
				{
					this.idxCurrentRow = 0;
				}
				else if (offset >= this.result.Count)
				{
					offset = this.result.Count;
				}
				else
				{
					this.idxCurrentRow = offset;
				}
				return this.CurrentRow;
			}

			private static QueryFilter CreateNegatedComparisonFilter(ComparisonFilter comparisonFilter)
			{
				switch (comparisonFilter.ComparisonOperator)
				{
				case ComparisonOperator.Equal:
					return new ComparisonFilter(ComparisonOperator.NotEqual, comparisonFilter.Property, comparisonFilter.PropertyValue);
				case ComparisonOperator.NotEqual:
					return new ComparisonFilter(ComparisonOperator.Equal, comparisonFilter.Property, comparisonFilter.PropertyValue);
				case ComparisonOperator.LessThan:
					return new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, comparisonFilter.Property, comparisonFilter.PropertyValue);
				case ComparisonOperator.LessThanOrEqual:
					return new ComparisonFilter(ComparisonOperator.GreaterThan, comparisonFilter.Property, comparisonFilter.PropertyValue);
				case ComparisonOperator.GreaterThan:
					return new ComparisonFilter(ComparisonOperator.LessThanOrEqual, comparisonFilter.Property, comparisonFilter.PropertyValue);
				case ComparisonOperator.GreaterThanOrEqual:
					return new ComparisonFilter(ComparisonOperator.LessThan, comparisonFilter.Property, comparisonFilter.PropertyValue);
				default:
					throw new InvalidOperationException("Invalid comparison operator");
				}
			}

			private static bool MatchItemClassTextFilter(TextFilter textFilter)
			{
				return textFilter.MatchFlags == MatchFlags.IgnoreCase && (textFilter.MatchOptions == MatchOptions.Prefix || textFilter.MatchOptions == MatchOptions.PrefixOnWords) && textFilter.Property == StoreObjectSchema.ItemClass;
			}

			private static bool MatchOrFilter(OrFilter comparisonOrFilter)
			{
				foreach (QueryFilter queryFilter in comparisonOrFilter.Filters)
				{
					TextFilter textFilter = queryFilter as TextFilter;
					if (textFilter == null)
					{
						return false;
					}
					if (!MailboxSyncQueryProcessor.InMemoryFilterQueryResult.MatchItemClassTextFilter(textFilter))
					{
						return false;
					}
				}
				return true;
			}

			private void BuildResultSet(Folder folder, ItemQueryType itemQueryType, QueryFilter filter, QueryFilter optimizationFilter, SortBy[] sortBy, PropertyDefinition[] queryColumns, bool useSortOrder)
			{
				MailboxSyncPropertyBag mailboxSyncPropertyBag = new MailboxSyncPropertyBag(queryColumns);
				ComparisonFilter comparisonFilter = optimizationFilter as ComparisonFilter;
				mailboxSyncPropertyBag.AddColumnsFromFilter(filter);
				this.idxSortColumn = mailboxSyncPropertyBag.AddColumn(sortBy[0].ColumnDefinition);
				SortBy[] sortColumns = null;
				if (comparisonFilter != null && useSortOrder)
				{
					SortOrder sortOrder;
					switch (comparisonFilter.ComparisonOperator)
					{
					case ComparisonOperator.LessThan:
					case ComparisonOperator.LessThanOrEqual:
						sortOrder = SortOrder.Descending;
						break;
					default:
						sortOrder = SortOrder.Ascending;
						break;
					}
					sortColumns = new SortBy[]
					{
						new SortBy(comparisonFilter.Property, sortOrder)
					};
				}
				QueryResult queryResult2;
				QueryResult queryResult = queryResult2 = folder.ItemQuery(itemQueryType, null, sortColumns, mailboxSyncPropertyBag.Columns);
				try
				{
					bool flag = 0 != queryResult.EstimatedRowCount;
					if (comparisonFilter != null)
					{
						flag = queryResult.SeekToCondition(SeekReference.OriginBeginning, comparisonFilter);
						int currentRow = queryResult.CurrentRow;
					}
					if (flag)
					{
						int currentRow = queryResult.CurrentRow;
						int num = -1;
						if (comparisonFilter != null)
						{
							QueryFilter seekFilter = MailboxSyncQueryProcessor.InMemoryFilterQueryResult.CreateNegatedComparisonFilter(comparisonFilter);
							if (queryResult.SeekToCondition(SeekReference.OriginCurrent, seekFilter))
							{
								num = queryResult.CurrentRow;
							}
							queryResult.SeekToOffset(SeekReference.OriginBeginning, currentRow);
						}
						this.result = new List<object[]>(queryResult.EstimatedRowCount);
						do
						{
							int num2 = 10000;
							if (-1 != num)
							{
								num2 = num - currentRow;
							}
							if (num2 > 10000)
							{
								num2 = 10000;
							}
							object[][] rows = queryResult.GetRows(num2);
							if (rows.Length == 0)
							{
								break;
							}
							for (int i = 0; i < rows.Length; i++)
							{
								mailboxSyncPropertyBag.Bind(rows[i]);
								if (EvaluatableFilter.Evaluate(filter, mailboxSyncPropertyBag))
								{
									this.result.Add(rows[i]);
								}
							}
						}
						while (-1 == num || queryResult.CurrentRow < num);
					}
				}
				finally
				{
					if (queryResult2 != null)
					{
						((IDisposable)queryResult2).Dispose();
					}
				}
				if (this.result != null)
				{
					this.result.Sort(this.GetSortByComparer(sortBy[0]));
				}
			}

			private IComparer<object[]> GetSortByComparer(SortBy sortBy)
			{
				if (sortBy.ColumnDefinition.Type == typeof(int))
				{
					this.compareDelegate = delegate(object x, object y)
					{
						if (x == null)
						{
							return -1;
						}
						if (y == null)
						{
							return 1;
						}
						int num = (int)x;
						int num2 = (int)y;
						if (num < num2)
						{
							return -1;
						}
						if (num > num2)
						{
							return 1;
						}
						return 0;
					};
					return new MailboxSyncQueryProcessor.CompareBySortColumn<int>(sortBy.SortOrder, this.idxSortColumn, this.compareDelegate);
				}
				throw new InvalidOperationException(ServerStrings.ExMatchShouldHaveBeenCalled);
			}

			private static readonly object[][] EmptyResultSet = Array<object[]>.Empty;

			private MailboxSyncQueryProcessor.ComparisonDelegate compareDelegate;

			private int idxCurrentRow;

			private int idxSortColumn;

			private List<object[]> result;

			private delegate bool FoundResultDelegate(int result);
		}

		internal class SlowQueryResult : MailboxSyncQueryProcessor.IQueryResult, ITableView, IPagedView, IDisposeTrackable, IDisposable
		{
			public SlowQueryResult(Folder folder, ItemQueryType itemQueryType, QueryFilter filter, SortBy[] sortBy, PropertyDefinition[] queryColumns)
			{
				this.queryResult = folder.ItemQuery(itemQueryType, filter, sortBy, queryColumns);
				this.disposeTracker = this.GetDisposeTracker();
			}

			public int CurrentRow
			{
				get
				{
					return this.queryResult.CurrentRow;
				}
			}

			public int EstimatedRowCount
			{
				get
				{
					return this.queryResult.EstimatedRowCount;
				}
			}

			public virtual DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<MailboxSyncQueryProcessor.SlowQueryResult>(this);
			}

			public void SuppressDisposeTracker()
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Suppress();
				}
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			public object[][] GetRows(int numRows)
			{
				return this.queryResult.GetRows(numRows);
			}

			public bool SeekToCondition(SeekReference seekReference, QueryFilter seekCondition)
			{
				return this.queryResult.SeekToCondition(seekReference, seekCondition);
			}

			public int SeekToOffset(SeekReference seekReference, int offset)
			{
				return this.queryResult.SeekToOffset(seekReference, offset);
			}

			private void Dispose(bool disposing)
			{
				if (!this.disposed)
				{
					this.disposed = true;
					this.InternalDispose(disposing);
				}
			}

			private void InternalDispose(bool disposing)
			{
				if (disposing)
				{
					if (this.queryResult != null)
					{
						this.queryResult.Dispose();
					}
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
					}
				}
			}

			private readonly DisposeTracker disposeTracker;

			private bool disposed;

			private QueryResult queryResult;
		}

		private class CompareBySortColumn<elementType> : IComparer<object[]>
		{
			public CompareBySortColumn(SortOrder sortOrder, int idxSortColumn, MailboxSyncQueryProcessor.ComparisonDelegate compareDelegate)
			{
				this.sortOrder = sortOrder;
				this.idxSortColumn = idxSortColumn;
				this.compareDelegate = compareDelegate;
			}

			int IComparer<object[]>.Compare(object[] x, object[] y)
			{
				int num = this.compareDelegate(x[this.idxSortColumn], y[this.idxSortColumn]);
				if (this.sortOrder != SortOrder.Ascending)
				{
					return -num;
				}
				return num;
			}

			private MailboxSyncQueryProcessor.ComparisonDelegate compareDelegate;

			private int idxSortColumn;

			private SortOrder sortOrder;
		}
	}
}
