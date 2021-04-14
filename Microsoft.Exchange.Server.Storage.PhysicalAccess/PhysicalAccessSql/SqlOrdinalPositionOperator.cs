using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlOrdinalPositionOperator : OrdinalPositionOperator
	{
		internal SqlOrdinalPositionOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, SortOrder keySortOrder, StartStopKey key, bool frequentOperation) : this(connectionProvider, new OrdinalPositionOperator.OrdinalPositionOperatorDefinition(culture, (queryOperator != null) ? queryOperator.OperatorDefinition : null, keySortOrder, key, frequentOperation))
		{
		}

		internal SqlOrdinalPositionOperator(IConnectionProvider connectionProvider, OrdinalPositionOperator.OrdinalPositionOperatorDefinition definition) : base(connectionProvider, definition)
		{
		}

		public override object ExecuteScalar()
		{
			base.TraceOperation("ExecuteScalar");
			object result;
			if (base.QueryOperator != null)
			{
				this.BuildSqlStatement();
				using (base.Connection.TrackDbOperationExecution(this))
				{
					result = this.sqlCommand.ExecuteScalar(Connection.TransactionOption.DontNeedTransaction);
					goto IL_4A;
				}
			}
			result = 0;
			IL_4A:
			base.TraceOperationResult("ExecuteScalar", null, result);
			return result;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SqlOrdinalPositionOperator>(this);
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
			this.BuildSqlStatement(this.sqlCommand);
			this.sqlCommand.AppendQueryHints(base.FrequentOperation);
		}

		private void BuildSqlStatement(SqlCommand sqlCommand)
		{
			sqlCommand.Append("SELECT COUNT(*) FROM ordinalDrivingLeg WHERE (");
			for (int i = 0; i < base.Key.Count; i++)
			{
				if (i > 0)
				{
					sqlCommand.Append(" OR ");
				}
				sqlCommand.Append("(");
				for (int j = 0; j < i; j++)
				{
					SqlTableOperator.AppendKeyColumnComparison(base.Culture, sqlCommand, base.QueryOperator.ColumnsToFetch[j], SearchCriteriaCompare.SearchRelOp.Equal, base.Key.Values[j]);
					sqlCommand.Append(" AND ");
				}
				bool flag = !base.KeySortOrder.Ascending[i];
				bool flag2 = i == base.Key.Count - 1;
				bool flag3 = flag2 && !base.Key.Inclusive;
				SearchCriteriaCompare.SearchRelOp relOp = flag ? (flag3 ? SearchCriteriaCompare.SearchRelOp.GreaterThanEqual : SearchCriteriaCompare.SearchRelOp.GreaterThan) : (flag3 ? SearchCriteriaCompare.SearchRelOp.LessThanEqual : SearchCriteriaCompare.SearchRelOp.LessThan);
				SqlTableOperator.AppendKeyColumnComparison(base.Culture, sqlCommand, base.QueryOperator.ColumnsToFetch[i], relOp, base.Key.Values[i]);
				sqlCommand.Append(")");
			}
			sqlCommand.Append(")");
		}

		private void BuildCteForSqlStatement(SqlCommand sqlCommand)
		{
			ISqlSimpleQueryOperator sqlSimpleQueryOperator = (ISqlSimpleQueryOperator)base.QueryOperator;
			if (sqlSimpleQueryOperator.NeedCteForSqlStatement())
			{
				sqlSimpleQueryOperator.BuildCteForSqlStatement(sqlCommand);
				sqlCommand.Append(", ");
			}
			sqlCommand.Append("ordinalDrivingLeg AS (");
			sqlSimpleQueryOperator.BuildSqlStatement(sqlCommand, true);
			sqlCommand.Append(")");
		}

		private bool NeedCteForSqlStatement()
		{
			return true;
		}

		private SqlCommand sqlCommand;
	}
}
