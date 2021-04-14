using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetSortOperator : SortOperator, IJetSimpleQueryOperator, IJetRecordCounter, ITWIR
	{
		internal JetSortOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, int skipTo, int maxRows, SortOrder sortOrder, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation) : this(connectionProvider, new SortOperator.SortOperatorDefinition(culture, queryOperator.OperatorDefinition, skipTo, maxRows, sortOrder, keyRanges, backwards, frequentOperation))
		{
		}

		internal JetSortOperator(IConnectionProvider connectionProvider, SortOperator.SortOperatorDefinition definition) : base(connectionProvider, definition)
		{
			this.distinctSortOrder = this.GetDistinctSortOrder();
			this.distinctColumnsToFetch = this.GetDistinctColumnsToFetch();
		}

		private IJetSimpleQueryOperator JetQueryOperator
		{
			get
			{
				return (IJetSimpleQueryOperator)base.QueryOperator;
			}
		}

		private JetConnection JetConnection
		{
			get
			{
				return (JetConnection)base.Connection;
			}
		}

		public IList<Column> DistinctColumnsToFetch
		{
			get
			{
				return this.distinctColumnsToFetch;
			}
		}

		public SortOrder DistinctSortOrder
		{
			get
			{
				return this.distinctSortOrder;
			}
		}

		public override Reader ExecuteReader(bool disposeQueryOperator)
		{
			base.TraceOperation("ExecuteReader");
			base.Connection.CountStatement(Connection.OperationType.Query);
			return new JetReader(base.ConnectionProvider, this, disposeQueryOperator);
		}

		public bool MoveFirst(out int rowsSkipped)
		{
			rowsSkipped = 0;
			this.rowsReturned = 0U;
			this.keyRangeIndex = 0;
			if (base.KeyRanges.Count == 0)
			{
				base.TraceMove("MoveFirst", false);
				return false;
			}
			if (!this.tempTablePopulated)
			{
				using (base.Connection.TrackTimeInDatabase())
				{
					this.PopulateTempTable();
					this.tempTablePopulated = true;
				}
			}
			JetConnection jetConnection = this.JetConnection;
			bool result;
			try
			{
				bool flag = false;
				using (base.Connection.TrackTimeInDatabase())
				{
					while (!flag && this.keyRangeIndex < base.KeyRanges.Count)
					{
						StartStopKey startKey = base.KeyRanges[this.keyRangeIndex].StartKey;
						if (!startKey.IsEmpty)
						{
							flag = this.FetchFromKey();
						}
						else if (base.Backwards)
						{
							flag = Api.TryMoveLast(jetConnection.JetSession, this.jetCursor);
						}
						else
						{
							flag = Api.TryMoveFirst(jetConnection.JetSession, this.jetCursor);
						}
						if (flag)
						{
							StartStopKey stopKey = base.KeyRanges[this.keyRangeIndex].StopKey;
							if (!stopKey.IsEmpty)
							{
								flag = this.SetKeyRange();
							}
						}
						if (!flag)
						{
							this.keyRangeIndex++;
						}
					}
				}
				if (!flag)
				{
					base.TraceMove("MoveFirst", false);
					result = false;
				}
				else
				{
					jetConnection.IncrementRowStatsCounter(null, RowStatsCounterType.Read);
					this.rowsRead += 1U;
					int num = base.SkipTo;
					if (base.Criteria != null)
					{
						bool flag2 = base.Criteria.Evaluate(this, base.CompareInfo);
						bool? flag3 = new bool?(true);
						if (!flag2 || flag3 == null)
						{
							goto IL_1E1;
						}
					}
					if (num <= 0)
					{
						base.TraceMove("MoveFirst", true);
						jetConnection.IncrementRowStatsCounter(null, RowStatsCounterType.Accept);
						this.rowsReturned += 1U;
						return true;
					}
					rowsSkipped++;
					num--;
					base.TraceMove("Skip", true);
					IL_1E1:
					result = this.MoveNext("MoveFirst", num, ref rowsSkipped);
				}
			}
			catch (EsentErrorException ex)
			{
				jetConnection.OnExceptionCatch(ex);
				throw jetConnection.ProcessJetError((LID)51656U, "JetSortOperator.MoveFirst", ex);
			}
			return result;
		}

		public bool MoveNext()
		{
			int num = 0;
			return this.MoveNext("MoveNext", 0, ref num);
		}

		private SortOrder GetDistinctSortOrder()
		{
			SortOrderBuilder sortOrderBuilder = new SortOrderBuilder(base.SortOrder.Columns.Count);
			for (int i = 0; i < base.SortOrder.Columns.Count; i++)
			{
				if (!sortOrderBuilder.Contains(base.SortOrder.Columns[i]))
				{
					sortOrderBuilder.Add(base.SortOrder.Columns[i], base.SortOrder.Ascending[i]);
				}
			}
			return sortOrderBuilder.ToSortOrder();
		}

		private IList<Column> GetDistinctColumnsToFetch()
		{
			IList<Column> list = new List<Column>(base.ColumnsToFetch.Count);
			for (int i = 0; i < this.DistinctSortOrder.Count; i++)
			{
				for (int j = 0; j < base.ColumnsToFetch.Count; j++)
				{
					if (this.DistinctSortOrder.Columns[i] == base.ColumnsToFetch[j])
					{
						list.Add(base.ColumnsToFetch[j]);
						break;
					}
				}
			}
			for (int k = 0; k < base.ColumnsToFetch.Count; k++)
			{
				Column item = base.ColumnsToFetch[k];
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		private byte[] GetColumnValueAsBytes(Column column)
		{
			int colNumberInTemp = this.GetColNumberInTemp(column);
			JetConnection jetConnection = this.JetConnection;
			byte[] array = null;
			try
			{
				using (jetConnection.TrackTimeInDatabase())
				{
					array = Api.RetrieveColumn(jetConnection.JetSession, this.jetCursor, this.tempTableDefinition.prgcolumnid[colNumberInTemp]);
					jetConnection.AddRowStatsCounter(null, RowStatsCounterType.ReadBytes, (array == null) ? 0 : array.Length);
				}
			}
			catch (EsentErrorException ex)
			{
				jetConnection.OnExceptionCatch(ex);
				throw jetConnection.ProcessJetError((LID)45512U, "JetSortOperator.GetColumn", ex);
			}
			return array;
		}

		private long? GetPhysicalColumnValueAsBytes(Column column, ArraySegment<byte> buffer)
		{
			int colNumberInTemp = this.GetColNumberInTemp(column);
			long? physicalColumnValueAsBytes = JetRetrieveColumnHelper.GetPhysicalColumnValueAsBytes(this.JetConnection, this.jetCursor, this.tempTableDefinition.prgcolumnid[colNumberInTemp], buffer);
			base.Connection.AddRowStatsCounter(null, RowStatsCounterType.ReadBytes, (int)physicalColumnValueAsBytes.GetValueOrDefault(0L));
			return physicalColumnValueAsBytes;
		}

		private object GetColumnValue(Column column)
		{
			if (JetColumnValueHelper.CanReuseColumnValueBuffer(column))
			{
				if (this.reusableValueBuffer == null)
				{
					this.reusableValueBuffer = JetColumnValueHelper.AllocateColumnValueBuffer();
				}
				byte[] columnValueAsBytes = this.reusableValueBuffer;
				long? physicalColumnValueAsBytes = this.GetPhysicalColumnValueAsBytes(column, new ArraySegment<byte>(columnValueAsBytes));
				if (physicalColumnValueAsBytes != null && physicalColumnValueAsBytes.Value > 0L)
				{
					return JetColumnValueHelper.GetAsObject(new ArraySegment<byte>(columnValueAsBytes, 0, (int)physicalColumnValueAsBytes.Value), column);
				}
			}
			else
			{
				byte[] columnValueAsBytes = this.GetColumnValueAsBytes(column);
				if (columnValueAsBytes != null)
				{
					return JetColumnValueHelper.GetAsObject(new ArraySegment<byte>(columnValueAsBytes), column);
				}
			}
			return null;
		}

		byte[] IJetSimpleQueryOperator.GetColumnValueAsBytes(Column column)
		{
			return this.GetColumnValueAsBytes(column);
		}

		byte[] IJetSimpleQueryOperator.GetPhysicalColumnValueAsBytes(PhysicalColumn column)
		{
			throw new InvalidOperationException("This method should not be called in a Sort operator");
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
			throw new InvalidOperationException("This method should not be called in a Sort operator");
		}

		object ITWIR.GetColumnValue(Column column)
		{
			return this.GetColumnValue(column);
		}

		int ITWIR.GetPhysicalColumnSize(PhysicalColumn column)
		{
			throw new InvalidOperationException("This method should not be called in a Sort operator");
		}

		object ITWIR.GetPhysicalColumnValue(PhysicalColumn column)
		{
			throw new InvalidOperationException("This method should not be called in a Sort operator");
		}

		int ITWIR.GetPropertyColumnSize(PropertyColumn column)
		{
			throw new InvalidOperationException("This method should not be called in a Sort operator");
		}

		object ITWIR.GetPropertyColumnValue(PropertyColumn column)
		{
			throw new InvalidOperationException("This method should not be called in a Sort operator");
		}

		internal bool MoveNext(string operation, int numberLeftToSkip, ref int rowsSkipped)
		{
			JetConnection jetConnection = this.JetConnection;
			bool result;
			try
			{
				using (base.Connection.TrackTimeInDatabase())
				{
					while (base.MaxRows <= 0 || (ulong)this.rowsReturned < (ulong)((long)base.MaxRows))
					{
						bool flag;
						if (base.Backwards)
						{
							flag = Api.TryMovePrevious(jetConnection.JetSession, this.jetCursor);
						}
						else
						{
							flag = Api.TryMoveNext(jetConnection.JetSession, this.jetCursor);
						}
						while (!flag && this.keyRangeIndex < base.KeyRanges.Count)
						{
							this.keyRangeIndex++;
							if (this.keyRangeIndex < base.KeyRanges.Count)
							{
								flag = this.FetchFromKey();
								if (flag)
								{
									StartStopKey stopKey = base.KeyRanges[this.keyRangeIndex].StopKey;
									if (!stopKey.IsEmpty)
									{
										flag = this.SetKeyRange();
									}
								}
							}
						}
						if (!flag)
						{
							base.TraceMove(operation, false);
							return false;
						}
						jetConnection.IncrementRowStatsCounter(null, RowStatsCounterType.Read);
						this.rowsRead += 1U;
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
							base.TraceMove(operation, true);
							base.Connection.IncrementRowStatsCounter(null, RowStatsCounterType.Accept);
							this.rowsReturned += 1U;
							return true;
						}
						rowsSkipped++;
						numberLeftToSkip--;
						base.TraceMove("Skip", true);
					}
					base.TraceMove(operation, false);
					result = false;
				}
			}
			catch (EsentErrorException ex)
			{
				jetConnection.OnExceptionCatch(ex);
				throw jetConnection.ProcessJetError((LID)61896U, "JetSortOperator.MoveNext", ex);
			}
			return result;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JetSortOperator>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				try
				{
					if (this.jetCursorOpen)
					{
						JetConnection jetConnection = this.JetConnection;
						try
						{
							using (jetConnection.TrackTimeInDatabase())
							{
								Api.JetCloseTable(jetConnection.JetSession, this.jetCursor);
							}
						}
						catch (EsentErrorException ex)
						{
							jetConnection.OnExceptionCatch(ex);
							throw jetConnection.ProcessJetError((LID)37320U, "JetSortOperator.InternalDisplse", ex);
						}
						this.jetCursorOpen = false;
						if (this.detailTracingEnabled)
						{
							StringBuilder stringBuilder = new StringBuilder(200);
							base.AppendOperationInfo("Dispose", stringBuilder);
							stringBuilder.Append("  rowsRead:[");
							stringBuilder.Append(this.rowsRead);
							stringBuilder.Append("]");
							ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, stringBuilder.ToString());
							this.rowsRead = 0U;
						}
					}
				}
				finally
				{
					base.InternalDispose(calledFromDispose);
				}
			}
		}

		private bool FetchFromKey()
		{
			JetConnection jetConnection = this.JetConnection;
			bool result = false;
			try
			{
				StartStopKey startKey = base.KeyRanges[this.keyRangeIndex].StartKey;
				bool flag = startKey.Count < this.DistinctSortOrder.Count;
				int num = 0;
				for (;;)
				{
					int num2 = num;
					StartStopKey startKey2 = base.KeyRanges[this.keyRangeIndex].StartKey;
					if (num2 >= startKey2.Count)
					{
						break;
					}
					MakeKeyGrbit makeKeyGrbit = (num == 0) ? MakeKeyGrbit.NewKey : MakeKeyGrbit.None;
					if (flag)
					{
						int num3 = num;
						StartStopKey startKey3 = base.KeyRanges[this.keyRangeIndex].StartKey;
						if (num3 == startKey3.Count - 1)
						{
							StartStopKey startKey4 = base.KeyRanges[this.keyRangeIndex].StartKey;
							if (startKey4.Inclusive)
							{
								if (base.Backwards)
								{
									makeKeyGrbit |= MakeKeyGrbit.FullColumnEndLimit;
								}
								else
								{
									makeKeyGrbit |= MakeKeyGrbit.FullColumnStartLimit;
								}
							}
							else if (base.Backwards)
							{
								makeKeyGrbit |= MakeKeyGrbit.FullColumnStartLimit;
							}
							else
							{
								makeKeyGrbit |= MakeKeyGrbit.FullColumnEndLimit;
							}
						}
					}
					JET_SESID jetSession = jetConnection.JetSession;
					JET_TABLEID tableId = this.jetCursor;
					MakeKeyGrbit grbit = makeKeyGrbit;
					StartStopKey startKey5 = base.KeyRanges[this.keyRangeIndex].StartKey;
					JetColumnValueHelper.MakeJetKeyFromValue(jetSession, tableId, grbit, startKey5.Values[num], this.DistinctSortOrder[num].Column);
					num++;
				}
				StartStopKey startKey6 = base.KeyRanges[this.keyRangeIndex].StartKey;
				SeekGrbit grbit2;
				if (startKey6.Inclusive)
				{
					StartStopKey stopKey = base.KeyRanges[this.keyRangeIndex].StopKey;
					if (stopKey.Inclusive && StartStopKey.CommonKeyPrefix(base.KeyRanges[this.keyRangeIndex].StartKey, base.KeyRanges[this.keyRangeIndex].StopKey, base.CompareInfo) == this.DistinctSortOrder.Count)
					{
						grbit2 = SeekGrbit.SeekEQ;
						goto IL_235;
					}
				}
				if (!base.Backwards)
				{
					StartStopKey startKey7 = base.KeyRanges[this.keyRangeIndex].StartKey;
					grbit2 = (startKey7.Inclusive ? SeekGrbit.SeekGE : SeekGrbit.SeekGT);
				}
				else
				{
					StartStopKey startKey8 = base.KeyRanges[this.keyRangeIndex].StartKey;
					grbit2 = (startKey8.Inclusive ? SeekGrbit.SeekLE : SeekGrbit.SeekLT);
				}
				IL_235:
				jetConnection.IncrementRowStatsCounter(null, RowStatsCounterType.Seek);
				result = Api.TrySeek(jetConnection.JetSession, this.jetCursor, grbit2);
			}
			catch (EsentErrorException ex)
			{
				jetConnection.OnExceptionCatch(ex);
				throw jetConnection.ProcessJetError((LID)53704U, "JetSortOperator.FetchFromKey", ex);
			}
			return result;
		}

		private bool SetKeyRange()
		{
			JetConnection jetConnection = this.JetConnection;
			bool result = false;
			try
			{
				StartStopKey stopKey = base.KeyRanges[this.keyRangeIndex].StopKey;
				bool flag = stopKey.Count < this.DistinctSortOrder.Count;
				int num = 0;
				for (;;)
				{
					int num2 = num;
					StartStopKey stopKey2 = base.KeyRanges[this.keyRangeIndex].StopKey;
					if (num2 >= stopKey2.Count)
					{
						break;
					}
					MakeKeyGrbit makeKeyGrbit = (num == 0) ? MakeKeyGrbit.NewKey : MakeKeyGrbit.None;
					if (flag)
					{
						int num3 = num;
						StartStopKey stopKey3 = base.KeyRanges[this.keyRangeIndex].StopKey;
						if (num3 == stopKey3.Count - 1)
						{
							StartStopKey stopKey4 = base.KeyRanges[this.keyRangeIndex].StopKey;
							if (stopKey4.Inclusive)
							{
								if (base.Backwards)
								{
									makeKeyGrbit |= MakeKeyGrbit.FullColumnStartLimit;
								}
								else
								{
									makeKeyGrbit |= MakeKeyGrbit.FullColumnEndLimit;
								}
							}
							else if (base.Backwards)
							{
								makeKeyGrbit |= MakeKeyGrbit.FullColumnEndLimit;
							}
							else
							{
								makeKeyGrbit |= MakeKeyGrbit.FullColumnStartLimit;
							}
						}
					}
					JET_SESID jetSession = this.JetConnection.JetSession;
					JET_TABLEID tableId = this.jetCursor;
					MakeKeyGrbit grbit = makeKeyGrbit;
					StartStopKey stopKey5 = base.KeyRanges[this.keyRangeIndex].StopKey;
					JetColumnValueHelper.MakeJetKeyFromValue(jetSession, tableId, grbit, stopKey5.Values[num], this.DistinctSortOrder[num].Column);
					num++;
				}
				SetIndexRangeGrbit setIndexRangeGrbit = SetIndexRangeGrbit.None;
				if (!base.Backwards)
				{
					setIndexRangeGrbit = SetIndexRangeGrbit.RangeUpperLimit;
				}
				StartStopKey stopKey6 = base.KeyRanges[this.keyRangeIndex].StopKey;
				if (stopKey6.Inclusive)
				{
					setIndexRangeGrbit |= SetIndexRangeGrbit.RangeInclusive;
				}
				result = Api.TrySetIndexRange(jetConnection.JetSession, this.jetCursor, setIndexRangeGrbit);
			}
			catch (EsentErrorException ex)
			{
				jetConnection.OnExceptionCatch(ex);
				throw jetConnection.ProcessJetError((LID)41416U, "JetSortOperator.SetkeyRange", ex);
			}
			return result;
		}

		private JET_TABLEID CreateTempTable()
		{
			this.tempTableDefinition = new JET_OPENTEMPORARYTABLE();
			this.tempTableDefinition.pidxunicode = new JET_UNICODEINDEX();
			this.tempTableDefinition.pidxunicode.dwMapFlags = JetTable.LCMapFlagsFromCompareOptions(CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
			this.tempTableDefinition.pidxunicode.szLocaleName = base.Culture.CompareInfo.Name;
			this.tempTableDefinition.grbit = (TempTableGrbit)257;
			this.tempTableDefinition.prgcolumndef = new JET_COLUMNDEF[this.DistinctColumnsToFetch.Count];
			this.tempTableDefinition.ccolumn = this.tempTableDefinition.prgcolumndef.Length;
			this.tempTableDefinition.prgcolumnid = new JET_COLUMNID[this.tempTableDefinition.ccolumn];
			int num = 0;
			int num2 = 0;
			int i = 0;
			while (i < this.DistinctColumnsToFetch.Count)
			{
				Column column = this.DistinctColumnsToFetch[i];
				JET_COLUMNDEF jet_COLUMNDEF = new JET_COLUMNDEF();
				if (column.MaxLength > 0)
				{
					jet_COLUMNDEF.cbMax = column.MaxLength;
				}
				else
				{
					jet_COLUMNDEF.cbMax = column.Size;
				}
				switch (column.ExtendedTypeCode)
				{
				case ExtendedTypeCode.Boolean:
					jet_COLUMNDEF.coltyp = JET_coltyp.Bit;
					break;
				case ExtendedTypeCode.Int16:
					jet_COLUMNDEF.coltyp = JET_coltyp.Short;
					break;
				case ExtendedTypeCode.Int32:
					jet_COLUMNDEF.coltyp = JET_coltyp.Long;
					break;
				case ExtendedTypeCode.Int64:
					jet_COLUMNDEF.coltyp = (JET_coltyp)15;
					break;
				case ExtendedTypeCode.Single:
					jet_COLUMNDEF.coltyp = JET_coltyp.IEEESingle;
					break;
				case ExtendedTypeCode.Double:
					jet_COLUMNDEF.coltyp = JET_coltyp.IEEEDouble;
					break;
				case ExtendedTypeCode.DateTime:
					jet_COLUMNDEF.coltyp = JET_coltyp.DateTime;
					break;
				case ExtendedTypeCode.Guid:
					jet_COLUMNDEF.coltyp = (JET_coltyp)16;
					break;
				case ExtendedTypeCode.String:
					jet_COLUMNDEF.cp = JET_CP.Unicode;
					if (jet_COLUMNDEF.cbMax <= 127)
					{
						jet_COLUMNDEF.coltyp = JET_coltyp.Text;
					}
					else
					{
						jet_COLUMNDEF.coltyp = JET_coltyp.LongText;
					}
					jet_COLUMNDEF.cbMax *= 2;
					break;
				case ExtendedTypeCode.Binary:
					if (jet_COLUMNDEF.cbMax <= 255)
					{
						jet_COLUMNDEF.coltyp = JET_coltyp.Binary;
					}
					else
					{
						jet_COLUMNDEF.coltyp = JET_coltyp.LongBinary;
					}
					break;
				case (ExtendedTypeCode)11:
				case (ExtendedTypeCode)12:
				case (ExtendedTypeCode)13:
				case (ExtendedTypeCode)14:
				case (ExtendedTypeCode)15:
				case ExtendedTypeCode.MVFlag:
				case (ExtendedTypeCode)17:
					goto IL_247;
				case ExtendedTypeCode.MVInt16:
				case ExtendedTypeCode.MVInt32:
				case ExtendedTypeCode.MVInt64:
				case ExtendedTypeCode.MVSingle:
				case ExtendedTypeCode.MVDouble:
				case ExtendedTypeCode.MVDateTime:
				case ExtendedTypeCode.MVGuid:
				case ExtendedTypeCode.MVString:
				case ExtendedTypeCode.MVBinary:
					jet_COLUMNDEF.coltyp = JET_coltyp.LongBinary;
					break;
				default:
					goto IL_247;
				}
				if (!column.IsNullable)
				{
					jet_COLUMNDEF.grbit |= ColumndefGrbit.ColumnNotNULL;
				}
				if (i < this.DistinctSortOrder.Count)
				{
					jet_COLUMNDEF.grbit |= ColumndefGrbit.TTKey;
					if (!this.DistinctSortOrder[i].Ascending)
					{
						jet_COLUMNDEF.grbit |= ColumndefGrbit.TTDescending;
					}
					if (jet_COLUMNDEF.coltyp == JET_coltyp.Text || jet_COLUMNDEF.coltyp == JET_coltyp.LongText || jet_COLUMNDEF.coltyp == JET_coltyp.Binary || jet_COLUMNDEF.coltyp == JET_coltyp.LongBinary)
					{
						num++;
					}
					else
					{
						num2 += 1 + column.Size;
					}
				}
				this.tempTableDefinition.prgcolumndef[i] = jet_COLUMNDEF;
				i++;
				continue;
				IL_247:
				throw new InvalidOperationException(string.Format("Unknown or unexpected extended type code {0} for a column {1} having type {2}", column.ExtendedTypeCode, column, column.Type));
			}
			if (num > 0)
			{
				this.tempTableDefinition.cbVarSegMac = (2000 - num2) / num;
				this.tempTableDefinition.cbKeyMost = 2000;
			}
			return this.JetConnection.GetTempTable(ref this.tempTableDefinition);
		}

		private int GetColNumberInTemp(Column col)
		{
			for (int i = 0; i < this.DistinctColumnsToFetch.Count; i++)
			{
				if (this.DistinctColumnsToFetch[i] == col)
				{
					return i;
				}
			}
			return -1;
		}

		private void CreateAndOpenTempTable()
		{
			if (!this.jetCursorOpen)
			{
				JetConnection jetConnection = this.JetConnection;
				try
				{
					this.jetCursor = this.CreateTempTable();
					this.jetCursorOpen = true;
				}
				catch (EsentErrorException ex)
				{
					jetConnection.OnExceptionCatch(ex);
					throw jetConnection.ProcessJetError((LID)57800U, "JetSortOperator.Constructor", ex);
				}
			}
		}

		private void PopulateTempTable()
		{
			this.CreateAndOpenTempTable();
			JetConnection jetConnection = this.JetConnection;
			jetConnection.CountStatement(Connection.OperationType.Query);
			jetConnection.CountStatement(Connection.OperationType.Insert);
			bool flag = false;
			if (!jetConnection.TransactionStarted)
			{
				try
				{
					Api.JetBeginTransaction(jetConnection.JetSession);
					flag = true;
				}
				catch (EsentErrorException ex)
				{
					jetConnection.OnExceptionCatch(ex);
					throw jetConnection.ProcessJetError((LID)47864U, "PopulateTempTable", ex);
				}
			}
			try
			{
				int num;
				bool flag2 = this.JetQueryOperator.MoveFirst(out num);
				while (flag2)
				{
					jetConnection.IncrementRowStatsCounter(null, RowStatsCounterType.Write);
					try
					{
						Api.JetPrepareUpdate(jetConnection.JetSession, this.jetCursor, JET_prep.Insert);
						JET_SETCOLUMN[] array = new JET_SETCOLUMN[base.QueryOperator.ColumnsToFetch.Count];
						for (int i = 0; i < base.QueryOperator.ColumnsToFetch.Count; i++)
						{
							Column column = base.QueryOperator.ColumnsToFetch[i];
							int colNumberInTemp = this.GetColNumberInTemp(column);
							byte[] columnValueAsBytes = this.JetQueryOperator.GetColumnValueAsBytes(column);
							int num2 = (columnValueAsBytes == null) ? 0 : columnValueAsBytes.Length;
							jetConnection.AddRowStatsCounter(null, RowStatsCounterType.WriteBytes, num2);
							array[i] = new JET_SETCOLUMN();
							array[i].columnid = this.tempTableDefinition.prgcolumnid[colNumberInTemp];
							array[i].pvData = columnValueAsBytes;
							array[i].cbData = num2;
							array[i].itagSequence = 1;
							if (columnValueAsBytes != null && num2 == 0)
							{
								array[i].grbit = SetColumnGrbit.ZeroLength;
							}
						}
						Api.JetSetColumns(jetConnection.JetSession, this.jetCursor, array, array.Length);
						Api.JetUpdate(jetConnection.JetSession, this.jetCursor);
					}
					catch (EsentErrorException ex2)
					{
						jetConnection.OnExceptionCatch(ex2);
						throw jetConnection.ProcessJetError((LID)33224U, "JetSortOperator.PopulateTempTable", ex2);
					}
					flag2 = this.JetQueryOperator.MoveNext();
				}
			}
			finally
			{
				if (flag)
				{
					try
					{
						Api.JetCommitTransaction(jetConnection.JetSession, CommitTransactionGrbit.LazyFlush);
					}
					catch (EsentErrorException ex3)
					{
						jetConnection.OnExceptionCatch(ex3);
						throw jetConnection.ProcessJetError((LID)32839U, "PopulateTempTable", ex3);
					}
				}
			}
		}

		public override bool Interrupted
		{
			get
			{
				return false;
			}
		}

		void IJetSimpleQueryOperator.RequestResume()
		{
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

		private const string ThisMethodShouldNotBeCalled = "This method should not be called in a Sort operator";

		private readonly IList<Column> distinctColumnsToFetch;

		private readonly SortOrder distinctSortOrder;

		private JET_TABLEID jetCursor;

		private JET_OPENTEMPORARYTABLE tempTableDefinition;

		private bool tempTablePopulated;

		private bool jetCursorOpen;

		private uint rowsReturned;

		private uint rowsRead;

		private byte[] reusableValueBuffer;

		private int keyRangeIndex;
	}
}
