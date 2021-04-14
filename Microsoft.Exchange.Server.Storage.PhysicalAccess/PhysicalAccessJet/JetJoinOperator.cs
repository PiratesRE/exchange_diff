using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetJoinOperator : JoinOperator, IJetSimpleQueryOperator, IJetRecordCounter, ITWIR, IRowAccess, IColumnStreamAccess
	{
		internal JetJoinOperator(CultureInfo culture, IConnectionProvider connectionProvider, Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<Column> keyColumns, SimpleQueryOperator outerQuery, bool frequentOperation) : this(connectionProvider, new JoinOperator.JoinOperatorDefinition(culture, table, columnsToFetch, longValueColumnsToPreread, restriction, renameDictionary, skipTo, maxRows, keyColumns, outerQuery.OperatorDefinition, frequentOperation))
		{
		}

		internal JetJoinOperator(IConnectionProvider connectionProvider, JoinOperator.JoinOperatorDefinition definition) : base(connectionProvider, definition)
		{
			if (!this.CanUsePreRead())
			{
				base.PreReadCacheSize = 0;
				return;
			}
			if (base.MaxRows > 0 && base.SkipTo + base.MaxRows < 150)
			{
				base.PreReadCacheSize /= 2;
			}
			if (base.OuterQuery.MaxRows > 0)
			{
				base.PreReadCacheSize = Math.Min(base.PreReadCacheSize, base.OuterQuery.MaxRows);
			}
		}

		private JetConnection JetConnection
		{
			get
			{
				return (JetConnection)base.Connection;
			}
		}

		private IJetSimpleQueryOperator JetOuterQuery
		{
			get
			{
				return (IJetSimpleQueryOperator)base.OuterQuery;
			}
		}

		public override Reader ExecuteReader(bool disposeQueryOperator)
		{
			base.TraceOperation("ExecuteReader");
			this.ResetCachedColumns();
			base.Connection.CountStatement(Connection.OperationType.Query);
			return new JetReader(base.ConnectionProvider, this, disposeQueryOperator);
		}

		public override bool EnableInterrupts(IInterruptControl interruptControl)
		{
			if (interruptControl != null && base.SkipTo != 0)
			{
				return false;
			}
			if (!base.OuterQuery.EnableInterrupts(interruptControl))
			{
				return false;
			}
			this.interruptControl = interruptControl;
			this.outerCanMoveBack = (interruptControl != null && this.JetOuterQuery.CanMoveBack);
			return true;
		}

		public bool MoveFirst(out int rowsSkipped)
		{
			rowsSkipped = 0;
			this.rowsReturned = 0U;
			this.ResetCachedColumns();
			if (base.Criteria is SearchCriteriaFalse)
			{
				base.TraceMove("MoveFirst", false);
				return false;
			}
			if (!this.MoveFirstFromOuter(out rowsSkipped))
			{
				base.TraceMove("MoveFirst", false);
				return false;
			}
			this.SetupColumnCaching();
			int num = base.SkipTo;
			if (this.interrupted)
			{
				return true;
			}
			JetConnection jetConnection = this.JetConnection;
			bool flag = this.FetchFromKey(new StartStopKey(true, (IList<object>)this.outerValues.Key));
			if (flag)
			{
				jetConnection.IncrementRowStatsCounter(base.Table, RowStatsCounterType.Read);
				if (base.Criteria != null)
				{
					bool flag2 = base.Criteria.Evaluate(this, base.CompareInfo);
					bool? flag3 = new bool?(true);
					if (!flag2 || flag3 == null)
					{
						goto IL_FE;
					}
				}
				if (num <= 0)
				{
					base.TraceMove("MoveFirst", true);
					jetConnection.IncrementRowStatsCounter(base.Table, RowStatsCounterType.Accept);
					this.rowsReturned += 1U;
					return true;
				}
				rowsSkipped++;
				num--;
			}
			else
			{
				this.TraceInnerRowNotFound();
			}
			IL_FE:
			return this.MoveNext("MoveFirst", num, ref rowsSkipped);
		}

		public bool MoveNext()
		{
			int num = 0;
			return this.MoveNext("MoveNext", 0, ref num);
		}

		internal bool MoveNext(string operation, int numberLeftToSkip, ref int rowsSkipped)
		{
			bool flag = false;
			if (this.interrupted)
			{
				base.TraceCrumb(operation, "Resume");
				if (!this.Resume())
				{
					base.TraceMove(operation, false);
					return false;
				}
				this.interruptControl.Reset();
			}
			if (base.MaxRows <= 0 || (ulong)this.rowsReturned < (ulong)((long)base.MaxRows))
			{
				JetConnection jetConnection = this.JetConnection;
				using (base.Connection.TrackTimeInDatabase())
				{
					for (;;)
					{
						flag = false;
						this.ResetCachedColumns();
						flag = this.MoveNextFromOuter();
						if (!flag)
						{
							goto IL_123;
						}
						if (this.interrupted)
						{
							break;
						}
						flag = this.FetchFromKey(new StartStopKey(true, (IList<object>)this.outerValues.Key));
						if (!flag)
						{
							this.TraceInnerRowNotFound();
						}
						else
						{
							jetConnection.IncrementRowStatsCounter(base.Table, RowStatsCounterType.Read);
							if (base.Criteria != null)
							{
								bool flag2 = base.Criteria.Evaluate(this, base.CompareInfo);
								bool? flag3 = new bool?(true);
								if (!flag2 || flag3 == null)
								{
									continue;
								}
							}
							if (numberLeftToSkip <= 0)
							{
								goto IL_106;
							}
							rowsSkipped++;
							numberLeftToSkip--;
						}
					}
					return true;
					IL_106:
					jetConnection.IncrementRowStatsCounter(base.Table, RowStatsCounterType.Accept);
					flag = true;
					this.rowsReturned += 1U;
					IL_123:;
				}
			}
			base.TraceMove(operation, flag);
			return flag;
		}

		private void SetupColumnCaching()
		{
			if (this.columnCache != null)
			{
				return;
			}
			this.outerColumnsToFetch = new Dictionary<Column, int>(base.OuterQuery.ColumnsToFetch.Count);
			for (int i = 0; i < base.OuterQuery.ColumnsToFetch.Count; i++)
			{
				Column column = base.OuterQuery.ColumnsToFetch[i];
				if (!(column is ConstantColumn))
				{
					this.outerColumnsToFetch.Add(column, i);
				}
			}
			IList<Column> list2 = null;
			if (base.Criteria != null)
			{
				list2 = new List<Column>();
				base.Criteria.EnumerateColumns(delegate(Column c, object list)
				{
					((List<Column>)list).Add(c);
				}, list2, false);
			}
			HashSet<PhysicalColumn> hashSet = ConcurrentLookAside<HashSet<PhysicalColumn>>.Pool.Get();
			if (hashSet == null)
			{
				hashSet = new HashSet<PhysicalColumn>();
			}
			JetColumnValueHelper.GetPhysicalColumns(base.Table, list2, null, this, hashSet);
			HashSet<Column> hashSet2 = new HashSet<Column>(base.ColumnsToFetch);
			hashSet2.RemoveWhere(new Predicate<Column>(this.outerColumnsToFetch.ContainsKey));
			HashSet<PhysicalColumn> hashSet3 = ConcurrentLookAside<HashSet<PhysicalColumn>>.Pool.Get();
			if (hashSet3 == null)
			{
				hashSet3 = new HashSet<PhysicalColumn>();
			}
			JetColumnValueHelper.GetPhysicalColumns(base.Table, hashSet2, hashSet, this, hashSet3);
			JetConnection jetConnection = this.JetConnection;
			this.restrictedColumnValues = new CachedColumnValues(jetConnection, hashSet, null, null);
			this.fetchColumnValues = new CachedColumnValues(jetConnection, hashSet3, null, null);
			this.columnCache = new Dictionary<PhysicalColumn, object>(hashSet.Count + hashSet3.Count);
			if (hashSet != null)
			{
				hashSet.Clear();
				ConcurrentLookAside<HashSet<PhysicalColumn>>.Pool.Put(hashSet);
			}
			if (hashSet3 != null)
			{
				hashSet3.Clear();
				ConcurrentLookAside<HashSet<PhysicalColumn>>.Pool.Put(hashSet3);
			}
		}

		private byte[] GetPhysicalColumnValueAsBytes(PhysicalColumn column)
		{
			if (JetPartitionHelper.IsPartitioningColumn(base.Table, column))
			{
				return JetColumnValueHelper.GetAsByteArray(this.outerValues.Key[column.Index], column);
			}
			byte[] array = null;
			JetConnection jetConnection = this.JetConnection;
			if (object.ReferenceEquals(column, base.Table.SpecialCols.OffPagePropertyBlob) && !this.hitOffPageBlob)
			{
				jetConnection.IncrementOffPageBlobHits();
				this.hitOffPageBlob = true;
			}
			try
			{
				JetPhysicalColumn jetPhysicalColumn = (JetPhysicalColumn)column;
				JET_COLUMNID jetColumnId = jetPhysicalColumn.GetJetColumnId(jetConnection);
				using (jetConnection.TrackTimeInDatabase())
				{
					array = Api.RetrieveColumn(jetConnection.JetSession, this.jetCursor, jetColumnId);
					jetConnection.AddRowStatsCounter(base.Table, RowStatsCounterType.ReadBytes, (array == null) ? 0 : array.Length);
				}
			}
			catch (EsentErrorException ex)
			{
				jetConnection.OnExceptionCatch(ex);
				throw jetConnection.ProcessJetError((LID)47560U, "JetJoinOperator.GetColumn", ex);
			}
			return array;
		}

		private object GetPhysicalColumnValue(PhysicalColumn column)
		{
			Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table = base.Table;
			if (JetPartitionHelper.IsPartitioningColumn(table, column))
			{
				return this.outerValues.Key[column.Index];
			}
			object obj;
			if (this.columnCache.TryGetValue(column, out obj))
			{
				return obj;
			}
			JetConnection jetConnection = this.JetConnection;
			JetPhysicalColumn jetPhysicalColumn = (JetPhysicalColumn)column;
			JET_TABLEID tableid = this.jetCursor;
			JET_COLUMNID jetColumnId = jetPhysicalColumn.GetJetColumnId(jetConnection);
			if (object.ReferenceEquals(column, base.Table.SpecialCols.OffPagePropertyBlob) && !this.hitOffPageBlob)
			{
				jetConnection.IncrementOffPageBlobHits();
				this.hitOffPageBlob = true;
			}
			if (!this.restrictedColumnValues.TryGetValue(jetConnection, table, tableid, jetColumnId, out obj) && !this.fetchColumnValues.TryGetValue(jetConnection, table, tableid, jetColumnId, out obj))
			{
				if (this.intermediateTracingEnabled && !column.StreamSupport)
				{
					ExTraceGlobals.DbInteractionIntermediateTracer.TraceDebug<JetPhysicalColumn, JET_COLUMNID>(0L, "Column {0} (columnid {1}) isn't in the retrieval set. Retrieving directly from Jet", jetPhysicalColumn, jetColumnId);
				}
				if (jetPhysicalColumn.StreamSupport)
				{
					obj = JetColumnValueHelper.LoadFullOrTruncatedValue(jetPhysicalColumn, this);
				}
				else
				{
					Microsoft.Isam.Esent.Interop.ColumnValue columnValue = JetColumnValueHelper.CreateColumnValue((JetConnection)base.Connection, jetPhysicalColumn, null, null);
					try
					{
						using (jetConnection.TrackTimeInDatabase())
						{
							Api.RetrieveColumns(jetConnection.JetSession, tableid, new Microsoft.Isam.Esent.Interop.ColumnValue[]
							{
								columnValue
							});
						}
					}
					catch (EsentErrorException ex)
					{
						jetConnection.OnExceptionCatch(ex);
						throw jetConnection.ProcessJetError((LID)50992U, "JetJoinOperator.GetPhysicalColumnValue", ex);
					}
					obj = columnValue.ValueAsObject;
					jetConnection.AddRowStatsCounter(table, RowStatsCounterType.ReadBytes, columnValue.Length);
				}
			}
			obj = JetColumnValueHelper.GetValueFromJetValue(column, obj);
			this.columnCache[column] = obj;
			return obj;
		}

		private object GetPropertyColumnValue(PropertyColumn column)
		{
			if (this.rowPropertyBag == null)
			{
				this.rowPropertyBag = column.PropertyBagCreator(this);
			}
			return this.rowPropertyBag.GetPropertyValue(base.Connection, column.StorePropTag);
		}

		byte[] IJetSimpleQueryOperator.GetColumnValueAsBytes(Column column)
		{
			if (base.RenameDictionary != null)
			{
				column = base.ResolveColumn(column);
			}
			int outerColumnIndex;
			if (this.outerColumnsToFetch.TryGetValue(column, out outerColumnIndex))
			{
				return JetColumnValueHelper.GetAsByteArray(this.GetOuterValue(outerColumnIndex), column);
			}
			IJetColumn jetColumn = (IJetColumn)column;
			return jetColumn.GetValueAsBytes(this);
		}

		byte[] IJetSimpleQueryOperator.GetPhysicalColumnValueAsBytes(PhysicalColumn column)
		{
			return this.GetPhysicalColumnValueAsBytes(column);
		}

		int IJetRecordCounter.GetCount()
		{
			return JetSimpleQueryOperatorHelper.GetCount(this);
		}

		int IJetRecordCounter.GetOrdinalPosition(SortOrder sortOrder, StartStopKey stopKey, CompareInfo compareInfo)
		{
			return JetSimpleQueryOperatorHelper.GetOrdinalPosition(this, sortOrder, stopKey, compareInfo);
		}

		int ITWIR.GetColumnSize(Column column)
		{
			if (base.RenameDictionary != null)
			{
				column = base.ResolveColumn(column);
			}
			int outerColumnIndex;
			if (this.outerColumnsToFetch.TryGetValue(column, out outerColumnIndex))
			{
				return SizeOfColumn.GetColumnSize(column, this.GetOuterValue(outerColumnIndex)).GetValueOrDefault(0);
			}
			IColumn column2 = column;
			return column2.GetSize(this);
		}

		object ITWIR.GetColumnValue(Column column)
		{
			if (base.RenameDictionary != null)
			{
				column = base.ResolveColumn(column);
			}
			int outerColumnIndex;
			if (this.outerColumnsToFetch.TryGetValue(column, out outerColumnIndex))
			{
				return this.GetOuterValue(outerColumnIndex);
			}
			IColumn column2 = column;
			return column2.GetValue(this);
		}

		int ITWIR.GetPhysicalColumnSize(PhysicalColumn column)
		{
			return this.GetPhysicalColumnSize(column, false);
		}

		internal int GetPhysicalColumnCompressedSize(PhysicalColumn column)
		{
			return this.GetPhysicalColumnSize(column, true);
		}

		private int GetPhysicalColumnSize(PhysicalColumn column, bool compressedSize)
		{
			if (JetPartitionHelper.IsPartitioningColumn(base.Table, column))
			{
				return column.Size;
			}
			JetConnection jetConnection = this.JetConnection;
			if (object.ReferenceEquals(column, base.Table.SpecialCols.OffPagePropertyBlob) && !this.hitOffPageBlob)
			{
				jetConnection.IncrementOffPageBlobHits();
				this.hitOffPageBlob = true;
			}
			int result;
			try
			{
				JetPhysicalColumn jetPhysicalColumn = (JetPhysicalColumn)column;
				JET_COLUMNID jetColumnId = jetPhysicalColumn.GetJetColumnId(jetConnection);
				RetrieveColumnGrbit retrieveColumnSizeGrbit = JetTableOperator.GetRetrieveColumnSizeGrbit(compressedSize);
				using (jetConnection.TrackTimeInDatabase())
				{
					int? num = Api.RetrieveColumnSize(jetConnection.JetSession, this.jetCursor, jetColumnId, 1, retrieveColumnSizeGrbit);
					jetConnection.AddRowStatsCounter(base.Table, RowStatsCounterType.ReadBytes, 4);
					result = (num ?? 0);
				}
			}
			catch (EsentErrorException ex)
			{
				jetConnection.OnExceptionCatch(ex);
				throw jetConnection.ProcessJetError((LID)63944U, "JetJoinOperator.GetPhysicalColumnSize", ex);
			}
			return result;
		}

		object ITWIR.GetPhysicalColumnValue(PhysicalColumn column)
		{
			return this.GetPhysicalColumnValue(column);
		}

		int ITWIR.GetPropertyColumnSize(PropertyColumn column)
		{
			object propertyColumnValue = this.GetPropertyColumnValue(column);
			return SizeOfColumn.GetColumnSize(column, propertyColumnValue).GetValueOrDefault();
		}

		object ITWIR.GetPropertyColumnValue(PropertyColumn column)
		{
			return this.GetPropertyColumnValue(column);
		}

		object IRowAccess.GetPhysicalColumn(PhysicalColumn column)
		{
			if (column.Index == -1 && base.RenameDictionary != null)
			{
				Column column2 = base.ResolveColumn(column);
				if (column2 != column)
				{
					return column2.Evaluate(this);
				}
			}
			return this.GetPhysicalColumnValue(column);
		}

		int IColumnStreamAccess.GetColumnSize(PhysicalColumn column)
		{
			return ((ITWIR)this).GetPhysicalColumnSize(column);
		}

		int IColumnStreamAccess.ReadStream(PhysicalColumn physicalColumn, long position, byte[] buffer, int offset, int count)
		{
			if (this.summaryTracingEnabled)
			{
				base.TraceOperation("ReadBytesFromStream", string.Format("position:[{0}] count:[{1}]", position, count));
			}
			JetConnection jetConnection = this.JetConnection;
			JetPhysicalColumn jetPhysicalColumn = (JetPhysicalColumn)physicalColumn;
			if (position == 0L && object.ReferenceEquals(physicalColumn, base.Table.SpecialCols.OffPagePropertyBlob) && !this.hitOffPageBlob)
			{
				jetConnection.IncrementOffPageBlobHits();
				this.hitOffPageBlob = true;
			}
			JET_RETINFO jet_RETINFO = new JET_RETINFO();
			jet_RETINFO.ibLongValue = (int)position;
			jet_RETINFO.itagSequence = 1;
			ArraySegment<byte> userBuffer = new ArraySegment<byte>(buffer, offset, count);
			long? num = JetRetrieveColumnHelper.RetrieveColumnValueToArraySegment(jetConnection, this.jetCursor, jetPhysicalColumn.GetJetColumnId(jetConnection), userBuffer, jet_RETINFO);
			if (this.intermediateTracingEnabled)
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append(" <<< Read chunk  col:[");
				jetPhysicalColumn.AppendToString(stringBuilder, StringFormatOptions.None);
				stringBuilder.Append("]=[");
				if (this.detailTracingEnabled || num.Value <= 32L)
				{
					stringBuilder.AppendAsString(buffer, offset, (int)num.GetValueOrDefault());
				}
				else
				{
					stringBuilder.Append("<long_blob>");
				}
				stringBuilder.Append("]");
				ExTraceGlobals.DbInteractionIntermediateTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			return (int)num.GetValueOrDefault();
		}

		void IColumnStreamAccess.WriteStream(PhysicalColumn physicalColumn, long position, byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("Write is not supported");
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JetJoinOperator>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				try
				{
					if (this.jetCursorOpen)
					{
						this.CloseJetCursor();
					}
					if (this.detailTracingEnabled)
					{
						StringBuilder stringBuilder = new StringBuilder(200);
						base.AppendOperationInfo("Dispose", stringBuilder);
						stringBuilder.Append("  rowsRead:[");
						stringBuilder.Append(this.rowsRead);
						stringBuilder.Append("]");
						ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, stringBuilder.ToString());
					}
				}
				finally
				{
					base.InternalDispose(calledFromDispose);
				}
			}
		}

		private void TraceInnerRowNotFound()
		{
			if (this.intermediateTracingEnabled)
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				base.AppendOperationInfo("INNER ROW NOT FOUND", stringBuilder);
				if (base.OuterQuery.ColumnsToFetch != null)
				{
					stringBuilder.Append("  outer_columns:[");
					if (base.PreReadCacheSize > 1)
					{
						for (int i = 0; i < base.OuterQuery.ColumnsToFetch.Count; i++)
						{
							if (i != 0)
							{
								stringBuilder.Append(", ");
							}
							base.OuterQuery.ColumnsToFetch[i].AppendToString(stringBuilder, StringFormatOptions.None);
							stringBuilder.Append("=[");
							stringBuilder.AppendAsString(this.GetOuterValue(i));
							stringBuilder.Append("]");
						}
					}
					else
					{
						base.TraceAppendColumns(stringBuilder, this.JetOuterQuery, base.OuterQuery.ColumnsToFetch);
					}
					stringBuilder.Append("]");
				}
				ExTraceGlobals.DbInteractionIntermediateTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		private bool FetchFromKey(StartStopKey startKey)
		{
			if (!this.TryOpenJetCursorIfNecessary(startKey))
			{
				return false;
			}
			JetConnection jetConnection = this.JetConnection;
			bool result = false;
			try
			{
				using (jetConnection.TrackTimeInDatabase())
				{
					int numberOfPartioningColumns = base.Table.SpecialCols.NumberOfPartioningColumns;
					for (int i = numberOfPartioningColumns; i < startKey.Values.Count; i++)
					{
						MakeKeyGrbit grbit = (i == numberOfPartioningColumns) ? MakeKeyGrbit.NewKey : MakeKeyGrbit.None;
						JetColumnValueHelper.MakeJetKeyFromValue(jetConnection.JetSession, this.jetCursor, grbit, startKey.Values[i], this.GetKeyIndex().SortOrder.Columns[i]);
					}
					jetConnection.IncrementRowStatsCounter(base.Table, RowStatsCounterType.Seek);
					result = Api.TrySeek(jetConnection.JetSession, this.jetCursor, SeekGrbit.SeekEQ);
				}
				this.rowsRead += 1U;
			}
			catch (EsentErrorException ex)
			{
				jetConnection.OnExceptionCatch(ex);
				throw jetConnection.ProcessJetError((LID)55752U, "JetJoinOperator.FetchFromKey", ex);
			}
			return result;
		}

		private void ResetCachedColumns()
		{
			this.hitOffPageBlob = false;
			this.restrictedColumnValues.Reset();
			this.fetchColumnValues.Reset();
			if (this.columnCache != null)
			{
				this.columnCache.Clear();
			}
			this.rowPropertyBag = null;
		}

		private bool TryOpenJetCursorIfNecessary(StartStopKey key)
		{
			if (base.Table.IsPartitioned)
			{
				JetPartitionHelper.CheckPartitionKeys(base.Table, base.Table.PrimaryKeyIndex, key, key);
			}
			if (!this.jetCursorOpen)
			{
				JetConnection jetConnection = this.JetConnection;
				string tableName = base.Table.IsPartitioned ? JetPartitionHelper.GetPartitionName(base.Table, key.Values, base.Table.SpecialCols.NumberOfPartioningColumns) : base.Table.Name;
				try
				{
					if (base.Table.IsPartitioned)
					{
						if (jetConnection.TryOpenTable(base.Table, tableName, key.Values, Connection.OperationType.Query, out this.jetCursor))
						{
							this.jetCursorOpen = true;
						}
						this.partitionKey = key.Values[0];
					}
					else
					{
						this.jetCursor = jetConnection.GetOpenTable(base.Table, tableName, null, Connection.OperationType.Query);
						this.jetCursorOpen = true;
					}
					Api.JetSetCurrentIndex(jetConnection.JetSession, this.jetCursor, this.GetKeyIndex().Name);
					goto IL_14A;
				}
				catch (EsentErrorException ex)
				{
					jetConnection.OnExceptionCatch(ex);
					throw jetConnection.ProcessJetError((LID)43464U, "JetJoinOperator.TryOpenJetCursorIfNecessary", ex);
				}
			}
			if (base.Table.IsPartitioned && ValueHelper.ValuesCompare(this.partitionKey, key.Values[0]) != 0)
			{
				throw new ArgumentOutOfRangeException("key", "partition key has changed");
			}
			IL_14A:
			return this.jetCursorOpen;
		}

		private void CloseJetCursor()
		{
			JetConnection jetConnection = this.JetConnection;
			try
			{
				this.jetCursorOpen = false;
				using (jetConnection.TrackTimeInDatabase())
				{
					Api.JetCloseTable(jetConnection.JetSession, this.jetCursor);
				}
			}
			catch (EsentErrorException ex)
			{
				jetConnection.OnExceptionCatch(ex);
				throw jetConnection.ProcessJetError((LID)39368U, "JetJoinOperator.InternalDispose", ex);
			}
		}

		private Index GetKeyIndex()
		{
			if (this.keyIndex == null)
			{
				for (int i = 0; i < base.Table.Indexes.Count; i++)
				{
					bool flag = true;
					if (base.Table.Indexes[i].Columns.Count < base.KeyColumns.Count)
					{
						flag = false;
					}
					else
					{
						for (int j = 0; j < base.KeyColumns.Count; j++)
						{
							if (base.Table.Indexes[i].Columns[j].Name != base.KeyColumns[j].Name)
							{
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						this.keyIndex = base.Table.Indexes[i];
						break;
					}
				}
			}
			return this.keyIndex;
		}

		private bool CanUsePreRead()
		{
			return base.OuterQuery.MaxRows != 1;
		}

		private bool MoveFirstFromOuter(out int rowsSkipped)
		{
			if (this.interruptControl != null)
			{
				this.interrupted = false;
			}
			if (base.PreReadCacheSize <= 1)
			{
				bool flag = this.JetOuterQuery.MoveFirst(out rowsSkipped);
				if (flag)
				{
					if (this.interruptControl != null)
					{
						if (this.JetOuterQuery.Interrupted)
						{
							this.Interrupt();
							base.TraceCrumb("MoveFirst", "Interrupt");
							return true;
						}
						this.interruptControl.RegisterRead(true, base.Table.TableClass);
					}
					this.outerValues = this.GetValuesFromOuterWithoutPreReadCache();
				}
				return flag;
			}
			this.preReadIsEOF = false;
			this.joinValuesFromOuter = new Queue<KeyValuePair<object[], object[]>>(base.PreReadCacheSize + base.PreReadCacheSize / 3);
			this.prereadsToConsume = 0;
			if (!this.JetOuterQuery.MoveFirst(out rowsSkipped))
			{
				this.preReadIsEOF = true;
				return false;
			}
			if (this.interruptControl != null && this.JetOuterQuery.Interrupted)
			{
				this.Interrupt();
				base.TraceCrumb("MoveFirst", "Interrupt");
				return true;
			}
			this.PopulateOuterCache();
			this.outerValues = this.joinValuesFromOuter.Dequeue();
			this.prereadsToConsume--;
			return true;
		}

		private bool MoveNextFromOuter()
		{
			if (base.PreReadCacheSize <= 1 || (this.outerCanMoveBack && this.prereadsToConsume != 0 && this.joinValuesFromOuter.Count == 0))
			{
				bool flag = this.JetOuterQuery.MoveNext();
				if (flag)
				{
					if (this.interruptControl != null)
					{
						if (this.JetOuterQuery.Interrupted)
						{
							this.Interrupt();
							base.TraceCrumb("MoveNext", "Interrupt");
							return true;
						}
						this.interruptControl.RegisterRead(true, base.Table.TableClass);
					}
					this.outerValues = this.GetValuesFromOuterWithoutPreReadCache();
					if (this.prereadsToConsume > 0)
					{
						this.prereadsToConsume--;
					}
				}
				return flag;
			}
			bool flag2 = false;
			if (!this.preReadIsEOF && (this.interruptControl == null || !this.JetOuterQuery.Interrupted) && ((base.PreReadAhead && this.joinValuesFromOuter.Count < base.PreReadCacheSize / 3) || this.joinValuesFromOuter.Count == 0))
			{
				if (this.JetOuterQuery.MoveNext())
				{
					if (this.interruptControl == null || !this.JetOuterQuery.Interrupted)
					{
						flag2 = true;
						this.PopulateOuterCache();
					}
				}
				else
				{
					this.preReadIsEOF = true;
				}
			}
			if (this.interruptControl != null && this.outerCanMoveBack && (this.joinValuesFromOuter.Count != 0 || this.preReadIsEOF) && !flag2 && (this.JetOuterQuery.Interrupted || this.interruptControl.WantToInterrupt))
			{
				this.JetOuterQuery.MoveBackAndInterrupt(this.joinValuesFromOuter.Count);
				this.joinValuesFromOuter.Clear();
				this.preReadIsEOF = false;
			}
			if (this.joinValuesFromOuter.Count != 0)
			{
				this.outerValues = this.joinValuesFromOuter.Dequeue();
				this.prereadsToConsume--;
				return true;
			}
			if (this.interruptControl != null && this.JetOuterQuery.Interrupted)
			{
				this.Interrupt();
				base.TraceCrumb("MoveNext", "Interrupt");
				return true;
			}
			return false;
		}

		private void PopulateOuterCache()
		{
			using (base.Connection.TrackTimeInDatabase())
			{
				List<KeyRange> list = new List<KeyRange>(base.PreReadCacheSize);
				while (list.Count < base.PreReadCacheSize && !this.preReadIsEOF)
				{
					if (this.interruptControl != null)
					{
						this.interruptControl.RegisterRead(true, base.Table.TableClass);
					}
					KeyValuePair<object[], object[]> valuesFromOuterWithoutPreReadCache = this.GetValuesFromOuterWithoutPreReadCache();
					this.joinValuesFromOuter.Enqueue(valuesFromOuterWithoutPreReadCache);
					this.prereadsToConsume++;
					StartStopKey startStopKey = new StartStopKey(true, (IList<object>)valuesFromOuterWithoutPreReadCache.Key);
					list.Add(new KeyRange(startStopKey, startStopKey));
					if (list.Count < base.PreReadCacheSize)
					{
						if (this.JetOuterQuery.MoveNext())
						{
							if (this.interruptControl != null && this.JetOuterQuery.Interrupted)
							{
								break;
							}
						}
						else
						{
							this.preReadIsEOF = true;
						}
					}
				}
				if (list.Count > 1)
				{
					using (PreReadOperator preReadOperator = Factory.CreatePreReadOperator(base.Culture, base.Connection, base.Table, this.GetKeyIndex(), list, base.LongValueColumnsToPreread, true))
					{
						int num = (int)preReadOperator.ExecuteScalar();
						if (num < list.Count)
						{
							if (this.intermediateTracingEnabled)
							{
								ExTraceGlobals.DbInteractionIntermediateTracer.TraceDebug(0L, string.Format("Preread fewer keys than asked: {0}/{1}", list.Count, num));
							}
							if (base.PreReadCacheSize > 25)
							{
								base.PreReadCacheSize = Math.Max(num, Math.Max(25, base.PreReadCacheSize * 2 / 3));
							}
						}
					}
				}
			}
		}

		private KeyValuePair<object[], object[]> GetValuesFromOuterWithoutPreReadCache()
		{
			IList<Column> columnsToFetch = base.OuterQuery.ColumnsToFetch;
			object[] array = new object[base.KeyColumns.Count];
			int i;
			for (i = 0; i < base.KeyColumns.Count; i++)
			{
				array[i] = this.JetOuterQuery.GetColumnValue(columnsToFetch[i]);
			}
			object[] array2 = null;
			if (i < columnsToFetch.Count)
			{
				array2 = new object[columnsToFetch.Count - base.KeyColumns.Count];
				while (i < columnsToFetch.Count)
				{
					array2[i - base.KeyColumns.Count] = this.JetOuterQuery.GetColumnValue(columnsToFetch[i]);
					i++;
				}
			}
			return new KeyValuePair<object[], object[]>(array, array2);
		}

		private object GetOuterValue(int outerColumnIndex)
		{
			if (outerColumnIndex >= base.KeyColumns.Count)
			{
				return this.outerValues.Value[outerColumnIndex - base.KeyColumns.Count];
			}
			return this.outerValues.Key[outerColumnIndex];
		}

		public override bool Interrupted
		{
			get
			{
				return this.interrupted;
			}
		}

		void IJetSimpleQueryOperator.RequestResume()
		{
			base.TraceCrumb("RequestResume", "Resume");
			this.Resume();
		}

		bool IJetSimpleQueryOperator.CanMoveBack
		{
			get
			{
				return false;
			}
		}

		void IJetSimpleQueryOperator.MoveBackAndInterrupt(int rows)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "This method should never be called.");
		}

		private void Interrupt()
		{
			this.interrupted = true;
			if (this.jetCursorOpen)
			{
				this.CloseJetCursor();
			}
		}

		private bool Resume()
		{
			this.interrupted = false;
			this.JetOuterQuery.RequestResume();
			return true;
		}

		private CachedColumnValues restrictedColumnValues;

		private CachedColumnValues fetchColumnValues;

		private KeyValuePair<object[], object[]> outerValues;

		private Dictionary<PhysicalColumn, object> columnCache;

		private bool hitOffPageBlob;

		private Dictionary<Column, int> outerColumnsToFetch;

		private JET_TABLEID jetCursor;

		private IInterruptControl interruptControl;

		private bool interrupted;

		private bool outerCanMoveBack;

		private bool jetCursorOpen;

		private uint rowsReturned;

		private uint rowsRead;

		private IRowPropertyBag rowPropertyBag;

		private Index keyIndex;

		private object partitionKey;

		private Queue<KeyValuePair<object[], object[]>> joinValuesFromOuter;

		private int prereadsToConsume;

		private bool preReadIsEOF;
	}
}
