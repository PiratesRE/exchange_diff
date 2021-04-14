using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Rpc.QueueViewer;

namespace Microsoft.Exchange.Transport.QueueViewer
{
	internal class PagingEngine<ObjectType, SchemaType> where ObjectType : PagedDataObject where SchemaType : PagedObjectSchema, new()
	{
		public PagingEngine()
		{
			this.maxQueryResultCount = Components.TransportAppConfig.RemoteDelivery.MaxQueryResultCount;
		}

		public bool SearchForward
		{
			get
			{
				return this.searchForward;
			}
			set
			{
				this.searchForward = value;
				ExTraceGlobals.QueuingTracer.TraceDebug<bool>((long)this.GetHashCode(), "Search forward: {0}", this.searchForward);
			}
		}

		public int PageSize
		{
			get
			{
				return this.pageSize;
			}
			set
			{
				this.pageSize = value;
				ExTraceGlobals.QueuingTracer.TraceDebug<int>((long)this.GetHashCode(), "Page size: {0}", this.pageSize);
			}
		}

		public ObjectType BookmarkObject
		{
			get
			{
				return this.bookmarkObject;
			}
			set
			{
				this.bookmarkObject = value;
				ExTraceGlobals.QueuingTracer.TraceDebug((long)this.GetHashCode(), "Bookmark object: {0}", new object[]
				{
					(this.bookmarkObject != null) ? this.bookmarkObject : "(null)"
				});
			}
		}

		public int BookmarkIndex
		{
			get
			{
				return this.bookmarkIndex;
			}
			set
			{
				this.bookmarkIndex = value;
				ExTraceGlobals.QueuingTracer.TraceDebug<int>((long)this.GetHashCode(), "Bookmark index: {0}", this.bookmarkIndex);
			}
		}

		public bool IncludeBookmark
		{
			get
			{
				return this.includeBookmark;
			}
			set
			{
				this.includeBookmark = value;
				ExTraceGlobals.QueuingTracer.TraceDebug<bool>((long)this.GetHashCode(), "Include bookmark: {0}", this.includeBookmark);
			}
		}

		public bool IncludeDetails
		{
			get
			{
				return this.includeDetails;
			}
			set
			{
				this.includeDetails = value;
				ExTraceGlobals.QueuingTracer.TraceDebug<bool>((long)this.GetHashCode(), "Include details: {0}", this.includeDetails);
			}
		}

		public bool IncludeComponentLatencyInfo
		{
			get
			{
				return this.includeComponentLatencyInfo;
			}
			set
			{
				this.includeComponentLatencyInfo = value;
				ExTraceGlobals.QueuingTracer.TraceDebug<bool>((long)this.GetHashCode(), "Include ComponentLatencyInfo: {0}", this.includeComponentLatencyInfo);
			}
		}

		public bool FilterUsesOnlyBasicFields
		{
			get
			{
				return this.filterUsesOnlyBasicFields;
			}
		}

		public bool IdentitySearch
		{
			get
			{
				return this.identitySearch;
			}
		}

		public object IdentitySearchValue
		{
			get
			{
				return this.identitySearchValue;
			}
		}

		public void ResetResultSet()
		{
			this.resultSet.Clear();
		}

		public bool AddToResultSet(ObjectType dataObject)
		{
			if (this.resultSet.Count == this.maxQueryResultCount || this.resultSet.Count == this.PageSize)
			{
				return false;
			}
			this.resultSet.Add(dataObject);
			return true;
		}

