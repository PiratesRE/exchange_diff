using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ViewCache : BaseObject
	{
		internal ViewCache(IViewDataSource dataSource)
		{
			this.dataSource = dataSource;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ViewCache>(this);
		}

		protected override void InternalDispose()
		{
			this.dataSource.Dispose();
			base.InternalDispose();
		}

		public int GetPosition()
		{
			return Math.Max(this.dataSource.GetPosition() - this.serverPosition + this.position, 0);
		}

		public int GetRowCount()
		{
			int num = this.dataSource.GetRowCount();
			if (num == 1 && this.GetPosition() == 0)
			{
				num = this.FetchNextBatch(1);
				if (num > 0)
				{
					this.SeekRow(BookmarkOrigin.Beginning, 0);
				}
			}
			return num;
		}

		public PropertyValue[][] FetchNoAdvance(uint rowCount)
		{
			if (rowCount > 0U)
			{
				PropertyValue[][] array2;
				if (this.IsPositionWithinCache(this.position))
				{
					int num = this.data.Length - this.position;
					PropertyValue[][] array = Array<PropertyValue[]>.Empty;
					if ((ulong)rowCount > (ulong)((long)num))
					{
						array = this.dataSource.GetRows((int)(rowCount - (uint)num), QueryRowsFlags.DoNotAdvance);
					}
					int num2 = Math.Min((int)rowCount, num + array.Length);
					array2 = new PropertyValue[num2][];
					Array.Copy(this.data, (long)this.position, array2, 0L, Math.Min((long)((ulong)rowCount), (long)num));
					if (array.Length > 0)
					{
						Array.Copy(array, 0, array2, num, array.Length);
					}
				}
				else
				{
					array2 = this.dataSource.GetRows((int)rowCount, QueryRowsFlags.DoNotAdvance);
				}
				return array2;
			}
			return Array<PropertyValue[]>.Empty;
		}

		public bool TryGetCurrentRow(int currentRowIndex, int fetchRowCount, out PropertyValue[] row)
		{
			int num = this.position + currentRowIndex;
			if (this.IsPositionWithinCache(num))
			{
				row = this.data[num];
				return true;
			}
			int num2 = this.FetchNextBatch(fetchRowCount);
			if (num2 > 0)
			{
				this.TryGetCurrentRow(currentRowIndex, 0, out row);
				return true;
			}
			row = null;
			return false;
		}

		public int MoveNext(int step)
		{
			this.position += step;
			return step;
		}

		public int SeekRow(BookmarkOrigin bookmarkOrigin, int rowCount)
		{
			if (bookmarkOrigin != BookmarkOrigin.Current || this.data == null)
			{
				this.ClearCache();
				return this.dataSource.SeekRow(bookmarkOrigin, rowCount);
			}
			int num = this.position;
			this.position += rowCount;
			if (this.IsPositionWithinCache(num) && this.IsPositionWithinCache(this.position))
			{
				return rowCount;
			}
			int num2 = this.serverPosition;
			return num2 + this.SyncServerCursor(rowCount).Value - num;
		}

		public int SeekRowBookmark(byte[] bookmark, int rowCount, bool wantMoveCount, out bool soughtLess, out bool positionChanged)
		{
			return this.dataSource.SeekRowBookmark(bookmark, rowCount, wantMoveCount, out soughtLess, out positionChanged);
		}

		public bool FindRow(Restriction restriction, uint bookmark, bool useForwardDirection, out PropertyValue[] row)
		{
			if (bookmark == 1U)
			{
				this.SyncServerCursor();
			}
			if (this.dataSource.FindRow(restriction, bookmark, useForwardDirection))
			{
				this.ClearCache();
				return this.TryGetCurrentRowNoAdvance(out row);
			}
			row = null;
			return false;
		}

		public bool FindRow(Restriction restriction, byte[] bookmark, bool useForwardDirection, out PropertyValue[] row)
		{
			this.ClearCache();
			if (this.dataSource.FindRow(restriction, bookmark, useForwardDirection))
			{
				return this.TryGetCurrentRowNoAdvance(out row);
			}
			row = null;
			return false;
		}

		public IViewDataSource DataSource
		{
			get
			{
				return this.dataSource;
			}
		}

		public void SetColumns(NativeStorePropertyDefinition[] propertyDefinitions, PropertyTag[] propertyTags)
		{
			this.SyncServerCursor();
			this.dataSource.SetColumns(propertyDefinitions, propertyTags);
		}

		public PropertyValue[][] ExpandRow(int maxRows, StoreId categoryId, out int rowsInExpandedCategory)
		{
			this.ClearCache();
			return this.DataSource.ExpandRow(maxRows, categoryId, out rowsInExpandedCategory);
		}

		public int CollapseRow(StoreId categoryId)
		{
			this.ClearCache();
			return this.DataSource.CollapseRow(categoryId);
		}

		internal PropertyTag[] QueryColumnsAll()
		{
			return this.DataSource.QueryColumnsAll();
		}

		public byte[] GetCollapseState(byte[] instanceKey)
		{
			return this.dataSource.GetCollapseState(instanceKey);
		}

		public byte[] SetCollapseState(byte[] collapseState)
		{
			this.ClearCache();
			return this.dataSource.SetCollapseState(collapseState);
		}

		public byte[] CreateBookmark()
		{
			this.SyncServerCursor();
			return this.dataSource.CreateBookmark();
		}

		public void FreeBookmark(byte[] bookmark)
		{
			this.dataSource.FreeBookmark(bookmark);
		}

		internal bool IsRowWithinUnreadCache(int instanceKeyColumnIndex, byte[] instanceKey, View.RowLookupPosition rowLookupPosition)
		{
			if (this.data == null || this.data.Length == 0 || instanceKey == null || instanceKey.Length == 0)
			{
				return false;
			}
			int num;
			int num2;
			if (rowLookupPosition == View.RowLookupPosition.Previous)
			{
				num = this.position - 1;
				num2 = this.data.Length - 1;
			}
			else
			{
				num = this.position;
				num2 = this.data.Length;
			}
			num = ((num > 0) ? num : 0);
			for (int i = num; i < num2; i++)
			{
				if (ArrayComparer<byte>.Comparer.Equals(this.data[i][instanceKeyColumnIndex].GetServerValue<byte[]>(), instanceKey))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsSameRow(PropertyValue[] row1, PropertyValue[] row2)
		{
			if (row1.Length != row2.Length)
			{
				return false;
			}
			for (int i = 0; i < row1.Length; i++)
			{
				if (row1[i].PropertyTag == PropertyTag.InstanceKey)
				{
					return row2[i].PropertyTag == row1[i].PropertyTag && ArrayComparer<byte>.Comparer.Equals(row1[i].GetValueAssert<byte[]>(), row2[i].GetValueAssert<byte[]>());
				}
			}
			return false;
		}

		private void ClearCache()
		{
			this.position = 0;
			this.serverPosition = 0;
			this.data = null;
		}

		private bool IsPositionWithinCache(int somePosition)
		{
			return this.data != null && somePosition >= 0 && somePosition < this.data.Length;
		}

		public int FetchNextBatch(int nRowsToFetch)
		{
			this.SyncServerCursor();
			QueryRowsFlags flags = (nRowsToFetch >= 0) ? QueryRowsFlags.None : QueryRowsFlags.DoNotAdvance;
			this.FaultInjectRowsToFetchIfApplicable(ref nRowsToFetch);
			PropertyValue[][] rows = this.dataSource.GetRows(nRowsToFetch, flags);
			this.data = rows;
			if (nRowsToFetch < 0)
			{
				this.serverPosition = 0;
				this.position = this.data.Length;
			}
			else
			{
				this.serverPosition = this.data.Length;
				this.position = 0;
			}
			return rows.Length;
		}

		private void FaultInjectRowsToFetchIfApplicable(ref int nRows)
		{
			int num = 0;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(3961924925U, ref num);
			if (num != 0 && num <= Math.Abs(nRows))
			{
				nRows = ((nRows > 0) ? num : (-num));
			}
		}

		private int? SyncServerCursor()
		{
			return this.SyncServerCursor(0);
		}

		private int? SyncServerCursor(int seekRow)
		{
			int? result;
			if (this.data != null)
			{
				if (this.serverPosition == this.position)
				{
					result = new int?(0);
				}
				else
				{
					result = new int?(this.dataSource.SeekRow(BookmarkOrigin.Current, this.position - this.serverPosition));
					this.InstrumentToCaptureOutlookBlankView(seekRow);
				}
				this.ClearCache();
			}
			else
			{
				result = null;
			}
			return result;
		}

		private void InstrumentToCaptureOutlookBlankView(int seekRow)
		{
			if (this.data.Length != 0 && seekRow == -1 && this.IsPositionWithinCache(this.position) && !this.IsPositionWithinCache(this.position + 1))
			{
				PropertyValue[][] rows = this.dataSource.GetRows(1, QueryRowsFlags.DoNotAdvance);
				if (rows.Length == 1)
				{
					PropertyValue[] row = rows[0];
					PropertyValue[] row2 = this.data[this.data.Length - 1];
					if (!ViewCache.IsSameRow(row, row2))
					{
						this.debugInformation = row;
						WatsonHelper.SendGenericWatsonReport("Table has been changed: cached row differs from server row.", string.Format("Server cursor position: {0}, ViewCache cursor position: {1}, Cached rows: {2}", this.serverPosition, this.position, this.data.Length));
						return;
					}
				}
				else
				{
					WatsonHelper.SendGenericWatsonReport("Table has been changed - last row removed", string.Format("Server cursor position: {0}, ViewCache cursor position: {1}, Cached rows: {2}", this.serverPosition, this.position, this.data.Length));
				}
			}
		}

		private bool TryGetCurrentRowNoAdvance(out PropertyValue[] row)
		{
			this.SyncServerCursor();
			PropertyValue[][] rows = this.dataSource.GetRows(1, QueryRowsFlags.DoNotAdvance);
			if (rows.Length == 1)
			{
				row = rows[0];
				return true;
			}
			row = null;
			return false;
		}

		private readonly IViewDataSource dataSource;

		private PropertyValue[][] data;

		private int position;

		private int serverPosition;

		private PropertyValue[] debugInformation;
	}
}
