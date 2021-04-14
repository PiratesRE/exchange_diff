using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class DataRow : DisposableBase
	{
		private DataRow(CultureInfo culture, Table table, bool newItem, bool writeThrough)
		{
			this.table = table;
			this.culture = culture;
			this.state = (newItem ? DataRow.DataRowState.New : DataRow.DataRowState.Existing);
			this.writeThrough = writeThrough;
			this.objects = new object[this.table.Columns.Count];
			this.primaryKey = new object[this.table.PrimaryKeyIndex.ColumnCount];
			this.dirtyColumn = new BitArray(this.table.Columns.Count, false);
			this.fetched = new BitArray(this.table.Columns.Count, newItem);
		}

		protected DataRow(DataRow.CreateDataRowFlag createFlag, CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, ColumnValue[] initialValues) : this(culture, table, true, writeThrough)
		{
			foreach (ColumnValue columnValue in initialValues)
			{
				PhysicalColumn physicalColumn = (PhysicalColumn)columnValue.Column;
				int num = table.PrimaryKeyIndex.PositionInIndex(physicalColumn);
				if (num >= 0)
				{
					this.primaryKey[num] = columnValue.Value;
				}
				this.dirtyColumn[physicalColumn.Index] = true;
				this.objects[physicalColumn.Index] = columnValue.Value;
				this.fetched[physicalColumn.Index] = true;
			}
			this.SetDirtyFlag(connectionProvider);
			for (int j = 0; j < table.Columns.Count; j++)
			{
				PhysicalColumn physicalColumn2 = table.Columns[j];
				if (physicalColumn2.StreamSupport && !physicalColumn2.IsNullable)
				{
					this.SetValue(connectionProvider, physicalColumn2, Array<byte>.Empty);
				}
			}
		}

		protected DataRow(DataRow.OpenDataRowFlag openFlag, CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, ColumnValue[] primaryKeyValues) : this(culture, table, false, writeThrough)
		{
			this.SetPrimaryKey(primaryKeyValues);
		}

		protected DataRow(DataRow.OpenDataRowFlag openFlag, CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, Reader reader) : this(culture, table, false, writeThrough)
		{
			List<Column> list = new List<Column>(this.table.Columns.Count);
			list.AddRange(this.table.CommonColumns);
			for (int i = 0; i < this.table.PrimaryKeyIndex.Columns.Count; i++)
			{
				Column item = this.table.PrimaryKeyIndex.Columns[i];
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			this.LoadFromReader(reader, list);
		}

		public CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public bool IsNew
		{
			get
			{
				return this.state == DataRow.DataRowState.New;
			}
		}

		public bool IsExisting
		{
			get
			{
				return this.state == DataRow.DataRowState.Existing;
			}
		}

		public bool IsDisconnected
		{
			get
			{
				return this.state == DataRow.DataRowState.Disconnected;
			}
		}

		public bool IsDead
		{
			get
			{
				return this.state == DataRow.DataRowState.Dead;
			}
		}

		public bool IsDirty
		{
			get
			{
				return this.dirty;
			}
		}

		public bool WriteThrough
		{
			get
			{
				return this.writeThrough;
			}
		}

		public bool CacheDiscardedForTest
		{
			get
			{
				return this.cacheDiscarded;
			}
		}

		public int DirtyColumnCount
		{
			get
			{
				int num = 0;
				foreach (object obj in this.dirtyColumn)
				{
					bool flag = (bool)obj;
					if (flag)
					{
						num++;
					}
				}
				return num;
			}
		}

		protected object[] PrimaryKey
		{
			get
			{
				return this.primaryKey;
			}
		}

		public void SetPrimaryKey(params ColumnValue[] primaryKeyValues)
		{
			foreach (ColumnValue columnValue in primaryKeyValues)
			{
				PhysicalColumn physicalColumn = (PhysicalColumn)columnValue.Column;
				int num = this.table.PrimaryKeyIndex.PositionInIndex(physicalColumn);
				this.primaryKey[num] = columnValue.Value;
				this.objects[physicalColumn.Index] = columnValue.Value;
				this.fetched[physicalColumn.Index] = true;
			}
		}

		public void DiscardCache(IConnectionProvider connectionProvider, bool discardUnsavedChanges)
		{
			for (int i = 0; i < this.objects.Length; i++)
			{
				if (this.table.PrimaryKeyIndex.PositionInIndex(this.table.Columns[i]) < 0 && (discardUnsavedChanges || !this.dirtyColumn[i]))
				{
					IDisposable disposable = this.objects[i] as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
					this.fetched[i] = false;
					this.objects[i] = null;
				}
			}
			if (discardUnsavedChanges && this.dirty)
			{
				this.ClearDirtyFlag(connectionProvider, null);
			}
			this.cacheDiscarded = true;
		}

		public ErrorCode ReloadCacheIfDiscarded(IConnectionProvider connectionProvider)
		{
			ErrorCode result = ErrorCode.NoError;
			if (!this.cacheDiscarded)
			{
				return ErrorCode.NoError;
			}
			if (this.table.CommonColumns.Count > 0)
			{
				result = this.Load(connectionProvider, this.table.CommonColumns);
			}
			this.cacheDiscarded = false;
			return result;
		}

		public void MarkDisconnected()
		{
			this.state = DataRow.DataRowState.Disconnected;
		}

		public void MarkReconnected()
		{
			this.state = DataRow.DataRowState.Existing;
		}

		internal DataRow VerifyAndLoad(IConnectionProvider connectionProvider)
		{
			if (this.Load(connectionProvider, this.table.CommonColumns) != ErrorCode.NoError)
			{
				base.Dispose();
				return null;
			}
			return this;
		}

		public ErrorCode Load(IConnectionProvider connectionProvider, params PhysicalColumn[] columns)
		{
			return this.Load(connectionProvider, (IList<PhysicalColumn>)columns, false);
		}

		public ErrorCode Load(IConnectionProvider connectionProvider, IList<PhysicalColumn> columns)
		{
			return this.Load(connectionProvider, columns, false);
		}

		public ErrorCode Load(IConnectionProvider connectionProvider, IList<PhysicalColumn> columns, bool loadFullStreamableColumns)
		{
			bool flag;
			IList<Column> columnsToLoad = this.GetColumnsToLoad(columns, loadFullStreamableColumns, out flag);
			if (columnsToLoad.Count == 0 && !flag)
			{
				columnsToLoad.Add(this.table.PrimaryKeyIndex.Columns[this.table.PrimaryKeyIndex.ColumnCount - 1]);
			}
			if (columnsToLoad.Count != 0)
			{
				StartStopKey startStopKey = new StartStopKey(true, this.PrimaryKey);
				using (TableOperator tableOperator = Factory.CreateTableOperator(this.culture, connectionProvider, this.Table, this.Table.PrimaryKeyIndex, columnsToLoad, null, null, 0, 1, new KeyRange(startStopKey, startStopKey), false, true))
				{
					using (Reader reader = tableOperator.ExecuteReader(false))
					{
						if (!reader.Read())
						{
							return ErrorCode.CreateNotFound((LID)65304U);
						}
						this.LoadFromReader(reader, columnsToLoad);
					}
				}
			}
			if (flag)
			{
				this.LoadStreamColumns(connectionProvider, columns, loadFullStreamableColumns);
			}
			return ErrorCode.NoError;
		}

		public void MarkAsDeleted(IConnectionProvider connectionProvider)
		{
			if (!this.IsDead)
			{
				try
				{
					if (this.IsDirty)
					{
						this.ClearDirtyFlag(connectionProvider, null);
					}
				}
				finally
				{
					this.state = DataRow.DataRowState.Dead;
				}
			}
		}

		public void Delete(IConnectionProvider connectionProvider)
		{
			try
			{
				if (!this.IsNew)
				{
					StartStopKey startStopKey = new StartStopKey(true, this.PrimaryKey);
					using (DeleteOperator deleteOperator = Factory.CreateDeleteOperator(this.culture, connectionProvider, Factory.CreateTableOperator(this.culture, connectionProvider, this.Table, this.Table.PrimaryKeyIndex, null, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true), true))
					{
						int num = (int)deleteOperator.ExecuteScalar();
					}
				}
			}
			finally
			{
				this.MarkAsDeleted(connectionProvider);
			}
		}

		public void Flush(IConnectionProvider connectionProvider)
		{
			this.Flush(connectionProvider, true);
		}

		public void Flush(IConnectionProvider connectionProvider, bool flushLargeDirtyStreams)
		{
			if (!this.IsNew)
			{
				this.Update(connectionProvider, flushLargeDirtyStreams, null);
				return;
			}
			this.Insert(connectionProvider, flushLargeDirtyStreams);
		}

		public int GetLargeDirtyStreamsSize()
		{
			int num = 0;
			if (this.IsDirty)
			{
				for (int i = 0; i < this.Table.Columns.Count; i++)
				{
					PhysicalColumn physicalColumn = this.Table.Columns[i];
					if (this.dirtyColumn[physicalColumn.Index])
					{
						object obj = this.objects[physicalColumn.Index];
						Stream stream = obj as Stream;
						if (stream != null && stream.Length > (long)Factory.GetOptimalStreamChunkSize())
						{
							num += (int)stream.Length;
						}
					}
				}
			}
			return num;
		}

		public IEnumerable<bool> FlushDirtyStreams(IConnectionProvider connectionProvider)
		{
			if (this.IsDirty)
			{
				byte[] streamBuffer = DataRow.GetBufferPool(Factory.GetOptimalStreamChunkSize()).Acquire();
				BitArray stillDirtyColumn = null;
				for (int i = 0; i < this.Table.Columns.Count; i++)
				{
					PhysicalColumn c = this.Table.Columns[i];
					if (this.dirtyColumn[c.Index])
					{
						Stream valueStream = this.objects[c.Index] as Stream;
						if (valueStream != null && valueStream.Length > (long)Factory.GetOptimalStreamChunkSize())
						{
							long position = 0L;
							valueStream.Position = position;
							do
							{
								int count = valueStream.Read(streamBuffer, 0, Factory.GetOptimalStreamChunkSize());
								this.WriteBytesToStream(connectionProvider, c, position, streamBuffer, 0, count);
								position += (long)count;
								yield return false;
							}
							while (position < valueStream.Length);
							this.dirtyColumn[c.Index] = false;
						}
						else
						{
							if (stillDirtyColumn == null)
							{
								stillDirtyColumn = new BitArray(this.table.Columns.Count, false);
							}
							stillDirtyColumn[c.Index] = true;
						}
					}
				}
				this.ClearDirtyFlag(connectionProvider, stillDirtyColumn);
			}
			yield break;
		}

		public object GetValue(IConnectionProvider connectionProvider, PhysicalColumn column)
		{
			if (!this.fetched[column.Index])
			{
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, "faulting in column " + column.ToString());
				}
				this.Load(connectionProvider, new PhysicalColumn[]
				{
					column
				});
			}
			object obj = this.objects[column.Index];
			Stream stream = obj as Stream;
			if (stream != null)
			{
				int num = object.ReferenceEquals(column, this.Table.SpecialCols.OffPagePropertyBlob) ? 65536 : 8192;
				if (stream.Length < (long)num)
				{
					byte[] array = new byte[stream.Length];
					stream.Position = 0L;
					stream.Read(array, 0, array.Length);
					obj = array;
				}
				else
				{
					byte[] array2 = new byte[2048];
					stream.Position = 0L;
					stream.Read(array2, 0, array2.Length);
					obj = new LargeValue(stream.Length, array2);
				}
			}
			else if (obj != null && column.StreamSupport && !(obj is LargeValue))
			{
				byte[] array3 = (byte[])obj;
				int num2 = object.ReferenceEquals(column, this.Table.SpecialCols.OffPagePropertyBlob) ? 65536 : 8192;
				if (array3.Length >= num2)
				{
					byte[] array4 = new byte[2048];
					Array.Copy(array3, 0, array4, 0, array4.Length);
					obj = new LargeValue((long)array3.Length, array4);
				}
			}
			return obj;
		}

		public void SetValue(IConnectionProvider connectionProvider, PhysicalColumn column, object value)
		{
			this.SetValue(connectionProvider, column, value, false);
		}

		public void SetValue(IConnectionProvider connectionProvider, PhysicalColumn column, object value, bool notDirty)
		{
			if (this.fetched[column.Index] && !(this.objects[column.Index] is Stream) && !(this.objects[column.Index] is LargeValue) && ValueHelper.ValuesEqual(this.objects[column.Index], value))
			{
				return;
			}
			IDisposable disposable = this.objects[column.Index] as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			this.fetched[column.Index] = true;
			this.objects[column.Index] = value;
			int num = this.table.PrimaryKeyIndex.PositionInIndex(column);
			if (num >= 0)
			{
				this.primaryKey[num] = value;
			}
			if (!notDirty)
			{
				this.SetDirtyFlag(connectionProvider);
				this.dirtyColumn[column.Index] = true;
			}
		}

		public void DirtyValue(IConnectionProvider connectionProvider, PhysicalColumn column)
		{
			this.dirtyColumn[column.Index] = true;
			this.SetDirtyFlag(connectionProvider);
		}

		public bool ColumnFetched(PhysicalColumn column)
		{
			return this.fetched[column.Index];
		}

		public bool ColumnDirty(PhysicalColumn column)
		{
			return this.dirtyColumn[column.Index];
		}

		public int? GetColumnSize(IConnectionProvider connectionProvider, PhysicalColumn column)
		{
			int? result = null;
			if (!this.fetched[column.Index] && !column.StreamSupport && (column.Size == 0 || column.IsNullable))
			{
				this.Load(connectionProvider, new PhysicalColumn[]
				{
					column
				});
			}
			if (this.fetched[column.Index])
			{
				object obj = this.objects[column.Index];
				Stream stream = obj as Stream;
				if (stream != null)
				{
					result = new int?((int)stream.Length);
				}
				else if (obj is LargeValue)
				{
					result = new int?((int)((LargeValue)obj).ActualLength);
				}
				else
				{
					result = SizeOfColumn.GetColumnSize(column, obj);
				}
			}
			else if (!column.StreamSupport)
			{
				result = new int?(column.Size);
			}
			else
			{
				DataRow.LargeValueSize largeValueSize = (DataRow.LargeValueSize)this.objects[column.Index];
				if (largeValueSize != null)
				{
					result = new int?((int)largeValueSize.Size);
				}
				else
				{
					result = this.ColumnSize(connectionProvider, column);
					if (result != null)
					{
						largeValueSize = new DataRow.LargeValueSize
						{
							Size = (long)result.Value
						};
						this.objects[column.Index] = largeValueSize;
					}
					else
					{
						this.fetched[column.Index] = true;
					}
				}
			}
			return result;
		}

		public void WriteStream(IConnectionProvider connectionProvider, PhysicalColumn column, long position, byte[] buffer, int offset, int count, out long deltaSize)
		{
			deltaSize = 0L;
			DataRow.LargeValueSize largeValueSize = null;
			Stream stream = null;
			if (this.fetched[column.Index] && this.objects[column.Index] is LargeValue)
			{
				largeValueSize = new DataRow.LargeValueSize
				{
					Size = ((LargeValue)this.objects[column.Index]).ActualLength
				};
				this.fetched[column.Index] = false;
				this.objects[column.Index] = largeValueSize;
			}
			if (!this.fetched[column.Index])
			{
				if (this.WriteThrough)
				{
					largeValueSize = (DataRow.LargeValueSize)this.objects[column.Index];
					if (largeValueSize == null && this.GetColumnSize(connectionProvider, column) != null)
					{
						largeValueSize = (DataRow.LargeValueSize)this.objects[column.Index];
					}
				}
				else
				{
					this.Load(connectionProvider, (IList<PhysicalColumn>)new PhysicalColumn[]
					{
						column
					}, true);
				}
			}
			if (this.fetched[column.Index])
			{
				if (this.IsExisting && this.WriteThrough)
				{
					if (this.objects[column.Index] == null)
					{
						this.SetValue(connectionProvider, column, Array<byte>.Empty);
					}
					if (this.dirtyColumn[column.Index])
					{
						this.Update(connectionProvider, true, new PhysicalColumn[]
						{
							column
						});
					}
					byte[] array = this.objects[column.Index] as byte[];
					long size;
					if (array != null)
					{
						size = (long)array.Length;
					}
					else
					{
						size = ((Stream)this.objects[column.Index]).Length;
					}
					largeValueSize = new DataRow.LargeValueSize
					{
						Size = size
					};
					this.fetched[column.Index] = false;
					this.objects[column.Index] = largeValueSize;
				}
				else if (this.objects[column.Index] == null || this.objects[column.Index] is byte[])
				{
					stream = TempStream.CreateInstance();
					byte[] array2 = this.objects[column.Index] as byte[];
					if (array2 != null)
					{
						stream.Write(array2, 0, array2.Length);
					}
					this.objects[column.Index] = stream;
				}
				else
				{
					stream = (Stream)this.objects[column.Index];
					if (!stream.CanWrite)
					{
						Stream stream2 = TempStream.CreateInstance();
						stream.Position = 0L;
						TempStream.CopyStream(stream, stream2);
						stream.Dispose();
						stream = stream2;
						this.objects[column.Index] = stream;
					}
				}
			}
			if (stream == null)
			{
				if (position > largeValueSize.Size)
				{
					throw new NotSupportedException("writing beyond value size is currently not supported");
				}
				if (count == 0)
				{
					return;
				}
				this.WriteBytesToStream(connectionProvider, column, position, buffer, offset, count);
				long num = position + (long)count;
				if (num > largeValueSize.Size)
				{
					deltaSize = num - largeValueSize.Size;
					largeValueSize.Size = num;
					return;
				}
			}
			else
			{
				long length = stream.Length;
				if (position > length)
				{
					throw new NotSupportedException("writing beyond value size is currently not supported");
				}
				stream.Position = position;
				stream.Write(buffer, offset, count);
				if (stream.Length > length)
				{
					deltaSize = stream.Length - length;
				}
				this.SetDirtyFlag(connectionProvider);
				this.dirtyColumn[column.Index] = true;
			}
		}

		public int ReadStream(IConnectionProvider connectionProvider, PhysicalColumn column, long position, byte[] buffer, int offset, int count)
		{
			DataRow.LargeValueSize largeValueSize = null;
			if (!this.fetched[column.Index])
			{
				largeValueSize = (DataRow.LargeValueSize)this.objects[column.Index];
				if (largeValueSize == null && this.GetColumnSize(connectionProvider, column) != null)
				{
					largeValueSize = (DataRow.LargeValueSize)this.objects[column.Index];
				}
			}
			else if (this.objects[column.Index] is LargeValue)
			{
				largeValueSize = new DataRow.LargeValueSize
				{
					Size = ((LargeValue)this.objects[column.Index]).ActualLength
				};
			}
			if (this.fetched[column.Index] && !(this.objects[column.Index] is LargeValue))
			{
				if (this.objects[column.Index] == null)
				{
					throw new NullColumnException(column);
				}
				byte[] array = this.objects[column.Index] as byte[];
				if (array == null)
				{
					Stream stream = (Stream)this.objects[column.Index];
					stream.Position = position;
					return stream.Read(buffer, offset, count);
				}
				if (position >= (long)array.Length)
				{
					return 0;
				}
				int num = (int)Math.Min((long)array.Length - position, (long)count);
				Buffer.BlockCopy(array, (int)position, buffer, offset, num);
				return num;
			}
			else
			{
				if (position >= largeValueSize.Size)
				{
					return 0;
				}
				int count2 = (int)Math.Min((long)count, largeValueSize.Size - position);
				long num2 = (long)this.ReadBytesFromStream(connectionProvider, column, position, buffer, offset, count2);
				return (int)num2;
			}
		}

		public abstract bool CheckTableExists(IConnectionProvider connectionProvider);

		protected abstract int? ColumnSize(IConnectionProvider connectionProvider, PhysicalColumn column);

		protected abstract int ReadBytesFromStream(IConnectionProvider connectionProvider, PhysicalColumn column, long position, byte[] buffer, int offset, int count);

		protected abstract void WriteBytesToStream(IConnectionProvider connectionProvider, PhysicalColumn column, long position, byte[] buffer, int offset, int count);

		internal object GetCachedValueForTest(IConnectionProvider connectionProvider, PhysicalColumn column)
		{
			return this.objects[column.Index];
		}

		internal bool GetIsFetchedForTest(IConnectionProvider connectionProvider, PhysicalColumn column)
		{
			return this.fetched[column.Index];
		}

		[Conditional("DEBUG")]
		private void CheckColumn(PhysicalColumn column)
		{
		}

		private void LoadFromReader(Reader reader, IList<Column> columnsToLoad)
		{
			for (int i = 0; i < columnsToLoad.Count; i++)
			{
				PhysicalColumn physicalColumn = (PhysicalColumn)columnsToLoad[i];
				object value = reader.GetValue(physicalColumn);
				this.objects[physicalColumn.Index] = value;
				int num = this.table.PrimaryKeyIndex.PositionInIndex(physicalColumn);
				if (num >= 0)
				{
					this.primaryKey[num] = value;
				}
				this.fetched[physicalColumn.Index] = true;
			}
		}

		private IList<Column> GetColumnsToLoad(IList<PhysicalColumn> columns, bool loadFullStreamableColumns, out bool hasStreamColumns)
		{
			IList<Column> list = new List<Column>(columns.Count);
			hasStreamColumns = false;
			for (int i = 0; i < columns.Count; i++)
			{
				PhysicalColumn physicalColumn = columns[i];
				if (!this.fetched[physicalColumn.Index] || (loadFullStreamableColumns && this.objects[physicalColumn.Index] is LargeValue))
				{
					if (physicalColumn.StreamSupport)
					{
						hasStreamColumns = true;
					}
					else
					{
						list.Add(physicalColumn);
					}
				}
			}
			return list;
		}

		private void LoadStreamColumns(IConnectionProvider connectionProvider, IEnumerable<PhysicalColumn> columns, bool loadFullStreamableColumns)
		{
			foreach (PhysicalColumn physicalColumn in columns)
			{
				if ((!this.fetched[physicalColumn.Index] || (loadFullStreamableColumns && this.objects[physicalColumn.Index] is LargeValue)) && physicalColumn.StreamSupport)
				{
					if (this.objects[physicalColumn.Index] is LargeValue)
					{
						this.objects[physicalColumn.Index] = new DataRow.LargeValueSize
						{
							Size = ((LargeValue)this.objects[physicalColumn.Index]).ActualLength
						};
						this.fetched[physicalColumn.Index] = false;
					}
					DataRow.LargeValueSize largeValueSize = (DataRow.LargeValueSize)this.objects[physicalColumn.Index];
					if (largeValueSize == null && this.GetColumnSize(connectionProvider, physicalColumn) != null)
					{
						largeValueSize = (DataRow.LargeValueSize)this.objects[physicalColumn.Index];
					}
					if (largeValueSize != null)
					{
						int num = object.ReferenceEquals(physicalColumn, this.Table.SpecialCols.OffPagePropertyBlob) ? 65536 : 8192;
						if (!loadFullStreamableColumns && largeValueSize.Size >= (long)num)
						{
							byte[] array = new byte[2048];
							this.ReadBytesFromStream(connectionProvider, physicalColumn, 0L, array, 0, 2048);
							this.objects[physicalColumn.Index] = new LargeValue(largeValueSize.Size, array);
							this.fetched[physicalColumn.Index] = true;
						}
						else
						{
							BufferPool bufferPool = DataRow.GetBufferPool((int)Math.Min(largeValueSize.Size, (long)Factory.GetOptimalStreamChunkSize()));
							byte[] array2 = bufferPool.Acquire();
							try
							{
								Stream stream = TempStream.CreateInstance();
								int num3;
								for (long num2 = 0L; num2 < largeValueSize.Size; num2 += (long)num3)
								{
									num3 = this.ReadBytesFromStream(connectionProvider, physicalColumn, num2, array2, 0, Math.Min(array2.Length, (int)(largeValueSize.Size - num2)));
									stream.Write(array2, 0, num3);
								}
								this.objects[physicalColumn.Index] = stream;
								this.fetched[physicalColumn.Index] = true;
							}
							finally
							{
								bufferPool.Release(array2);
							}
						}
					}
				}
			}
		}

		public static BufferPool GetBufferPool(int bufferSize)
		{
			if (bufferSize > 8192)
			{
				return DataRow.largePool;
			}
			BufferPoolCollection.BufferSize bufferSize2;
			BufferPoolCollection.AutoCleanupCollection.TryMatchBufferSize(bufferSize, out bufferSize2);
			return BufferPoolCollection.AutoCleanupCollection.Acquire(bufferSize2);
		}

		private void Update(IConnectionProvider connectionProvider, bool flushLargeDirtyStreams, IList<PhysicalColumn> columns)
		{
			if (!this.dirty)
			{
				return;
			}
			List<Column> list = new List<Column>(10);
			List<object> list2 = new List<object>(10);
			List<byte[]> list3 = null;
			IList<PhysicalColumn> list4 = this.Table.Columns;
			BitArray bitArray = null;
			if (columns != null)
			{
				list4 = columns;
				bitArray = new BitArray(this.dirtyColumn);
				for (int i = 0; i < columns.Count; i++)
				{
					bitArray[columns[i].Index] = false;
				}
				if (bitArray.All(false))
				{
					bitArray = null;
				}
			}
			try
			{
				byte[] array = null;
				for (int j = 0; j < list4.Count; j++)
				{
					PhysicalColumn physicalColumn = list4[j];
					if (this.dirtyColumn[physicalColumn.Index])
					{
						object obj = this.objects[physicalColumn.Index];
						Stream stream = obj as Stream;
						if (stream != null)
						{
							int num;
							if (!flushLargeDirtyStreams && stream.Length > (long)Factory.GetOptimalStreamChunkSize())
							{
								if (bitArray == null)
								{
									bitArray = new BitArray(this.table.Columns.Count, false);
								}
								bitArray[physicalColumn.Index] = true;
								num = 0;
							}
							else
							{
								num = (int)Math.Min(stream.Length, (long)Factory.GetOptimalStreamChunkSize());
							}
							if (list3 == null)
							{
								list3 = new List<byte[]>();
							}
							byte[] array2 = DataRow.GetBufferPool(num).Acquire();
							list3.Add(array2);
							stream.Position = 0L;
							stream.Read(array2, 0, num);
							obj = new ArraySegment<byte>(array2, 0, num);
							if (num == Factory.GetOptimalStreamChunkSize())
							{
								array = array2;
							}
						}
						list.Add(physicalColumn);
						list2.Add(obj);
					}
				}
				if (list.Count != 0)
				{
					StartStopKey startStopKey = new StartStopKey(true, this.PrimaryKey);
					using (UpdateOperator updateOperator = Factory.CreateUpdateOperator(this.culture, connectionProvider, Factory.CreateTableOperator(this.culture, connectionProvider, this.Table, this.Table.PrimaryKeyIndex, null, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true), list, list2, true))
					{
						int num2 = (int)updateOperator.ExecuteScalar();
					}
				}
				if (flushLargeDirtyStreams && array != null)
				{
					for (int k = 0; k < list4.Count; k++)
					{
						PhysicalColumn physicalColumn2 = list4[k];
						if (this.dirtyColumn[physicalColumn2.Index])
						{
							Stream stream2 = this.objects[physicalColumn2.Index] as Stream;
							if (stream2 != null && stream2.Length > (long)Factory.GetOptimalStreamChunkSize())
							{
								this.CopyStreamToColumn(connectionProvider, physicalColumn2, stream2, (long)Factory.GetOptimalStreamChunkSize(), array);
							}
						}
					}
				}
				for (int l = 0; l < list4.Count; l++)
				{
					PhysicalColumn physicalColumn3 = list4[l];
					if (this.dirtyColumn[physicalColumn3.Index])
					{
						int num3 = this.table.PrimaryKeyIndex.PositionInIndex(physicalColumn3);
						if (num3 >= 0)
						{
							this.primaryKey[num3] = this.objects[physicalColumn3.Index];
						}
					}
				}
				this.ClearDirtyFlag(connectionProvider, bitArray);
			}
			finally
			{
				if (list3 != null)
				{
					foreach (byte[] array3 in list3)
					{
						DataRow.GetBufferPool(array3.Length).Release(array3);
					}
				}
			}
		}

		private void Insert(IConnectionProvider connectionProvider, bool flushLargeDirtyStreams)
		{
			List<Column> list = new List<Column>(this.Table.Columns.Count);
			List<object> list2 = new List<object>(this.Table.Columns.Count);
			PhysicalColumn physicalColumn = null;
			List<byte[]> list3 = null;
			BitArray bitArray = null;
			try
			{
				byte[] array = null;
				for (int i = 0; i < this.Table.Columns.Count; i++)
				{
					PhysicalColumn physicalColumn2 = this.Table.Columns[i];
					if (physicalColumn2.IsIdentity)
					{
						physicalColumn = physicalColumn2;
					}
					if (this.dirtyColumn[physicalColumn2.Index])
					{
						object obj = this.objects[physicalColumn2.Index];
						Stream stream = obj as Stream;
						if (stream != null)
						{
							int num;
							if (!flushLargeDirtyStreams && stream.Length > (long)Factory.GetOptimalStreamChunkSize())
							{
								if (bitArray == null)
								{
									bitArray = new BitArray(this.table.Columns.Count, false);
								}
								bitArray[physicalColumn2.Index] = true;
								num = 0;
							}
							else
							{
								num = (int)Math.Min(stream.Length, (long)Factory.GetOptimalStreamChunkSize());
							}
							if (list3 == null)
							{
								list3 = new List<byte[]>();
							}
							byte[] array2 = DataRow.GetBufferPool(num).Acquire();
							list3.Add(array2);
							stream.Position = 0L;
							stream.Read(array2, 0, num);
							obj = new ArraySegment<byte>(array2, 0, num);
							if (num == Factory.GetOptimalStreamChunkSize())
							{
								array = array2;
							}
						}
						list.Add(physicalColumn2);
						list2.Add(obj);
					}
				}
				object value;
				using (InsertOperator insertOperator = Factory.CreateInsertOperator(this.culture, connectionProvider, this.Table, null, list, list2, physicalColumn, true))
				{
					value = insertOperator.ExecuteScalar();
				}
				if (physicalColumn != null)
				{
					if (physicalColumn.Type == typeof(long))
					{
						this.objects[physicalColumn.Index] = Convert.ToInt64(value);
					}
					else
					{
						this.objects[physicalColumn.Index] = Convert.ToInt32(value);
					}
					this.fetched[physicalColumn.Index] = true;
					int num2 = this.table.PrimaryKeyIndex.PositionInIndex(physicalColumn);
					if (num2 >= 0)
					{
						this.primaryKey[num2] = this.objects[physicalColumn.Index];
					}
				}
				if (flushLargeDirtyStreams && array != null)
				{
					for (int j = 0; j < this.Table.Columns.Count; j++)
					{
						PhysicalColumn physicalColumn3 = this.Table.Columns[j];
						if (this.dirtyColumn[physicalColumn3.Index])
						{
							Stream stream2 = this.objects[physicalColumn3.Index] as Stream;
							if (stream2 != null && stream2.Length > (long)Factory.GetOptimalStreamChunkSize())
							{
								this.CopyStreamToColumn(connectionProvider, physicalColumn3, stream2, (long)Factory.GetOptimalStreamChunkSize(), array);
							}
						}
					}
				}
				this.state = DataRow.DataRowState.Existing;
				this.ClearDirtyFlag(connectionProvider, bitArray);
			}
			finally
			{
				if (list3 != null)
				{
					foreach (byte[] array3 in list3)
					{
						DataRow.GetBufferPool(array3.Length).Release(array3);
					}
				}
			}
		}

		private void CopyStreamToColumn(IConnectionProvider connectionProvider, PhysicalColumn column, Stream valueStream, long startPosition, byte[] tempBuffer)
		{
			long num = startPosition;
			valueStream.Position = num;
			do
			{
				int num2 = valueStream.Read(tempBuffer, 0, tempBuffer.Length);
				this.WriteBytesToStream(connectionProvider, column, num, tempBuffer, 0, num2);
				num += (long)num2;
			}
			while (num < valueStream.Length);
		}

		public void ClearDirtyFlag(IConnectionProvider connectionProvider, BitArray stillDirtyColumn)
		{
			this.dirtyColumn.Xor(this.dirtyColumn);
			if (stillDirtyColumn != null)
			{
				this.dirtyColumn.Or(stillDirtyColumn);
				return;
			}
			this.dirty = false;
			if (this.Table.TrackDirtyObjects)
			{
				connectionProvider.GetConnection().CleanDirtyObject(this);
				this.connectionDirtyThisObject = null;
			}
		}

		private void SetDirtyFlag(IConnectionProvider connectionProvider)
		{
			if (!this.dirty)
			{
				this.dirty = true;
				if (this.Table.TrackDirtyObjects)
				{
					connectionProvider.GetConnection().AddDirtyObject(this);
					this.connectionDirtyThisObject = connectionProvider.GetConnection();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DataRow>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.IsDirty && this.Table.TrackDirtyObjects)
				{
					this.connectionDirtyThisObject.GetConnection().CleanDirtyObject(this);
				}
				if (this.objects != null)
				{
					for (int i = 0; i < this.objects.Length; i++)
					{
						IDisposable disposable = this.objects[i] as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
			}
		}

		internal void AppendDirtyTrackingInfoToString(StringBuilder sb)
		{
			sb.Append("id = [");
			for (int i = 0; i < this.primaryKey.Length; i++)
			{
				sb.Append(this.table.Columns[i]);
				sb.Append(" = ");
				sb.AppendAsString(this.primaryKey[i]);
			}
			sb.Append("], type = [");
			sb.Append(this.table.Name);
			sb.Append("] dirty columns = [");
			for (int j = 0; j < this.table.Columns.Count; j++)
			{
				if (this.dirtyColumn[j])
				{
					sb.Append("column = [");
					sb.Append(this.table.Columns[j]);
					sb.Append("] value = [");
					sb.Append((this.objects[j] is Stream) ? "<stream>" : this.objects[j]);
					sb.Append("] ");
				}
			}
			sb.Append("]");
		}

		public const int MinimumColumnSizeToStream = 8192;

		public const int OffPageBlobColumnSizeToStream = 65536;

		public const int TruncatedLargeValueSize = 2048;

		public static readonly DataRow.CreateDataRowFlag Create = default(DataRow.CreateDataRowFlag);

		public static readonly DataRow.OpenDataRowFlag Open = default(DataRow.OpenDataRowFlag);

		private static BufferPool largePool = new BufferPool(Factory.GetOptimalStreamChunkSize(), 4, true, true);

		private BitArray fetched;

		private BitArray dirtyColumn;

		private DataRow.DataRowState state;

		private bool dirty;

		private bool cacheDiscarded;

		private readonly CultureInfo culture;

		private bool writeThrough;

		private Table table;

		private object[] objects;

		private object[] primaryKey;

		private Connection connectionDirtyThisObject;

		public struct CreateDataRowFlag
		{
		}

		public struct OpenDataRowFlag
		{
		}

		private enum DataRowState
		{
			Dead,
			New,
			Existing,
			Disconnected
		}

		private class LargeValueSize
		{
			public long Size;
		}
	}
}
