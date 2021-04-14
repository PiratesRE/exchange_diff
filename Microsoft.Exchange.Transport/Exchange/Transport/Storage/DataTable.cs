using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class DataTable
	{
		public DataTable()
		{
			Type type = base.GetType();
			this.name = type.Name;
			this.isNewTable = false;
			BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public;
			Type typeFromHandle = typeof(DataColumnDefinitionAttribute);
			int num = 0;
			int num2 = 0;
			foreach (FieldInfo fieldInfo in type.GetFields(bindingAttr))
			{
				if (fieldInfo.IsLiteral && !(fieldInfo.FieldType != typeof(int)))
				{
					if (!fieldInfo.IsPublic)
					{
						throw new InvalidOperationException();
					}
					DataColumnDefinitionAttribute[] array = (DataColumnDefinitionAttribute[])fieldInfo.GetCustomAttributes(typeFromHandle, true);
					if (array.Length != 0)
					{
						num++;
						num2 = Math.Max((int)fieldInfo.GetValue(null) + 1, num2);
					}
				}
			}
			if (num2 == 0)
			{
				throw new SchemaException(Strings.NoColumns);
			}
			if (num != num2)
			{
				throw new SchemaException(Strings.ColumnIndexesMustBeSequential);
			}
			DataColumn[] array2 = new DataColumn[num2];
			int[] array3 = new int[num2];
			foreach (FieldInfo fieldInfo2 in type.GetFields(bindingAttr))
			{
				if (fieldInfo2.IsLiteral && !(fieldInfo2.FieldType != typeof(int)))
				{
					DataColumnDefinitionAttribute[] array4 = (DataColumnDefinitionAttribute[])fieldInfo2.GetCustomAttributes(typeFromHandle, true);
					if (array4.Length != 0)
					{
						int num3 = (int)fieldInfo2.GetValue(null);
						if (array2[num3] != null)
						{
							throw new SchemaException(Strings.DuplicateColumnIndexes(array2[num3].Name, fieldInfo2.Name));
						}
						DataColumnDefinitionAttribute dataColumnDefinitionAttribute = array4[0];
						DataColumn dataColumn = DataColumn.CreateInstance(dataColumnDefinitionAttribute.ColumnType);
						dataColumn.Name = fieldInfo2.Name;
						dataColumn.Required = dataColumnDefinitionAttribute.Required;
						dataColumn.AutoIncrement = dataColumnDefinitionAttribute.AutoIncrement;
						dataColumn.AutoVersioned = dataColumnDefinitionAttribute.AutoVersioned;
						dataColumn.IntrinsicLV = dataColumnDefinitionAttribute.IntrinsicLV;
						dataColumn.MultiValued = dataColumnDefinitionAttribute.MultiValued;
						array2[num3] = dataColumn;
						if (dataColumnDefinitionAttribute.ColumnAccess == ColumnAccess.Stream)
						{
							array3[num3] = 3;
						}
						else
						{
							if (dataColumnDefinitionAttribute.ColumnAccess != ColumnAccess.CachedProp)
							{
								throw new SchemaException(Strings.ColumnAccessInvalid(fieldInfo2.Name));
							}
							dataColumn.Cached = true;
							array3[num3] = (dataColumnDefinitionAttribute.PrimaryKey ? 1 : 2);
						}
					}
				}
			}
			for (int k = 0; k < array3.Length; k++)
			{
				switch (array3[k])
				{
				case 1:
					this.KeyCount++;
					this.CacheCount++;
					break;
				case 2:
					this.CacheCount++;
					break;
				}
			}
			for (int l = 0; l < array2.Length; l++)
			{
				array2[l].CacheIndex = l;
			}
			this.allColumns = new DataTable.ColumnList(array2);
			this.defaultView = new DataTableView(this);
		}

		protected int OpenCursorCount
		{
			get
			{
				return this.cursorCreationLock.CurrentReadCount;
			}
		}

		public DataSource DataSource
		{
			get
			{
				return this.dataSource;
			}
		}

		public IList<DataColumn> Schemas
		{
			get
			{
				return this.allColumns;
			}
		}

		public DataTableView DefaultView
		{
			get
			{
				return this.defaultView;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public bool IsNewTable
		{
			get
			{
				return this.isNewTable;
			}
		}

		public bool IsAttached
		{
			get
			{
				return this.dataSource != null;
			}
		}

		protected static void Rename(DataConnection connection, string tableName, string newTableName)
		{
			try
			{
				Api.JetRenameTable(connection.Session, connection.Database, tableName, newTableName);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, connection.Source))
				{
					throw;
				}
			}
		}

		public virtual DataTableCursor OpenOrCreateCursor(DataConnection connection)
		{
			DataTableCursor result = null;
			try
			{
				result = this.OpenCursorInternal(connection, false, true);
			}
			catch (EsentObjectNotFoundException)
			{
				result = this.OpenCursorInternal(connection, true, false);
			}
			return result;
		}

		public virtual DataTableCursor OpenCursor(DataConnection connection)
		{
			try
			{
				return this.OpenCursorInternal(connection, false, false);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.dataSource))
				{
					throw;
				}
			}
			return null;
		}

		public virtual DataTableCursor GetCursor()
		{
			if (this.DataSource == null)
			{
				throw new InvalidOperationException("not attached to data source.");
			}
			DataConnection dataConnection = this.DataSource.DemandNewConnection();
			DataTableCursor result = this.OpenCursor(dataConnection);
			dataConnection.Release();
			return result;
		}

		public void Attach(DataSource source, DataConnection connection)
		{
			this.Attach(source, connection, this.name);
		}

		public void Attach(DataSource source, DataConnection connection, string tableName)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (tableName == null)
			{
				throw new ArgumentNullException("tableName");
			}
			this.dataSource = source;
			this.dataSource.AddRef();
			this.name = tableName;
			using (Transaction transaction = connection.BeginTransaction())
			{
				using (DataTableCursor dataTableCursor = this.OpenOrCreateCursor(connection))
				{
					this.AttachColumns(dataTableCursor.Session, connection.Database, dataTableCursor.TableId, this.Name);
					this.AttachPrimaryIndex(dataTableCursor.Session, dataTableCursor.TableId);
					this.AttachLoadInitValues(transaction, dataTableCursor);
				}
				transaction.Commit();
			}
		}

		public void Detach()
		{
			if (this.dataSource != null)
			{
				this.dataSource.Release();
				this.dataSource = null;
				this.isNewTable = false;
			}
		}

		public virtual bool TryDrop(DataConnection connection)
		{
			return this.TryDropInternal(connection, false);
		}

		public virtual void Drop(DataConnection connection)
		{
			this.TryDropInternal(connection, true);
		}

		public void ReleaseCursor()
		{
			this.cursorCreationLock.ExitReadLock();
		}

		internal void ValidateCachedColumn(int index)
		{
			if (index >= this.Schemas.Count)
			{
				throw new ArgumentException(Strings.IncorrectColumn, "column");
			}
			if (!this.Schemas[index].Cached)
			{
				throw new ArgumentException(Strings.InvalidColumn(this.Name, index), "column");
			}
		}

		protected void AttachPrimaryIndex(JET_SESID session, JET_TABLEID table)
		{
			if (this.IsNewTable && this.KeyCount > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.KeyCount; i++)
				{
					stringBuilder.AppendFormat("+{0}\0", this.allColumns[i].Name);
				}
				stringBuilder.Append("\0");
				string text = stringBuilder.ToString();
				try
				{
					Api.JetCreateIndex(session, table, "primary", CreateIndexGrbit.IndexUnique | CreateIndexGrbit.IndexPrimary, text, text.Length, 100);
				}
				catch (EsentErrorException ex)
				{
					if (!DataSource.HandleIsamException(ex, this.dataSource))
					{
						throw;
					}
				}
			}
		}

		protected virtual void AttachLoadInitValues(Transaction transaction, DataTableCursor cursor)
		{
		}

		protected bool TryStopOpenCursor()
		{
			return this.cursorCreationLock.TryEnterWriteLock();
		}

		protected void ContinueOpenCursor()
		{
			this.cursorCreationLock.ExitWriteLock();
		}

		protected void CopyRow(DataTableCursor cursorFrom, DataTableCursor cursorTo)
		{
			foreach (DataColumn dataColumn in this.Schemas)
			{
				dataColumn.CopyData(cursorFrom, cursorTo);
			}
		}

		private bool TryDropInternal(DataConnection connection, bool throwTableInUse = false)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			try
			{
				Api.JetDeleteTable(connection.Session, connection.Database, this.Name);
				this.Detach();
			}
			catch (EsentTableInUseException ex)
			{
				if (throwTableInUse && !DataSource.HandleIsamException(ex, this.dataSource))
				{
					throw;
				}
				return false;
			}
			catch (EsentErrorException ex2)
			{
				if (!DataSource.HandleIsamException(ex2, this.dataSource))
				{
					throw;
				}
			}
			return true;
		}

		private DataTableCursor OpenCursorInternal(DataConnection connection, bool createTable = false, bool throwIfTableNotFound = false)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			this.cursorCreationLock.EnterReadLock();
			try
			{
				JET_TABLEID tableId;
				if (createTable)
				{
					this.isNewTable = true;
					Api.JetCreateTable(connection.Session, connection.Database, this.Name, 16, 100, out tableId);
				}
				else
				{
					Api.JetOpenTable(connection.Session, connection.Database, this.Name, null, 0, OpenTableGrbit.None, out tableId);
				}
				return new DataTableCursor(tableId, connection, this);
			}
			catch (EsentObjectNotFoundException ex)
			{
				if (throwIfTableNotFound || !DataSource.HandleIsamException(ex, connection.Source))
				{
					this.cursorCreationLock.ExitReadLock();
					throw;
				}
			}
			catch (EsentErrorException ex2)
			{
				if (!DataSource.HandleIsamException(ex2, connection.Source))
				{
					this.cursorCreationLock.ExitReadLock();
					throw;
				}
			}
			this.cursorCreationLock.ExitReadLock();
			return null;
		}

		private void AttachColumns(JET_SESID session, JET_DBID database, JET_TABLEID table, string tableName)
		{
			IEnumerable<ColumnInfo> enumerable = null;
			IDictionary<string, ColumnInfo> dictionary = new Dictionary<string, ColumnInfo>(16, StringComparer.OrdinalIgnoreCase);
			try
			{
				if (!this.isNewTable)
				{
					enumerable = Api.GetTableColumns(session, database, tableName);
					foreach (ColumnInfo columnInfo in enumerable)
					{
						dictionary[columnInfo.Name] = columnInfo;
					}
				}
				int num = 0;
				int num2 = 0;
				foreach (DataColumn dataColumn in this.allColumns)
				{
					JET_COLUMNDEF jet_COLUMNDEF = dataColumn.MakeColumnDef();
					if (this.isNewTable)
					{
						JET_COLUMNID columnId;
						Api.JetAddColumn(session, table, dataColumn.Name, jet_COLUMNDEF, null, 0, out columnId);
						dataColumn.ColumnId = columnId;
					}
					else
					{
						bool flag = false;
						ColumnInfo columnInfo2;
						if (dictionary.TryGetValue(dataColumn.Name, out columnInfo2))
						{
							flag = true;
							num2++;
							if (columnInfo2.Coltyp != jet_COLUMNDEF.coltyp)
							{
								ExTraceGlobals.ExpoTracer.TraceDebug(0L, "Column {1} on table {0} expected data type {2}. Actual data type {3}; fatal error", new object[]
								{
									this.Name,
									dataColumn.Name,
									jet_COLUMNDEF.coltyp,
									columnInfo2.Coltyp
								});
								DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SchemaTypeMismatch, null, new object[]
								{
									this.DataSource.DatabasePath,
									this.Name,
									dataColumn.Name,
									jet_COLUMNDEF.coltyp,
									columnInfo2.Coltyp
								});
								this.dataSource.HandleSchemaException(new SchemaException(Strings.SchemaTypeMismatch(jet_COLUMNDEF.coltyp, columnInfo2.Coltyp), table, dataColumn));
							}
							dataColumn.ColumnId = columnInfo2.Columnid;
						}
						if (!flag)
						{
							if (dataColumn.Required)
							{
								string text = string.Format("Non-Nullable schema column {1} not found in existing table {0}; fatal error", this.Name, dataColumn.Name);
								ExTraceGlobals.ExpoTracer.TraceDebug(0L, text);
								DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SchemaRequiredColumnNotFound, null, new object[]
								{
									this.DataSource.DatabasePath,
									this.Name,
									dataColumn.Name
								});
								EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, text, ResultSeverityLevel.Warning, false);
								this.dataSource.HandleSchemaException(new SchemaException(Strings.SchemaRequiredColumnNotFound(this.Name, dataColumn.Name), table, dataColumn));
							}
							else
							{
								ExTraceGlobals.ExpoTracer.TraceDebug<string, string>(0L, "Nullable schema column {1} not found in existing table {0}; creating column", this.Name, dataColumn.Name);
								JET_COLUMNID columnId2;
								Api.JetAddColumn(session, table, dataColumn.Name, jet_COLUMNDEF, null, 0, out columnId2);
								dataColumn.ColumnId = columnId2;
							}
						}
					}
					num++;
				}
				if (!this.dataSource.NewDatabase && num2 != dictionary.Count)
				{
					foreach (ColumnInfo columnInfo3 in enumerable)
					{
						bool flag2 = false;
						foreach (DataColumn dataColumn2 in this.allColumns)
						{
							if (string.Equals(columnInfo3.Name, dataColumn2.Name, StringComparison.OrdinalIgnoreCase))
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							ExTraceGlobals.ExpoTracer.TraceDebug<string, string>(0L, "Column {1} from existing table {0} not found in schema; deleting column", this.Name, columnInfo3.Name);
							Api.JetDeleteColumn(session, table, columnInfo3.Name);
						}
					}
				}
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.dataSource))
				{
					throw;
				}
			}
		}

		private const int Density = 100;

		private const int GrowthInPages = 16;

		internal readonly int KeyCount;

		internal readonly int CacheCount;

		private readonly DataTable.UnsafeReaderWriterLock cursorCreationLock = new DataTable.UnsafeReaderWriterLock();

		private readonly DataTableView defaultView;

		private string name;

		private DataSource dataSource;

		private DataTable.ColumnList allColumns;

		private bool isNewTable;

		private class ColumnList : IList<DataColumn>, ICollection<DataColumn>, IEnumerable<DataColumn>, IEnumerable
		{
			public ColumnList(DataColumn[] data)
			{
				this.columns = data;
			}

			public int Count
			{
				get
				{
					return this.columns.Length;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public DataColumn this[int index]
			{
				get
				{
					return this.columns[index];
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public void Add(DataColumn item)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				throw new NotSupportedException();
			}

			public bool Contains(DataColumn item)
			{
				return item != null && this.columns.Length > item.CacheIndex && this.columns[item.CacheIndex] == item;
			}

			public void CopyTo(DataColumn[] array, int arrayIndex)
			{
			}

			public bool Remove(DataColumn item)
			{
				throw new NotSupportedException();
			}

			public int IndexOf(DataColumn item)
			{
				if (item == null)
				{
					return -1;
				}
				if (!this.Contains(item))
				{
					return -1;
				}
				return item.CacheIndex;
			}

			public void Insert(int index, DataColumn item)
			{
				throw new NotSupportedException();
			}

			public void RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			public IEnumerator<DataColumn> GetEnumerator()
			{
				return new DataTable.ColumnList.Enumerator(this.columns);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new DataTable.ColumnList.Enumerator(this.columns);
			}

			private DataColumn[] columns;

			public struct Enumerator : IEnumerator<DataColumn>, IDisposable, IEnumerator
			{
				internal Enumerator(DataColumn[] list)
				{
					this.list = list;
					this.index = 0;
					this.current = null;
				}

				public DataColumn Current
				{
					get
					{
						return this.current;
					}
				}

				object IEnumerator.Current
				{
					get
					{
						if (this.index == 0 || this.index > this.list.Length + 1)
						{
							throw new InvalidOperationException(Strings.EnumeratorBadPosition);
						}
						return this.Current;
					}
				}

				public void Dispose()
				{
				}

				public bool MoveNext()
				{
					if (this.index < this.list.Length)
					{
						this.current = this.list[this.index];
						this.index++;
						return true;
					}
					this.current = null;
					return false;
				}

				void IEnumerator.Reset()
				{
					this.index = 0;
					this.current = null;
				}

				private DataColumn[] list;

				private int index;

				private DataColumn current;
			}
		}

		private class UnsafeReaderWriterLock
		{
			public int CurrentReadCount
			{
				get
				{
					return this.readerCount;
				}
			}

			public bool TryEnterWriteLock()
			{
				this.openCursorEvent.Reset();
				return Interlocked.CompareExchange(ref this.readerCount, -1, 0) == 0;
			}

			public void ExitWriteLock()
			{
				Interlocked.CompareExchange(ref this.readerCount, 0, -1);
				this.openCursorEvent.Set();
			}

			public void EnterReadLock()
			{
				int num;
				do
				{
					num = this.readerCount;
					if (num == -1)
					{
						this.openCursorEvent.WaitOne();
					}
				}
				while (Interlocked.CompareExchange(ref this.readerCount, num + 1, num) != num);
			}

			public void ExitReadLock()
			{
				Interlocked.Decrement(ref this.readerCount);
			}

			private readonly ManualResetEvent openCursorEvent = new ManualResetEvent(true);

			private int readerCount;
		}
	}
}
