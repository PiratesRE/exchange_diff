using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetIndexAndOperator : IndexAndOperator, IJetSimpleQueryOperator, IJetRecordCounter, ITWIR
	{
		internal JetIndexAndOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator[] queryOperators, bool frequentOperation) : this(connectionProvider, new IndexAndOperator.IndexAndOperatorDefinition(culture, columnsToFetch, (from op in queryOperators
		select op.OperatorDefinition).ToArray<SimpleQueryOperator.SimpleQueryOperatorDefinition>(), frequentOperation))
		{
		}

		internal JetIndexAndOperator(IConnectionProvider connectionProvider, IndexAndOperator.IndexAndOperatorDefinition definition) : base(connectionProvider, definition)
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

		internal static List<object[]> GetAllRows(SimpleQueryOperator queryOperator)
		{
			List<object[]> result;
			using (queryOperator.Connection.TrackTimeInDatabase())
			{
				List<object[]> list = new List<object[]>(100);
				IJetSimpleQueryOperator jetSimpleQueryOperator = (IJetSimpleQueryOperator)queryOperator;
				int num;
				if (!jetSimpleQueryOperator.MoveFirst(out num))
				{
					result = null;
				}
				else
				{
					bool flag = true;
					while (flag)
					{
						IList<Column> columnsToFetch = queryOperator.ColumnsToFetch;
						object[] array = new object[columnsToFetch.Count];
						for (int i = 0; i < columnsToFetch.Count; i++)
						{
							Column column = columnsToFetch[i];
							array[i] = jetSimpleQueryOperator.GetColumnValue(column);
						}
						list.Add(array);
						flag = jetSimpleQueryOperator.MoveNext();
					}
					result = list;
				}
			}
			return result;
		}

		private List<object[]> AndTables(List<object[]> table1, List<object[]> table2)
		{
			List<object[]> list = null;
			if (table1 != null && table2 != null)
			{
				foreach (object[] array in table1)
				{
					foreach (object[] array2 in table2)
					{
						bool flag = true;
						for (int i = 0; i < array.Length; i++)
						{
							if (!ValueHelper.ValuesEqual(array[i], array2[i], base.CompareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							if (list == null)
							{
								list = new List<object[]>(100);
							}
							list.Add(array);
							break;
						}
					}
				}
			}
			return list;
		}

		public bool MoveFirst(out int rowsSkipped)
		{
			rowsSkipped = 0;
			this.rowIndex = 0U;
			for (int i = 0; i < base.QueryOperators.Length; i++)
			{
				List<object[]> allRows = JetIndexAndOperator.GetAllRows(base.QueryOperators[i]);
				if (i == 0)
				{
					this.results = allRows;
				}
				else
				{
					this.results = this.AndTables(this.results, allRows);
				}
			}
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
			throw new InvalidOperationException("This method should not be called in an IndexAnd operator");
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
			throw new InvalidOperationException("This method should not be called in an IndexAnd operator");
		}

		object ITWIR.GetColumnValue(Column column)
		{
			return this.GetColumnValue(column);
		}

		int ITWIR.GetPhysicalColumnSize(PhysicalColumn column)
		{
			throw new InvalidOperationException("This method should not be called in an IndexAnd operator");
		}

		object ITWIR.GetPhysicalColumnValue(PhysicalColumn column)
		{
			throw new InvalidOperationException("This method should not be called in an IndexAnd operator");
		}

		int ITWIR.GetPropertyColumnSize(PropertyColumn column)
		{
			throw new InvalidOperationException("This method should not be called in an IndexAnd operator");
		}

		object ITWIR.GetPropertyColumnValue(PropertyColumn column)
		{
			throw new InvalidOperationException("This method should not be called in an IndexAnd operator");
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JetIndexAndOperator>(this);
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

		private const string ThisMethodShouldNotBeCalled = "This method should not be called in an IndexAnd operator";

		private uint rowIndex;

		private List<object[]> results;
	}
}
