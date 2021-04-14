using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlIndexOrOperator : IndexOrOperator, ISqlSimpleQueryOperator
	{
		internal SqlIndexOrOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator[] queryOperators, bool frequentOperation) : this(connectionProvider, new IndexOrOperator.IndexOrOperatorDefinition(culture, columnsToFetch, (from op in queryOperators
		select op.OperatorDefinition).ToArray<SimpleQueryOperator.SimpleQueryOperatorDefinition>(), frequentOperation))
		{
		}

		internal SqlIndexOrOperator(IConnectionProvider connectionProvider, IndexOrOperator.IndexOrOperatorDefinition definition) : base(connectionProvider, definition)
		{
		}

		public override Reader ExecuteReader(bool disposeQueryOperator)
		{
			base.TraceOperation("ExecuteReader");
			this.BuildSqlStatement();
			Reader result;
			using (base.Connection.TrackDbOperationExecution(this))
			{
				result = this.sqlCommand.ExecuteReader(Connection.TransactionOption.DontNeedTransaction, 0, this, disposeQueryOperator);
			}
			return result;
		}

		public override object ExecuteScalar()
		{
			base.TraceOperation("ExecuteScalar");
			this.BuildSqlStatement();
			object result;
			using (base.Connection.TrackDbOperationExecution(this))
			{
				result = this.sqlCommand.ExecuteScalar(Connection.TransactionOption.DontNeedTransaction);
			}
			base.TraceOperationResult("ExecuteScalar", null, result);
			return result;
		}

		public void BuildSqlStatement(SqlCommand sqlCommand, bool orderedResultsNeeded)
		{
			new SingleTableQueryModel("IndexOrDrivingLeg");
			sqlCommand.Append("SELECT ");
			((ISqlSimpleQueryOperator)base.QueryOperators[0]).AppendSelectList(sqlCommand, SqlQueryModel.Shorthand, false);
			sqlCommand.Append(" FROM IndexOrDrivingLeg");
		}

		public void BuildCteForSqlStatement(SqlCommand sqlCommand)
		{
			foreach (ISqlSimpleQueryOperator sqlSimpleQueryOperator in base.QueryOperators)
			{
				if (sqlSimpleQueryOperator.NeedCteForSqlStatement())
				{
					sqlSimpleQueryOperator.BuildCteForSqlStatement(sqlCommand);
					sqlCommand.Append(", ");
				}
			}
			bool flag = true;
			sqlCommand.Append("IndexOrDrivingLeg AS (");
			foreach (ISqlSimpleQueryOperator sqlSimpleQueryOperator2 in base.QueryOperators)
			{
				if (!flag)
				{
					sqlCommand.Append(" UNION ");
				}
				sqlSimpleQueryOperator2.BuildSqlStatement(sqlCommand, false);
				flag = false;
			}
			sqlCommand.Append(")");
		}

		public bool NeedCteForSqlStatement()
		{
			return true;
		}

		public void AppendSelectList(SqlCommand sqlCommand, SqlQueryModel model, bool orderedResultsNeeded)
		{
			IList<Column> list = SqlTableOperator.RemoveDuplicateColumns(base.ColumnsToFetch);
			for (int i = 0; i < list.Count; i++)
			{
				if (i != 0)
				{
					sqlCommand.Append(", ");
				}
				model.AppendColumnToQuery(list[i], ColumnUse.FetchList, sqlCommand);
				sqlCommand.Append(" AS ");
				((ISqlColumn)list[i]).AppendNameToQuery(sqlCommand);
			}
		}

		public void AddToInsert(SqlCommand sqlCommand)
		{
			this.BuildSqlStatement(sqlCommand, false);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SqlIndexOrOperator>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.sqlCommand != null)
			{
				this.sqlCommand.Dispose();
			}
			base.InternalDispose(calledFromDispose);
		}

		private void BuildSqlStatement()
		{
			if (this.sqlCommand == null)
			{
				this.sqlCommand = new SqlCommand(base.Connection);
			}
			this.sqlCommand.StartNewStatement(Connection.OperationType.Query);
			if (this.NeedCteForSqlStatement())
			{
				this.sqlCommand.Append("WITH ");
				this.BuildCteForSqlStatement(this.sqlCommand);
				this.sqlCommand.Append(" ");
			}
			this.BuildSqlStatement(this.sqlCommand, false);
			this.sqlCommand.AppendQueryHints(base.FrequentOperation);
		}

		private SqlCommand sqlCommand;
	}
}
