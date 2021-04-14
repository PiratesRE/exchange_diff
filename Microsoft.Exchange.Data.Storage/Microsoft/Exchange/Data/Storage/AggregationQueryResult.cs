using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AggregationQueryResult : DisposableObject, IQueryResult, IDisposable
	{
		private AggregationQueryResult(IQueryResult rawQueryResult, QueryFilter aggregationFilter, int columnsRequested)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.rawQueryResult = rawQueryResult;
				this.aggregationFilter = aggregationFilter;
				this.aggregationPropertyBag = new MailboxSyncPropertyBag(rawQueryResult.Columns.PropertyDefinitions);
				this.aggregationPropertyBag.AddColumnsFromFilter(aggregationFilter);
				this.columnsRequested = columnsRequested;
				this.unusedRows = new Queue<object[]>();
				this.currentRow = -1;
				this.moreRowsAvailableToFetch = true;
				disposeGuard.Success();
			}
		}

		internal static AggregationQueryResult FromQueryResult(IQueryResult rawQueryResult, QueryFilter aggregationFilter, int columnsRequested)
		{
			Util.ThrowOnNullArgument(rawQueryResult, "rawQueryResult");
			Util.ThrowOnNullArgument(aggregationFilter, "aggregationFilter");
			return new AggregationQueryResult(rawQueryResult, aggregationFilter, columnsRequested);
		}

		public int EstimatedRowCount
		{
			get
			{
				this.CheckDisposed(null);
				return this.rawQueryResult.EstimatedRowCount;
			}
		}

		public StoreSession StoreSession
		{
			get
			{
				this.CheckDisposed(null);
				return this.rawQueryResult.StoreSession;
			}
		}

		public ColumnPropertyDefinitions Columns
		{
			get
			{
				this.CheckDisposed(null);
				return this.rawQueryResult.Columns;
			}
		}

		public int CurrentRow
		{
			get
			{
				this.CheckDisposed(null);
				return this.currentRow;
			}
		}

		public new bool IsDisposed
		{
			get
			{
				return base.IsDisposed;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AggregationQueryResult>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.rawQueryResult.Dispose();
			}
			base.InternalDispose(disposing);
		}

		public bool SeekToCondition(SeekReference reference, QueryFilter seekFilter, SeekToConditionFlags flags)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException("SeekToCondition");
		}

		public bool SeekToCondition(uint bookMark, bool useForwardDirection, QueryFilter seekFilter, SeekToConditionFlags flags)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException("SeekToCondition");
		}

		public bool SeekToCondition(SeekReference reference, QueryFilter seekFilter)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException("SeekToCondition");
		}

		public int SeekToOffset(SeekReference reference, int offset)
		{
			this.CheckDisposed(null);
			if (reference == SeekReference.OriginBeginning && offset == 0)
			{
				return this.rawQueryResult.SeekToOffset(reference, offset);
			}
			throw new NotSupportedException("SeekToOffset is only supported at the beginning of the result set");
		}

		public void SetTableColumns(ICollection<PropertyDefinition> propertyDefinitions)
		{
			this.CheckDisposed(null);
			this.rawQueryResult.SetTableColumns(propertyDefinitions);
		}

		public object[][] GetRows(int rowCount, out bool mightBeMoreRows)
		{
			return this.GetRows(rowCount, QueryRowsFlags.None, out mightBeMoreRows);
		}

		public object[][] GetRows(int rowCount, QueryRowsFlags flags, out bool mightBeMoreRows)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<QueryRowsFlags>(flags, "flags");
			int num = rowCount * 3;
			if (!this.moreRowsAvailableToFetch || num <= this.unusedRows.Count)
			{
				mightBeMoreRows = this.moreRowsAvailableToFetch;
			}
			else
			{
				num -= this.unusedRows.Count;
				object[][] rows = this.rawQueryResult.GetRows(num, flags, out mightBeMoreRows);
				foreach (object[] item in rows)
				{
					this.unusedRows.Enqueue(item);
				}
				this.moreRowsAvailableToFetch = mightBeMoreRows;
			}
			object[][] array2 = this.FilterUnusedRows(rowCount);
			if (this.unusedRows.Count > 0)
			{
				mightBeMoreRows = true;
			}
			if (array2.Length > 0)
			{
				this.currentRow += array2.Length;
			}
			return array2;
		}

		public object[][] ExpandRow(int rowCount, long categoryId, out int rowsInExpandedCategory)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException("ExpandRow");
		}

		public int CollapseRow(long categoryId)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException("CollapseRow");
		}

		public uint CreateBookmark()
		{
			this.CheckDisposed(null);
			throw new NotSupportedException("CreateBookmark");
		}

		public void FreeBookmark(uint bookmarkPosition)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException("FreeBookmark");
		}

		public int SeekRowBookmark(uint bookmarkPosition, int rowCount, bool wantRowsSought, out bool soughtLess, out bool positionChanged)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException("SeekRowBookmark");
		}

		public NativeStorePropertyDefinition[] GetAllPropertyDefinitions(params PropertyTagPropertyDefinition[] excludeProperties)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException("GetAllPropertyDefinitions");
		}

		public byte[] GetCollapseState(byte[] instanceKey)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException("GetCollapseState");
		}

		public uint SetCollapseState(byte[] collapseState)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException("SetCollapseState");
		}

		public object Advise(SubscriptionSink subscriptionSink, bool asyncMode)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException("Advise");
		}

		public void Unadvise(object notificationHandle)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException("Unadvise");
		}

		public IStorePropertyBag[] GetPropertyBags(int rowCount)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException("GetPropertyBags");
		}

		private object[][] FilterUnusedRows(int rowCount)
		{
			this.CheckDisposed(null);
			List<object[]> list = new List<object[]>(rowCount);
			while (list.Count < rowCount && this.unusedRows.Count > 0)
			{
				object[] array = this.unusedRows.Dequeue();
				this.aggregationPropertyBag.Bind(array);
				if (EvaluatableFilter.Evaluate(this.aggregationFilter, this.aggregationPropertyBag))
				{
					if (array.Length > this.columnsRequested)
					{
						Array.Resize<object>(ref array, this.columnsRequested);
					}
					list.Add(array);
				}
			}
			return list.ToArray();
		}

		private const int FetchMultiplier = 3;

		private readonly IQueryResult rawQueryResult;

		private readonly QueryFilter aggregationFilter;

		private readonly MailboxSyncPropertyBag aggregationPropertyBag;

		private readonly int columnsRequested;

		private Queue<object[]> unusedRows;

		private int currentRow;

		private bool moreRowsAvailableToFetch;
	}
}
