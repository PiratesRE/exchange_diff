using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetIndexNotOperator : IndexNotOperator, IJetSimpleQueryOperator, IJetRecordCounter, ITWIR
	{
		internal JetIndexNotOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator queryOperator, SimpleQueryOperator notOperator, bool frequentOperation) : this(connectionProvider, new IndexNotOperator.IndexNotOperatorDefinition(culture, columnsToFetch, queryOperator.OperatorDefinition, notOperator.OperatorDefinition, frequentOperation))
		{
		}

		internal JetIndexNotOperator(IConnectionProvider connectionProvider, IndexNotOperator.IndexNotOperatorDefinition definition) : base(connectionProvider, definition)
		{
		}

		private JetConnection JetConnection
		{
			get
			{
				return (JetConnection)base.Connection;
			}
		}

		public override Reader ExecuteReader(bool disposeQueryOperator)
		{
			base.TraceOperation("ExecuteReader");
			return new JetReader(base.ConnectionProvider, this, disposeQueryOperator);
		}

		private List<object[]> NotTables(List<object[]> table1, List<object[]> table2)
		{
			List<object[]> list = null;
			if (table1 != null)
			{
				foreach (object[] array in table1)
				{
					bool flag = false;
					if (table2 != null)
					{
						foreach (object[] array2 in table2)
						{
							bool flag2 = true;
							for (int i = 0; i < array2.Length; i++)
							{
								if (!ValueHelper.ValuesEqual(array[i], array2[i], base.CompareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth))
								{
									flag2 = false;
									break;
								}
							}
							if (flag2)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						if (list == null)
						{
							list = new List<object[]>(100);
						}
						list.Add(array);
					}
				}
			}
			return list;
		}

		public bool MoveFirst(out int rowsSkipped)
		{
			rowsSkipped = 0;
			this.rowIndex = 0U;
			List<object[]> allRows = JetIndexAndOperator.GetAllRows(base.QueryOperator);
			List<object[]> allRows2 = JetIndexAndOperator.GetAllRows(base.NotOperator);
			this.results = this.NotTables(allRows, allRows2);
			bool flag = false;
			if (this.results != null && this.results.Count > 0)
			{
				flag = true;
			}
			base.TraceMove("MoveFirst", flag);
			return flag;
		}

		public bool MoveNext()
		{
			bool flag = false;
			if (this.results != null && (long)this.results.Count > (long)((ulong)(this.rowIndex + 1U)))
			{
				this.rowIndex += 1U;
				flag = true;
			}
			base.TraceMove("MoveNext", flag);
			return flag;
		}

		private object GetColumnValue(Column column)
		{
			int num = -1;
			for (int i = 0; i < base.ColumnsToFetch.Count; i++)
			{
				if (base.ColumnsToFetch[i] == column)
				{
					num = i;
					break;
				}
			}
			return this.results[(int)this.rowIndex][num];
		}

		byte[] IJetSimpleQueryOperator.GetColumnValueAsBytes(Column column)
		{
			return JetColumnValueHelper.GetAsByteArray(this.GetColumnValue(column), column);
		}

		byte[] IJetSimpleQueryOperator.GetPhysicalColumnValueAsBytes(PhysicalColumn column)
		{
			throw new InvalidOperationException("This method should not be called in an IndexNot operator");
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
			throw new InvalidOperationException("This method should not be called in an IndexNot operator");
		}

		object ITWIR.GetColumnValue(Column column)
		{
			return this.GetColumnValue(column);
		}

		int ITWIR.GetPhysicalColumnSize(PhysicalColumn column)
		{
			throw new InvalidOperationException("This method should not be called in an IndexNot operator");
		}

		object ITWIR.GetPhysicalColumnValue(PhysicalColumn column)
		{
			throw new InvalidOperationException("This method should not be called in an IndexNot operator");
		}

		int ITWIR.GetPropertyColumnSize(PropertyColumn column)
		{
			throw new InvalidOperationException("This method should not be called in an IndexNot operator");
		}

		object ITWIR.GetPropertyColumnValue(PropertyColumn column)
		{
			throw new InvalidOperationException("This method should not be called in an IndexNot operator");
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JetIndexNotOperator>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				base.InternalDispose(calledFromDispose);
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

		private const int DefaultRowsPerTable = 100;

		private const string ThisMethodShouldNotBeCalled = "This method should not be called in an IndexNot operator";

		private uint rowIndex;

		private List<object[]> results;
	}
}
