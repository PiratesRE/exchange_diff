using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetApplyOperator : ApplyOperator, IJetSimpleQueryOperator, IJetRecordCounter, ITWIR, IRowAccess
	{
		internal JetApplyOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableFunction tableFunction, IList<Column> tableFunctionParameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, SimpleQueryOperator outerQuery, bool frequentOperation) : this(connectionProvider, new ApplyOperator.ApplyOperatorDefinition(culture, tableFunction, tableFunctionParameters, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, outerQuery.OperatorDefinition, frequentOperation))
		{
		}

		internal JetApplyOperator(IConnectionProvider connectionProvider, ApplyOperator.ApplyOperatorDefinition definition) : base(connectionProvider, definition)
		{
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

		private JetTableFunction JetTableFunction
		{
			get
			{
				return (JetTableFunction)base.TableFunction;
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
			if (!base.OuterQuery.EnableInterrupts(interruptControl))
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
			if (base.Criteria is SearchCriteriaFalse)
			{
				base.TraceMove("MoveFirst", false);
				return false;
			}
			if (this.interruptControl != null)
			{
				this.interrupted = false;
			}
			int num;
			if (!this.JetOuterQuery.MoveFirst(out num))
			{
				base.TraceMove("MoveFirst", false);
				return false;
			}
			int num2 = base.SkipTo;
			if (this.interruptControl != null && this.JetOuterQuery.Interrupted)
			{
				this.Interrupt();
				base.TraceCrumb("MoveFirst", "Interrupt");
				return true;
			}
			this.tableFunctionContents = this.CreateInnerTableFunctionIterator();
			bool flag = this.tableFunctionContents.MoveNext();
			if (flag)
			{
				base.Connection.IncrementRowStatsCounter(base.Table, RowStatsCounterType.Read);
				if (base.Criteria != null)
				{
					bool flag2 = base.Criteria.Evaluate(this, base.CompareInfo);
					bool? flag3 = new bool?(true);
					if (!flag2 || flag3 == null)
					{
						goto IL_125;
					}
				}
				if (num2 <= 0)
				{
					base.TraceMove("MoveFirst", true);
					base.Connection.IncrementRowStatsCounter(base.Table, RowStatsCounterType.Accept);
					this.rowsReturned += 1U;
					return true;
				}
				rowsSkipped++;
				num2--;
			}
			IL_125:
			return this.MoveNext("MoveFirst", num2, ref rowsSkipped);
		}

		public bool MoveNext()
		{
			int num = 0;
			return this.MoveNext("MoveNext", 0, ref num);
		}

		private bool MoveNext(string operation, int numberLeftToSkip, ref int rowsSkipped)
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
			if (base.MaxRows > 0 && (ulong)this.rowsReturned >= (ulong)((long)base.MaxRows))
			{
				base.TraceMove(operation, false);
				return false;
			}
			using (base.Connection.TrackTimeInDatabase())
			{
				for (;;)
				{
					if (this.tableFunctionContents == null)
					{
						flag = this.JetOuterQuery.MoveNext();
						if (!flag)
						{
							goto IL_14B;
						}
						if (this.interruptControl != null && this.JetOuterQuery.Interrupted)
						{
							break;
						}
						this.tableFunctionContents = this.CreateInnerTableFunctionIterator();
					}
					flag = this.tableFunctionContents.MoveNext();
					if (!flag)
					{
						this.tableFunctionContents = null;
					}
					else
					{
						base.Connection.IncrementRowStatsCounter(base.Table, RowStatsCounterType.Read);
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
							goto IL_129;
						}
						rowsSkipped++;
						numberLeftToSkip--;
					}
				}
				this.Interrupt();
				base.TraceCrumb(operation, "Interrupt");
				return true;
				IL_129:
				base.Connection.IncrementRowStatsCounter(base.Table, RowStatsCounterType.Accept);
				flag = true;
				this.rowsReturned += 1U;
				IL_14B:;
			}
			base.TraceMove(operation, flag);
			return flag;
		}

		private IEnumerator CreateInnerTableFunctionIterator()
		{
			object[] array = new object[base.TableFunctionParameters.Count];
			for (int i = 0; i < base.TableFunctionParameters.Count; i++)
			{
				array[i] = ((ITWIR)this).GetColumnValue(base.TableFunctionParameters[i]);
			}
			return ((IEnumerable)this.JetTableFunction.GetTableContents(base.ConnectionProvider, array)).GetEnumerator();
		}

		private byte[] GetPhysicalColumnValueAsBytes(PhysicalColumn column)
		{
			object obj = base.TableFunction.GetColumnFromRow(base.ConnectionProvider, this.tableFunctionContents.Current, column);
			base.Connection.AddRowStatsCounter(base.Table, RowStatsCounterType.ReadBytes, ValueTypeHelper.ValueSize(column.ExtendedTypeCode, obj));
			return JetColumnValueHelper.GetAsByteArray(obj, column);
		}

		private object GetPhysicalColumnValue(PhysicalColumn column)
		{
			object obj = base.TableFunction.GetColumnFromRow(base.ConnectionProvider, this.tableFunctionContents.Current, column);
			base.Connection.AddRowStatsCounter(base.Table, RowStatsCounterType.ReadBytes, ValueTypeHelper.ValueSize(column.ExtendedTypeCode, obj));
			return obj;
		}

		private object GetPropertyColumnValue(PropertyColumn column)
		{
			throw new NotSupportedException("Table function does not have property columns");
		}

		byte[] IJetSimpleQueryOperator.GetColumnValueAsBytes(Column column)
		{
			if (base.RenameDictionary != null)
			{
				column = base.ResolveColumn(column);
			}
			if (base.OuterQuery.ColumnsToFetch.Contains(column))
			{
				return this.JetOuterQuery.GetColumnValueAsBytes(column);
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
			if (base.OuterQuery.ColumnsToFetch.Contains(column))
			{
				return this.JetOuterQuery.GetColumnSize(column);
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
			if (base.OuterQuery.ColumnsToFetch.Contains(column))
			{
				return this.JetOuterQuery.GetColumnValue(column);
			}
			IColumn column2 = column;
			return column2.GetValue(this);
		}

		int ITWIR.GetPhysicalColumnSize(PhysicalColumn column)
		{
			byte[] physicalColumnValueAsBytes = this.GetPhysicalColumnValueAsBytes(column);
			if (physicalColumnValueAsBytes == null)
			{
				return 0;
			}
			return physicalColumnValueAsBytes.Length;
		}

		object ITWIR.GetPhysicalColumnValue(PhysicalColumn column)
		{
			return this.GetPhysicalColumnValue(column);
		}

		int ITWIR.GetPropertyColumnSize(PropertyColumn column)
		{
			return this.JetOuterQuery.GetPropertyColumnSize(column);
		}

		object ITWIR.GetPropertyColumnValue(PropertyColumn column)
		{
			return this.GetPropertyColumnValue(column);
		}

		object IRowAccess.GetPhysicalColumn(PhysicalColumn column)
		{
			return this.GetPhysicalColumnValue(column);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JetApplyOperator>(this);
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
			this.JetOuterQuery.RequestResume();
			return true;
		}

		private IInterruptControl interruptControl;

		private bool interrupted;

		private uint rowsReturned;

		private IEnumerator tableFunctionContents;
	}
}
