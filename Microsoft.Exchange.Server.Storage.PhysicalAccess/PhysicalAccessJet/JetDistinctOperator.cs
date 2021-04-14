using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetDistinctOperator : DistinctOperator, IJetSimpleQueryOperator, IJetRecordCounter, ITWIR
	{
		internal JetDistinctOperator(IConnectionProvider connectionProvider, int skipTo, int maxRows, SimpleQueryOperator outerQuery, bool frequentOperation) : this(connectionProvider, new DistinctOperator.DistinctOperatorDefinition(skipTo, maxRows, outerQuery.OperatorDefinition, frequentOperation))
		{
		}

		internal JetDistinctOperator(IConnectionProvider connectionProvider, DistinctOperator.DistinctOperatorDefinition definition) : base(connectionProvider, definition)
		{
			this.uniqueRowsSeen = new HashSet<object[]>(new ValueArrayEqualityComparer());
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
			base.Connection.CountStatement(Connection.OperationType.Query);
			return new JetReader(base.ConnectionProvider, this, disposeQueryOperator);
		}

		public override bool EnableInterrupts(IInterruptControl interruptControl)
		{
			return (interruptControl == null || base.SkipTo == 0) && base.OuterQuery.EnableInterrupts(interruptControl);
		}

		public bool MoveFirst(out int rowsSkipped)
		{
			rowsSkipped = 0;
			this.rowsReturned = 0U;
			this.uniqueRowsSeen.Clear();
			int num;
			if (!this.JetOuterQuery.MoveFirst(out num))
			{
				base.TraceMove("MoveFirst", false);
				return false;
			}
			if (this.JetOuterQuery.Interrupted)
			{
				base.TraceCrumb("MoveFirst", "Interrupt");
				return true;
			}
			this.uniqueRowsSeen.Add(this.GetRowFromOuter());
			int num2 = base.SkipTo;
			if (num2 > 0)
			{
				rowsSkipped++;
				num2--;
				return this.MoveNext("MoveFirst", num2, ref rowsSkipped);
			}
			base.TraceMove("MoveFirst", true);
			this.rowsReturned += 1U;
			return true;
		}

		public bool MoveNext()
		{
			int num = 0;
			return this.MoveNext("MoveNext", 0, ref num);
		}

		internal bool MoveNext(string operation, int numberLeftToSkip, ref int rowsSkipped)
		{
			bool flag = false;
			if (base.MaxRows <= 0 || (ulong)this.rowsReturned < (ulong)((long)base.MaxRows))
			{
				for (;;)
				{
					flag = this.JetOuterQuery.MoveNext();
					if (!flag)
					{
						goto IL_92;
					}
					if (this.JetOuterQuery.Interrupted)
					{
						break;
					}
					object[] rowFromOuter = this.GetRowFromOuter();
					if (this.uniqueRowsSeen.Contains(rowFromOuter))
					{
						this.TraceRowNotUnique();
					}
					else
					{
						this.uniqueRowsSeen.Add(rowFromOuter);
						if (numberLeftToSkip <= 0)
						{
							goto IL_82;
						}
						rowsSkipped++;
						numberLeftToSkip--;
					}
				}
				base.TraceCrumb(operation, "Interrupt");
				return true;
				IL_82:
				flag = true;
				this.rowsReturned += 1U;
			}
			IL_92:
			base.TraceMove(operation, flag);
			return flag;
		}

		byte[] IJetSimpleQueryOperator.GetColumnValueAsBytes(Column column)
		{
			return this.JetOuterQuery.GetColumnValueAsBytes(column);
		}

		byte[] IJetSimpleQueryOperator.GetPhysicalColumnValueAsBytes(PhysicalColumn column)
		{
			return this.JetOuterQuery.GetPhysicalColumnValueAsBytes(column);
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
			return this.JetOuterQuery.GetColumnSize(column);
		}

		object ITWIR.GetColumnValue(Column column)
		{
			return this.JetOuterQuery.GetColumnValue(column);
		}

		int ITWIR.GetPhysicalColumnSize(PhysicalColumn column)
		{
			return this.JetOuterQuery.GetPhysicalColumnSize(column);
		}

		object ITWIR.GetPhysicalColumnValue(PhysicalColumn column)
		{
			return this.JetOuterQuery.GetPhysicalColumnValue(column);
		}

		int ITWIR.GetPropertyColumnSize(PropertyColumn column)
		{
			return this.JetOuterQuery.GetPropertyColumnSize(column);
		}

		object ITWIR.GetPropertyColumnValue(PropertyColumn column)
		{
			return this.JetOuterQuery.GetPropertyColumnValue(column);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JetDistinctOperator>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				try
				{
					if (this.detailTracingEnabled)
					{
						StringBuilder stringBuilder = new StringBuilder(200);
						base.AppendOperationInfo("Dispose", stringBuilder);
						ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, stringBuilder.ToString());
					}
				}
				finally
				{
					base.InternalDispose(calledFromDispose);
				}
			}
		}

		private void TraceRowNotUnique()
		{
			if (this.intermediateTracingEnabled)
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				base.AppendOperationInfo("ROW IS NOT UNIQUE", stringBuilder);
				if (base.OuterQuery.ColumnsToFetch != null)
				{
					stringBuilder.Append("  outer_columns:[");
					base.TraceAppendColumns(stringBuilder, this.JetOuterQuery, base.OuterQuery.ColumnsToFetch);
					stringBuilder.Append("]");
				}
				ExTraceGlobals.DbInteractionIntermediateTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		private object[] GetRowFromOuter()
		{
			IList<Column> columnsToFetch = base.OuterQuery.ColumnsToFetch;
			object[] array = new object[columnsToFetch.Count];
			for (int i = 0; i < columnsToFetch.Count; i++)
			{
				array[i] = this.JetOuterQuery.GetColumnValue(columnsToFetch[i]);
			}
			return array;
		}

		public override bool Interrupted
		{
			get
			{
				return this.JetOuterQuery.Interrupted;
			}
		}

		void IJetSimpleQueryOperator.RequestResume()
		{
			this.JetOuterQuery.RequestResume();
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

		private uint rowsReturned;

		private HashSet<object[]> uniqueRowsSeen;
	}
}
