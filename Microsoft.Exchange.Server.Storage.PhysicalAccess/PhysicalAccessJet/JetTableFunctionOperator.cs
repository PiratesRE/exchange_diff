using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetTableFunctionOperator : TableFunctionOperator, IJetSimpleQueryOperator, IJetRecordCounter, ITWIR
	{
		internal JetTableFunctionOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableFunction tableFunction, object[] parameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation) : this(connectionProvider, new TableFunctionOperator.TableFunctionOperatorDefinition(culture, tableFunction, parameters, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, keyRanges, backwards, frequentOperation))
		{
		}

		internal JetTableFunctionOperator(IConnectionProvider connectionProvider, TableFunctionOperator.TableFunctionOperatorDefinition definition) : base(connectionProvider, definition)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				for (int i = 0; i < base.KeyRanges.Count; i++)
				{
					StartStopKey startKey = base.KeyRanges[i].StartKey;
					if (!startKey.IsEmpty)
					{
						JetTableFunctionOperator.CheckKeyColumnsMatchIndexColumns(base.KeyRanges[i].StartKey, base.Table.PrimaryKeyIndex);
					}
					StartStopKey stopKey = base.KeyRanges[i].StopKey;
					if (!stopKey.IsEmpty)
					{
						JetTableFunctionOperator.CheckKeyColumnsMatchIndexColumns(base.KeyRanges[i].StopKey, base.Table.PrimaryKeyIndex);
					}
				}
				disposeGuard.Success();
			}
		}

		public override uint RowsReturned
		{
			get
			{
				return this.rowsReturned;
			}
		}

		public override Reader ExecuteReader(bool disposeQueryOperator)
		{
			base.TraceOperation("ExecuteReader");
			base.Connection.CountStatement(Connection.OperationType.Query);
			return new JetReader(base.ConnectionProvider, this, disposeQueryOperator);
		}

		public override bool EnableInterrupts(IInterruptControl interruptControl)
		{
			if (interruptControl != null && base.SkipTo != 0)
			{
				return false;
			}
			this.interruptControl = interruptControl;
			return true;
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
			if (base.Criteria is SearchCriteriaFalse)
			{
				base.TraceMove("MoveFirst", false);
				return false;
			}
			if (this.interruptControl != null)
			{
				this.interrupted = false;
			}
			this.tableContents = (IEnumerable)base.TableFunction.GetTableContents(base.ConnectionProvider, base.Parameters);
			if (this.tableContents == null)
			{
				base.TraceMove("MoveFirst", false);
				return false;
			}
			IConfigurableTableContents configurableTableContents = this.tableContents as IConfigurableTableContents;
			if (configurableTableContents != null)
			{
				configurableTableContents.Configure(base.Backwards, base.KeyRanges[0].StartKey);
			}
			this.enumerator = this.tableContents.GetEnumerator();
			if (base.Backwards && configurableTableContents == null)
			{
				Stack<object> stack = new Stack<object>();
				while (this.enumerator.MoveNext())
				{
					stack.Push(this.enumerator.Current);
				}
				this.enumerator = ((IEnumerable)stack).GetEnumerator();
			}
			while (this.enumerator != null && this.enumerator.MoveNext())
			{
				if (this.CursorIsAfterAllKeyRanges())
				{
					base.TraceMove("MoveFirst", false);
					return false;
				}
				if (this.CursorIsInValidKeyRange())
				{
					base.Connection.IncrementRowStatsCounter(base.Table, RowStatsCounterType.Read);
					int num = base.SkipTo;
					if (base.Criteria == null || base.Criteria.Evaluate(this, base.CompareInfo))
					{
						if (num <= 0)
						{
							base.TraceMove("MoveFirst", true);
							base.Connection.IncrementRowStatsCounter(base.Table, RowStatsCounterType.Accept);
							this.rowsReturned += 1U;
							return true;
						}
						rowsSkipped++;
						num--;
					}
					return this.MoveNext("MoveFirst", num, ref rowsSkipped);
				}
			}
			IRefillableTableContents refillableTableContents = this.tableContents as IRefillableTableContents;
			if (refillableTableContents != null)
			{
				refillableTableContents.MarkChunkConsumed();
				if (refillableTableContents.CanRefill)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.interruptControl != null, "We cannot refill if interrupts are disabled");
					this.Interrupt();
					this.enumerator = null;
					base.TraceCrumb("MoveFirst", "Interrupt to refill");
					return true;
				}
			}
			base.TraceMove("MoveFirst", false);
			return false;
		}

		public bool MoveNext()
		{
			int num = 0;
			return this.MoveNext("MoveNext", 0, ref num);
		}

		public int GetColumnSize(Column column)
		{
			return ((IColumn)column).GetSize(this);
		}

		public byte[] GetColumnValueAsBytes(Column column)
		{
			if (base.RenameDictionary != null)
			{
				column = base.ResolveColumn(column);
			}
			IJetColumn jetColumn = (IJetColumn)column;
			return jetColumn.GetValueAsBytes(this);
		}

		internal bool MoveNext(string operation, int numberLeftToSkip, ref int rowsSkipped)
		{
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
			if (base.MaxRows > 0 && (ulong)this.rowsReturned >= (ulong)((long)base.MaxRows))
			{
				base.TraceMove(operation, false);
				return false;
			}
			while (this.interruptControl == null || !this.interruptControl.WantToInterrupt)
			{
				if (!this.enumerator.MoveNext())
				{
					IRefillableTableContents refillableTableContents = this.tableContents as IRefillableTableContents;
					if (refillableTableContents != null)
					{
						refillableTableContents.MarkChunkConsumed();
						if (refillableTableContents.CanRefill)
						{
							Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.interruptControl != null, "We cannot refill if interrupts are disabled");
							this.Interrupt();
							this.enumerator = null;
							base.TraceCrumb(operation, "Interrupt to refill");
							return true;
						}
					}
					base.TraceMove(operation, false);
					return false;
				}
				if (this.CursorIsAfterAllKeyRanges())
				{
					base.TraceMove(operation, false);
					return false;
				}
				if (this.CursorIsInValidKeyRange())
				{
					base.Connection.IncrementRowStatsCounter(base.Table, RowStatsCounterType.Read);
					if (base.Criteria == null || base.Criteria.Evaluate(this, base.CompareInfo))
					{
						if (numberLeftToSkip <= 0)
						{
							base.TraceMove(operation, true);
							base.Connection.IncrementRowStatsCounter(base.Table, RowStatsCounterType.Accept);
							this.rowsReturned += 1U;
							return true;
						}
						rowsSkipped++;
						numberLeftToSkip--;
					}
				}
			}
			this.Interrupt();
			base.TraceCrumb(operation, "Interrupt");
			return true;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JetTableFunctionOperator>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
		}

		private static void CheckKeyColumnsMatchIndexColumns(StartStopKey key, Index index)
		{
			if (key.Count > index.ColumnCount)
			{
				throw new InvalidOperationException("Key has more columns than primary index");
			}
		}

		private static int AdjustKeyColumnCompareResults(bool isAscending, bool isBackwards, int compareResults)
		{
			if (!isAscending)
			{
				compareResults = -compareResults;
			}
			if (isBackwards)
			{
				compareResults = -compareResults;
			}
			return compareResults;
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

		object ITWIR.GetColumnValue(Column column)
		{
			return this.GetColumnValue(column);
		}

		int ITWIR.GetPhysicalColumnSize(PhysicalColumn column)
		{
			object physicalColumnValue = ((ITWIR)this).GetPhysicalColumnValue(column);
			return SizeOfColumn.GetColumnSize(column, physicalColumnValue).GetValueOrDefault();
		}

		object ITWIR.GetPhysicalColumnValue(PhysicalColumn column)
		{
			object obj = base.TableFunction.GetColumnFromRow(base.ConnectionProvider, this.enumerator.Current, column);
			base.Connection.AddRowStatsCounter(base.Table, RowStatsCounterType.ReadBytes, ValueTypeHelper.ValueSize(column.ExtendedTypeCode, obj));
			return obj;
		}

		int ITWIR.GetPropertyColumnSize(PropertyColumn column)
		{
			throw new InvalidOperationException("TableFunctions do not support property columns");
		}

		object ITWIR.GetPropertyColumnValue(PropertyColumn column)
		{
			throw new InvalidOperationException("TableFunctions do not support property columns");
		}

		private object GetColumnValue(Column column)
		{
			if (base.RenameDictionary != null)
			{
				column = base.ResolveColumn(column);
			}
			IColumn column2 = column;
			return column2.GetValue(this);
		}

		private byte[] GetPhysicalColumnValueAsBytes(PhysicalColumn column)
		{
			object obj = base.TableFunction.GetColumnFromRow(base.ConnectionProvider, this.enumerator.Current, column);
			base.Connection.AddRowStatsCounter(base.Table, RowStatsCounterType.ReadBytes, ValueTypeHelper.ValueSize(column.ExtendedTypeCode, obj));
			return JetColumnValueHelper.GetAsByteArray(obj, column);
		}

		private int CompareCurrentRecordToKey(StartStopKey key)
		{
			for (int i = 0; i < key.Count; i++)
			{
				Column column = base.SortOrder.Columns[i];
				object columnValue = this.GetColumnValue(column);
				int num = JetTableFunctionOperator.AdjustKeyColumnCompareResults(base.Table.PrimaryKeyIndex.Ascending[i], base.Backwards, ValueHelper.ValuesCompare(columnValue, key.Values[i], base.CompareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth));
				if (num != 0)
				{
					return num;
				}
			}
			return 0;
		}

		private bool CursorIsInValidKeyRange()
		{
			while (this.CursorIsAfterStartKey())
			{
				if (!this.CursorIsAfterStopKey())
				{
					return true;
				}
				this.keyRangeIndex++;
				if (this.keyRangeIndex >= base.KeyRanges.Count)
				{
					return false;
				}
			}
			return false;
		}

		private bool CursorIsAfterAllKeyRanges()
		{
			return this.keyRangeIndex >= base.KeyRanges.Count;
		}

		private bool CursorIsAfterStartKey()
		{
			StartStopKey startKey = base.KeyRanges[this.keyRangeIndex].StartKey;
			return startKey.IsEmpty || this.CursorIsAfterKey(base.KeyRanges[this.keyRangeIndex].StartKey, false);
		}

		private bool CursorIsAfterStopKey()
		{
			StartStopKey stopKey = base.KeyRanges[this.keyRangeIndex].StopKey;
			return !stopKey.IsEmpty && this.CursorIsAfterKey(base.KeyRanges[this.keyRangeIndex].StopKey, true);
		}

		private bool CursorIsAfterKey(StartStopKey key, bool isStopKey)
		{
			int num = this.CompareCurrentRecordToKey(key);
			return num >= 0 && (num > 0 || key.Inclusive != isStopKey);
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
		}

		private bool Resume()
		{
			this.interrupted = false;
			if (this.enumerator == null)
			{
				this.enumerator = this.tableContents.GetEnumerator();
			}
			return true;
		}

		private const string TableFunctionsDoNotSupportPropertyColumns = "TableFunctions do not support property columns";

		private IInterruptControl interruptControl;

		private bool interrupted;

		private uint rowsReturned;

		private IEnumerable tableContents;

		private IEnumerator enumerator;

		private int keyRangeIndex;
	}
}
