using System;
using System.Collections.Generic;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.Storage
{
	[Serializable]
	internal class DataColumnsCache : DataInternalComponent
	{
		public DataColumnsCache(DataRow dataRow) : base(dataRow)
		{
			DataTableView tableView = base.DataRow.TableView;
			DataTable table = tableView.Table;
			this.cache = new ColumnCache[tableView.ColumnCount];
			for (int i = 0; i < this.cache.Length; i++)
			{
				int columnRowIndex = tableView.GetColumnRowIndex(i);
				DataColumn dataColumn = table.Schemas[columnRowIndex];
				this.cache[i] = dataColumn.NewCacheCell();
				if (dataColumn.Required)
				{
					this.cache[i].HasValue = true;
				}
			}
		}

		public override bool PendingDatabaseUpdates
		{
			get
			{
				foreach (ColumnCache columnCache in this.cache)
				{
					if (columnCache.Dirty)
					{
						return true;
					}
				}
				return false;
			}
		}

		public int Count
		{
			get
			{
				return this.cache.Length;
			}
		}

		public ColumnCache this[int index]
		{
			get
			{
				base.DataRow.Table.ValidateCachedColumn(index);
				int columnViewIndex = base.DataRow.TableView.GetColumnViewIndex(index);
				return this.cache[columnViewIndex];
			}
		}

		public override void LoadFromParentRow(DataTableCursor cursor)
		{
			if (cursor == null)
			{
				throw new ArgumentNullException("cursor");
			}
			DataTableView tableView = base.DataRow.TableView;
			DataTable table = tableView.Table;
			ColumnValue[] array = new ColumnValue[this.cache.Length];
			for (int i = 0; i < this.cache.Length; i++)
			{
				int columnRowIndex = tableView.GetColumnRowIndex(i);
				DataColumn dataColumn = table.Schemas[columnRowIndex];
				ColumnValue columnValueForRetrieval = dataColumn.GetColumnValueForRetrieval();
				array[i] = columnValueForRetrieval;
			}
			Api.RetrieveColumns(cursor.Session, cursor.TableId, array);
			for (int j = 0; j < this.cache.Length; j++)
			{
				ColumnCache columnCache = this.cache[j];
				ColumnValue columnValue = array[j];
				int columnRowIndex2 = tableView.GetColumnRowIndex(j);
				DataColumn dataColumn2 = table.Schemas[columnRowIndex2];
				object valueAsObject = columnValue.ValueAsObject;
				if (valueAsObject != null)
				{
					dataColumn2.ColumnValueToCache(columnValue, columnCache);
				}
				else
				{
					columnCache.HasValue = false;
				}
				columnCache.Dirty = false;
				base.DataRow.PerfCounters.ColumnsCacheColumnLoads.Increment();
			}
			base.DataRow.PerfCounters.ColumnsCacheLoads.Increment();
		}

		public override void SaveToParentRow(DataTableCursor cursor, Func<bool> checkpointCallback)
		{
			if (cursor == null)
			{
				throw new ArgumentNullException("cursor");
			}
			bool flag = false;
			DataTableView tableView = base.DataRow.TableView;
			DataTable table = tableView.Table;
			for (int i = 0; i < this.cache.Length; i++)
			{
				int columnRowIndex = tableView.GetColumnRowIndex(i);
				DataColumn dataColumn = table.Schemas[columnRowIndex];
				ColumnCache columnCache = this.cache[i];
				if (dataColumn.IsAutoGenerated)
				{
					flag = true;
				}
				else if (columnCache.Dirty)
				{
					byte[] array = columnCache.HasValue ? dataColumn.BytesFromCache(columnCache) : null;
					dataColumn.SaveToCursor(cursor, array, 1, false, -1);
					columnCache.Dirty = false;
					if (array != null)
					{
						base.DataRow.PerfCounters.ColumnsCacheByteSaves.IncrementBy((long)array.Length);
					}
					base.DataRow.PerfCounters.ColumnsCacheColumnSaves.Increment();
				}
			}
			if (flag)
			{
				this.LoadAutoGeneratedColumns(cursor);
			}
			base.DataRow.PerfCounters.ColumnsCacheSaves.Increment();
		}

		public override void CloneFrom(IDataObjectComponent other)
		{
			DataColumnsCache dataColumnsCache = (DataColumnsCache)other;
			if (this.Count != dataColumnsCache.Count)
			{
				throw new ArgumentException(Strings.CountWrong, "other");
			}
			for (int i = 0; i < this.cache.Length; i++)
			{
				ColumnCache columnCache = this.cache[i];
				ColumnCache columnCache2 = dataColumnsCache.cache[i];
				if (columnCache.GetType() != columnCache2.GetType())
				{
					throw new InvalidOperationException();
				}
				columnCache.CloneFrom(columnCache2);
			}
		}

		public void MarkDirtyForReload()
		{
			foreach (ColumnCache columnCache in this.cache)
			{
				if (columnCache.HasValue)
				{
					columnCache.Dirty = true;
				}
			}
		}

		public void MakeKey(DataTableCursor cursor)
		{
			if (cursor == null)
			{
				throw new ArgumentNullException("cursor");
			}
			DataTableView tableView = base.DataRow.TableView;
			DataTable table = tableView.Table;
			for (int i = 0; i < table.KeyCount; i++)
			{
				DataColumn dataColumn = table.Schemas[i];
				int columnViewIndex = tableView.GetColumnViewIndex(i);
				byte[] data = dataColumn.BytesFromCache(this.cache[columnViewIndex]);
				Api.MakeKey(cursor.Session, cursor.TableId, data, (i == 0) ? MakeKeyGrbit.NewKey : MakeKeyGrbit.None);
			}
		}

		public void MakeStartPrefixKey(DataTableCursor cursor, int prefixColumns)
		{
			if (cursor == null)
			{
				throw new ArgumentNullException("cursor");
			}
			DataTableView tableView = base.DataRow.TableView;
			DataTable table = tableView.Table;
			if (prefixColumns > table.KeyCount)
			{
				throw new InvalidOperationException("columns in start prefix must be less than or equal to KeyCount");
			}
			for (int i = 0; i < prefixColumns; i++)
			{
				DataColumn dataColumn = table.Schemas[i];
				MakeKeyGrbit makeKeyGrbit = MakeKeyGrbit.None;
				if (i == 0)
				{
					makeKeyGrbit |= MakeKeyGrbit.NewKey;
				}
				if (i == prefixColumns - 1)
				{
					makeKeyGrbit |= MakeKeyGrbit.FullColumnStartLimit;
				}
				int columnViewIndex = tableView.GetColumnViewIndex(i);
				byte[] data = dataColumn.BytesFromCache(this.cache[columnViewIndex]);
				Api.MakeKey(cursor.Session, cursor.TableId, data, makeKeyGrbit);
			}
		}

		private void LoadAutoGeneratedColumns(DataTableCursor cursor)
		{
			DataTableView tableView = base.DataRow.TableView;
			DataTable table = tableView.Table;
			List<ColumnValue> list = new List<ColumnValue>(this.cache.Length);
			for (int i = 0; i < this.cache.Length; i++)
			{
				int columnRowIndex = tableView.GetColumnRowIndex(i);
				DataColumn dataColumn = table.Schemas[columnRowIndex];
				if (dataColumn.IsAutoGenerated)
				{
					ColumnValue columnValueForRetrieval = dataColumn.GetColumnValueForRetrieval();
					columnValueForRetrieval.RetrieveGrbit = RetrieveColumnGrbit.RetrieveCopy;
					list.Add(columnValueForRetrieval);
				}
			}
			ColumnValue[] values = list.ToArray();
			Api.RetrieveColumns(cursor.Session, cursor.TableId, values);
			int num = 0;
			for (int j = 0; j < this.cache.Length; j++)
			{
				int columnRowIndex2 = tableView.GetColumnRowIndex(j);
				DataColumn dataColumn2 = table.Schemas[columnRowIndex2];
				if (dataColumn2.IsAutoGenerated)
				{
					ColumnValue columnValue = list[num++];
					object valueAsObject = columnValue.ValueAsObject;
					if (valueAsObject != null)
					{
						dataColumn2.ColumnValueToCache(columnValue, this.cache[j]);
					}
					else
					{
						this.cache[j].HasValue = false;
					}
					this.cache[j].Dirty = false;
				}
			}
		}

		private ColumnCache[] cache;
	}
}