		public void SetFilter(QueryFilter queryFilter)
		{
			this.filterUsesOnlyBasicFields = true;
			if (queryFilter == null)
			{
				this.filter = null;
				this.csharpFilters = null;
				return;
			}
			this.filter = new List<PagingEngine<ObjectType, SchemaType>.FilterOperation<ObjectType, SchemaType>>();
			this.csharpFilters = new List<PagingEngine<ObjectType, SchemaType>.CSharpFilterOperation<ObjectType>>();
			this.RecurseExpressionTree(queryFilter);
			ExTraceGlobals.QueuingTracer.TraceDebug((long)this.GetHashCode(), "Query filter parsed to a node structure");
			if (this.identitySearch)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug((long)this.GetHashCode(), "Search by Identity {0}", new object[]
				{
					this.identitySearchValue
				});
			}
			foreach (PagingEngine<ObjectType, SchemaType>.FilterOperation<ObjectType, SchemaType> arg in this.filter)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<PagingEngine<ObjectType, SchemaType>.FilterOperation<ObjectType, SchemaType>>((long)this.GetHashCode(), "{0}", arg);
			}
		}

		public void SetSortOrder(QueueViewerSortOrderEntry[] originalSortOrder)
		{
			this.normalizedSortOrder = new List<PagingEngine<ObjectType, SchemaType>.InternalSortOrderEntry<ObjectType, SchemaType>>();
			bool flag = true;
			if (originalSortOrder != null)
			{
				foreach (QueueViewerSortOrderEntry queueViewerSortOrderEntry in originalSortOrder)
				{
					int fieldIndex = PagingEngine<ObjectType, SchemaType>.schema.GetFieldIndex(queueViewerSortOrderEntry.Property);
					bool flag2 = true;
					foreach (PagingEngine<ObjectType, SchemaType>.InternalSortOrderEntry<ObjectType, SchemaType> internalSortOrderEntry in this.normalizedSortOrder)
					{
						if (internalSortOrderEntry.Field == fieldIndex)
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						this.normalizedSortOrder.Add(new PagingEngine<ObjectType, SchemaType>.InternalSortOrderEntry<ObjectType, SchemaType>(fieldIndex, queueViewerSortOrderEntry.SortDirection));
						if (queueViewerSortOrderEntry.Property == "Identity")
						{
							flag = false;
							break;
						}
					}
				}
			}
			if (flag)
			{
				this.normalizedSortOrder.Add(new PagingEngine<ObjectType, SchemaType>.InternalSortOrderEntry<ObjectType, SchemaType>(PagingEngine<ObjectType, SchemaType>.schema.GetFieldIndex("Identity"), ListSortDirection.Ascending));
			}
			ExTraceGlobals.QueuingTracer.TraceDebug((long)this.GetHashCode(), "Sort order normalized");
			foreach (PagingEngine<ObjectType, SchemaType>.InternalSortOrderEntry<ObjectType, SchemaType> arg in this.normalizedSortOrder)
			{
				ExTraceGlobals.QueuingTracer.TraceDebug<PagingEngine<ObjectType, SchemaType>.InternalSortOrderEntry<ObjectType, SchemaType>>((long)this.GetHashCode(), "{0}", arg);
			}
		}

		public bool ApplyFilterConditions(ObjectType dataObject, out bool failedOnBasicField)
		{
			failedOnBasicField = false;
			if (this.filter == null)
			{
				return true;
			}
			foreach (PagingEngine<ObjectType, SchemaType>.FilterOperation<ObjectType, SchemaType> filterOperation in this.filter)
			{
				if (!filterOperation.Evaluate(dataObject))
				{
					if (PagingEngine<ObjectType, SchemaType>.schema.IsBasicField(filterOperation.Field))
					{
						failedOnBasicField = true;
					}
					return false;
				}
			}
			foreach (PagingEngine<ObjectType, SchemaType>.CSharpFilterOperation<ObjectType> csharpFilterOperation in this.csharpFilters)
			{
				if (!csharpFilterOperation.Evaluate(dataObject))
				{
					return false;
				}
			}
			return true;
		}

		public bool ApplyFilterConditions(ObjectType dataObject)
		{
			bool flag;
			return this.ApplyFilterConditions(dataObject, out flag);
		}

		public bool ApplyBookmarkConditions(PagedDataObject dataObject, out bool onlyBasicFieldsUsed)
		{
			onlyBasicFieldsUsed = true;
			if (this.bookmarkObject == null)
			{
				return true;
			}
			foreach (PagingEngine<ObjectType, SchemaType>.InternalSortOrderEntry<ObjectType, SchemaType> internalSortOrderEntry in this.normalizedSortOrder)
			{
				ComparisonOperator comparisonOperator = this.searchForward ? ((internalSortOrderEntry.SortDirection == ListSortDirection.Ascending) ? ComparisonOperator.GreaterThan : ComparisonOperator.LessThan) : ((internalSortOrderEntry.SortDirection == ListSortDirection.Ascending) ? ComparisonOperator.LessThan : ComparisonOperator.GreaterThan);
				int num = PagingEngine<ObjectType, SchemaType>.schema.CompareField(internalSortOrderEntry.Field, dataObject, this.bookmarkObject);
				ComparisonOperator comparisonOperator2 = (num < 0) ? ComparisonOperator.LessThan : ((num == 0) ? ComparisonOperator.Equal : ComparisonOperator.GreaterThan);
				if (!PagingEngine<ObjectType, SchemaType>.schema.IsBasicField(internalSortOrderEntry.Field))
				{
					onlyBasicFieldsUsed = false;
				}
				if (comparisonOperator2 == comparisonOperator)
				{
					return true;
				}
				if (comparisonOperator2 != ComparisonOperator.Equal)
				{
					return false;
				}
			}
			return this.includeBookmark;
		}

		public bool ApplyBookmarkConditions(PagedDataObject dataObject)
		{
			bool flag;
			return this.ApplyBookmarkConditions(dataObject, out flag);
		}

		public ObjectType[] GetPage(int totalCount, out int pageOffset, out bool redoQuery)
		{
			pageOffset = 0;
			redoQuery = false;
			if (totalCount < this.resultSet.Count)
			{
				throw new InvalidOperationException();
			}
			this.OrderResultSet();
			int num = -1;
			int num2;
			if (this.bookmarkObject != null || this.bookmarkIndex < 0)
			{
				num2 = Math.Min(this.pageSize, this.resultSet.Count);
				if (this.searchForward)
				{
					if (num2 == 0 && totalCount > 0)
					{
						this.searchForward = false;
						this.bookmarkObject = default(ObjectType);
						redoQuery = true;
					}
					else
					{
						num = 0;
						pageOffset = totalCount - this.resultSet.Count;
					}
				}
				else if (num2 < this.pageSize && totalCount > num2)
				{
					this.searchForward = true;
					this.bookmarkObject = default(ObjectType);
					redoQuery = true;
				}
				else
				{
					num = this.resultSet.Count - num2;
					pageOffset = num;
				}
			}
			else
			{
				if (this.searchForward)
				{
					num = (this.includeBookmark ? this.bookmarkIndex : (this.bookmarkIndex + 1));
					if (num >= this.resultSet.Count)
					{
						num2 = Math.Min(this.pageSize, this.resultSet.Count);
						num = this.resultSet.Count - num2;
					}
					else
					{
						num2 = Math.Min(this.pageSize, this.resultSet.Count - num);
					}
				}
				else
				{
					int num3 = this.includeBookmark ? this.bookmarkIndex : (this.bookmarkIndex - 1);
					num2 = Math.Min(this.pageSize, this.resultSet.Count);
					if (num3 >= this.resultSet.Count)
					{
						num = this.resultSet.Count - num2;
					}
					else if (num3 < 0 || num3 + 1 < num2)
					{
						num = 0;
					}
					else
					{
						num = num3 + 1 - num2;
					}
				}
				pageOffset = num;
			}
			if (!redoQuery)
			{
				ObjectType[] array = new ObjectType[num2];
				if (num2 > 0)
				{
					this.resultSet.CopyTo(num, array, 0, num2);
				}
				return array;
			}
			return null;
		}

		private void OrderResultSet()
		{
			if (this.resultSet.Count == 0 || this.normalizedSortOrder == null)
			{
				return;
			}
			this.comparer = new PagingEngine<ObjectType, SchemaType>.PagedDataObjectComparer<ObjectType, SchemaType>(this);
			for (int i = this.resultSet.Count / 2 - 1; i >= 0; i--)
			{
				this.SiftDown(i, this.resultSet.Count - 1);
			}
			for (int j = this.resultSet.Count - 1; j >= 1; j--)
			{
				ObjectType value = this.resultSet[0];
				this.resultSet[0] = this.resultSet[j];
				this.resultSet[j] = value;
				this.SiftDown(0, j - 1);
			}
		}

		private void SiftDown(int root, int bottom)
		{
			for (int i = 2 * root + 1; i <= bottom; i = 2 * root + 1)
			{
				if (i + 1 <= bottom && this.comparer.Compare(this.resultSet[i + 1], this.resultSet[i]) > 0)
				{
					i++;
				}
				if (this.comparer.Compare(this.resultSet[root], this.resultSet[i]) >= 0)
				{
					return;
				}
				ObjectType value = this.resultSet[root];
				this.resultSet[root] = this.resultSet[i];
				this.resultSet[i] = value;
				root = i;
			}
		}

		private void RecurseExpressionTree(QueryFilter filterNode)
		{
			AndFilter andFilter = filterNode as AndFilter;
			if (andFilter != null)
			{
				foreach (QueryFilter filterNode2 in andFilter.Filters)
				{
					this.RecurseExpressionTree(filterNode2);
				}
				return;
			}
			NotFilter notFilter = filterNode as NotFilter;
			if (notFilter != null)
			{
				ExistsFilter existsFilter = notFilter.Filter as ExistsFilter;
				if (existsFilter != null)
				{
					this.PreprocessExistsFilter(existsFilter, false);
					return;
				}
				throw new QueueViewerException(QVErrorCode.QV_E_FILTER_TYPE_NOT_SUPPORTED);
			}
			else
			{
				ExistsFilter existsFilter2 = filterNode as ExistsFilter;
				if (existsFilter2 != null)
				{
					this.PreprocessExistsFilter(existsFilter2, true);
					return;
				}
				TextFilter textFilter = filterNode as TextFilter;
				if (textFilter != null)
				{
					PagingEngine<ObjectType, SchemaType>.FilterOperation<ObjectType, SchemaType> filterOperation = new PagingEngine<ObjectType, SchemaType>.TextFilterOperation<ObjectType, SchemaType>(textFilter);
					if (!PagingEngine<ObjectType, SchemaType>.schema.IsBasicField(filterOperation.Field))
					{
						this.filterUsesOnlyBasicFields = false;
					}
					this.filter.Add(filterOperation);
					return;
				}
				ComparisonFilter comparisonFilter = filterNode as ComparisonFilter;
				if (comparisonFilter != null)
				{
					PagingEngine<ObjectType, SchemaType>.FilterOperation<ObjectType, SchemaType> filterOperation2 = new PagingEngine<ObjectType, SchemaType>.ComparisonFilterOperation<ObjectType, SchemaType>(comparisonFilter);
					if (!PagingEngine<ObjectType, SchemaType>.schema.IsBasicField(filterOperation2.Field))
					{
						this.filterUsesOnlyBasicFields = false;
					}
					if (comparisonFilter.Property.Name == "Identity" && comparisonFilter.ComparisonOperator == ComparisonOperator.Equal)
					{
						this.identitySearch = true;
						this.identitySearchValue = comparisonFilter.PropertyValue;
						return;
					}
					this.filter.Add(filterOperation2);
					return;
				}
				else
				{
					CSharpFilter<ObjectType> csharpFilter = filterNode as CSharpFilter<ObjectType>;
					if (csharpFilter != null)
					{
						PagingEngine<ObjectType, SchemaType>.CSharpFilterOperation<ObjectType> item = new PagingEngine<ObjectType, SchemaType>.CSharpFilterOperation<ObjectType>(csharpFilter);
						this.csharpFilters.Add(item);
						this.filterUsesOnlyBasicFields = false;
						return;
					}
					throw new QueueViewerException(QVErrorCode.QV_E_FILTER_TYPE_NOT_SUPPORTED);
				}
			}
		}

		private void PreprocessExistsFilter(ExistsFilter filterNode, bool affirmative)
		{
			PagingEngine<ObjectType, SchemaType>.FilterOperation<ObjectType, SchemaType> filterOperation = new PagingEngine<ObjectType, SchemaType>.ComparisonFilterOperation<ObjectType, SchemaType>(new ComparisonFilter(affirmative ? ComparisonOperator.NotEqual : ComparisonOperator.Equal, filterNode.Property, null));
			if (!PagingEngine<ObjectType, SchemaType>.schema.IsBasicField(filterOperation.Field))
			{
				this.filterUsesOnlyBasicFields = false;
			}
			this.filter.Add(filterOperation);
		}

		private const int MaxQueryResultCountDefault = 250000;

		private static PagedObjectSchema schema = ObjectSchema.GetInstance<SchemaType>();

		private bool filterUsesOnlyBasicFields;

		private List<PagingEngine<ObjectType, SchemaType>.FilterOperation<ObjectType, SchemaType>> filter;

		private List<PagingEngine<ObjectType, SchemaType>.CSharpFilterOperation<ObjectType>> csharpFilters;

		private List<PagingEngine<ObjectType, SchemaType>.InternalSortOrderEntry<ObjectType, SchemaType>> normalizedSortOrder;

		private PagingEngine<ObjectType, SchemaType>.PagedDataObjectComparer<ObjectType, SchemaType> comparer;

		private bool searchForward = true;

		private int pageSize = 1000;

		private ObjectType bookmarkObject = default(ObjectType);

		private int bookmarkIndex = -1;

		private bool includeBookmark;

		private bool includeDetails;

		private bool includeComponentLatencyInfo;

		private bool identitySearch;

		private object identitySearchValue;

		private List<ObjectType> resultSet = new List<ObjectType>();

		private int maxQueryResultCount = 250000;

		private abstract class FilterOperation<T, S> where T : PagedDataObject where S : PagedObjectSchema, new()
		{
			public FilterOperation(SinglePropertyFilter filter)
			{
				this.Field = PagingEngine<T, S>.schema.GetFieldIndex(filter.Property.Name);
				this.filter = filter;
			}

			public override string ToString()
			{
				return this.filter.ToString();
			}

			public abstract bool Evaluate(T dataObject);

			public readonly int Field;

			protected SinglePropertyFilter filter;
		}

		private class CSharpFilterOperation<T> where T : PagedDataObject
		{
			public CSharpFilterOperation(CSharpFilter<T> filter)
			{
				if (filter == null)
				{
					throw new ArgumentNullException("filter");
				}
				this.csharpFilter = filter;
			}

			public bool Evaluate(T dataObject)
			{
				return this.csharpFilter.Match(dataObject);
			}

			public override string ToString()
			{
				return this.csharpFilter.ToString();
			}

			private CSharpFilter<T> csharpFilter;
		}

		private class ComparisonFilterOperation<T, S> : PagingEngine<ObjectType, SchemaType>.FilterOperation<T, S> where T : PagedDataObject where S : PagedObjectSchema, new()
		{
			public ComparisonFilterOperation(ComparisonFilter filter) : base(filter)
			{
			}

			public override bool Evaluate(T dataObject)
			{
				ComparisonFilter comparisonFilter = (ComparisonFilter)this.filter;
				int num = PagingEngine<T, S>.schema.CompareField(this.Field, dataObject, comparisonFilter.PropertyValue);
				ComparisonOperator comparisonOperator = (num < 0) ? ComparisonOperator.LessThan : ((num == 0) ? ComparisonOperator.Equal : ComparisonOperator.GreaterThan);
				return comparisonOperator == comparisonFilter.ComparisonOperator || (comparisonFilter.ComparisonOperator == ComparisonOperator.LessThanOrEqual && (comparisonOperator == ComparisonOperator.LessThan || comparisonOperator == ComparisonOperator.Equal)) || (comparisonFilter.ComparisonOperator == ComparisonOperator.GreaterThanOrEqual && (comparisonOperator == ComparisonOperator.GreaterThan || comparisonOperator == ComparisonOperator.Equal)) || (comparisonFilter.ComparisonOperator == ComparisonOperator.NotEqual && comparisonOperator != ComparisonOperator.Equal);
			}
		}

		private class TextFilterOperation<T, S> : PagingEngine<ObjectType, SchemaType>.FilterOperation<T, S> where T : PagedDataObject where S : PagedObjectSchema, new()
		{
			public TextFilterOperation(TextFilter filter) : base(filter)
			{
				Type fieldType = PagingEngine<T, S>.schema.GetFieldType(this.Field);
				this.matchPattern = filter.Text;
				this.matchOptions = filter.MatchOptions;
				MethodInfo method = fieldType.GetMethod("ParsePattern");
				if (method != null)
				{
					string text = filter.Text;
					if (filter.MatchOptions == MatchOptions.Prefix)
					{
						text += "*";
					}
					else if (filter.MatchOptions == MatchOptions.Suffix)
					{
						text = "*" + text;
					}
					else if (filter.MatchOptions == MatchOptions.SubString)
					{
						text = "*" + text + "*";
					}
					object[] array = new object[]
					{
						text,
						this.matchOptions
					};
					try
					{
						this.matchPattern = method.Invoke(null, array);
					}
					catch (TargetInvocationException ex)
					{
						if (ex.InnerException is ArgumentException || ex.InnerException is ArgumentNullException)
						{
							throw new QueueViewerException(QVErrorCode.QV_E_INVALID_IDENTITY_STRING);
						}
						throw;
					}
					this.matchOptions = (MatchOptions)array[1];
				}
			}

			public override bool Evaluate(T dataObject)
			{
				return PagingEngine<T, S>.schema.MatchField(this.Field, dataObject, this.matchPattern, this.matchOptions);
			}

			private object matchPattern;

			private MatchOptions matchOptions;
		}

		private class InternalSortOrderEntry<T, S> where T : PagedDataObject where S : PagedObjectSchema, new()
		{
			public InternalSortOrderEntry(int field, ListSortDirection sortDirection)
			{
				this.Field = field;
				this.SortDirection = sortDirection;
			}

			public override string ToString()
			{
				return string.Format("({0} {1})", PagingEngine<T, S>.schema.GetFieldName(this.Field), this.SortDirection);
			}

			public readonly int Field;

			public readonly ListSortDirection SortDirection;
		}

		private class PagedDataObjectComparer<T, S> : IComparer<T> where T : PagedDataObject where S : PagedObjectSchema, new()
		{
			public PagedDataObjectComparer(PagingEngine<T, S> pagingEngine)
			{
				this.engine = pagingEngine;
			}

			public int Compare(T x, T y)
			{
				int num = 0;
				int i = 0;
				while (i < this.engine.normalizedSortOrder.Count)
				{
					PagingEngine<T, S>.InternalSortOrderEntry<T, S> internalSortOrderEntry = this.engine.normalizedSortOrder[i];
					num = PagingEngine<T, S>.schema.CompareField(internalSortOrderEntry.Field, x, y);
					if (num != 0)
					{
						if (internalSortOrderEntry.SortDirection == ListSortDirection.Descending)
						{
							num = -num;
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
				return num;
			}

			private PagingEngine<T, S> engine;
		}
	}
}
