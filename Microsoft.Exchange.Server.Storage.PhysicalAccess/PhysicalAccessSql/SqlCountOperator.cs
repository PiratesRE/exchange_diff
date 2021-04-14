using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlCountOperator : CountOperator
	{
		internal SqlCountOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, bool frequentOperation) : this(connectionProvider, new CountOperator.CountOperatorDefinition(culture, (queryOperator != null) ? queryOperator.OperatorDefinition : null, frequentOperation))
		{
		}

		internal SqlCountOperator(IConnectionProvider connectionProvider, CountOperator.CountOperatorDefinition definition) : base(connectionProvider, definition)
		{
		}

		public override object ExecuteScalar()
		{
			base.TraceOperation("ExecuteScalar");
			object result;
			using (base.Connection.TrackDbOperationExecution(this))
			{
				if (base.QueryOperator != null)
				{
					this.BuildSqlStatement();
					result = this.sqlCommand.ExecuteScalar(Connection.TransactionOption.DontNeedTransaction);
				}
				else
				{
					result = 0;
				}
			}
			base.TraceOperationResult("ExecuteScalar", null, result);
			return result;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SqlCountOperator>(this);
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
			sqlCommand.Append("SELECT COUNT(*) FROM countDrivingLeg");
		}

		private void BuildCteForSqlStatement(SqlCommand sqlCommand)
		{
			ISqlSimpleQueryOperator sqlSimpleQueryOperator = (ISqlSimpleQueryOperator)base.QueryOperator;
			if (sqlSimpleQueryOperator.NeedCteForSqlStatement())
			{
				sqlSimpleQueryOperator.BuildCteForSqlStatement(sqlCommand);
				sqlCommand.Append(", ");
			}
			sqlCommand.Append("countDrivingLeg AS (");
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
