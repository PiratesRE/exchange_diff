using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlInsertOperator : InsertOperator
	{
		internal SqlInsertOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, SimpleQueryOperator simpleQueryOperator, IList<Column> columnsToInsert, IList<object> valuesToInsert, Column columnToFetch, bool frequentOperation) : base(culture, connectionProvider, table, simpleQueryOperator, columnsToInsert, valuesToInsert, columnToFetch, frequentOperation)
		{
		}

		public override object ExecuteScalar()
		{
			base.TraceOperation("ExecuteScalar");
			if (base.Table.ReadOnly)
			{
				throw new NonFatalDatabaseException("Cannot insert into a readonly table.");
			}
			this.sqlCommand = new SqlCommand(base.Connection);
			this.sqlCommand.StartNewStatement(Connection.OperationType.Insert);
			ISqlSimpleQueryOperator sqlSimpleQueryOperator = null;
			if (base.SimpleQueryOperator != null)
			{
				sqlSimpleQueryOperator = (base.SimpleQueryOperator as ISqlSimpleQueryOperator);
				if (sqlSimpleQueryOperator.NeedCteForSqlStatement())
				{
					this.sqlCommand.Append("WITH ");
					sqlSimpleQueryOperator.BuildCteForSqlStatement(this.sqlCommand);
					this.sqlCommand.Append(" ");
				}
			}
			this.sqlCommand.Append("INSERT INTO [Exchange].[");
			this.sqlCommand.Append(base.Table.Name);
			this.sqlCommand.Append("] (");
			for (int i = 0; i < base.ColumnsToInsert.Count; i++)
			{
				if (i != 0)
				{
					this.sqlCommand.Append(", ");
				}
				this.sqlCommand.Append(base.ColumnsToInsert[i].Name);
			}
			this.sqlCommand.Append(") ");
			if (base.ColumnToFetch != null)
			{
				this.sqlCommand.Append(" OUTPUT INSERTED.");
				this.sqlCommand.Append(base.ColumnToFetch.Name);
				this.sqlCommand.Append(" ");
			}
			if (base.SimpleQueryOperator != null)
			{
				sqlSimpleQueryOperator.AddToInsert(this.sqlCommand);
			}
			else
			{
				this.sqlCommand.Append("VALUES (");
				for (int j = 0; j < base.ColumnsToInsert.Count; j++)
				{
					if (j != 0)
					{
						this.sqlCommand.Append(", ");
					}
					this.sqlCommand.AppendParameter(base.ValuesToInsert[j]);
				}
				this.sqlCommand.Append(") ");
			}
			this.sqlCommand.AppendQueryHints(base.FrequentOperation);
			object result;
			using (base.Connection.TrackDbOperationExecution(this))
			{
				if (base.ColumnToFetch != null)
				{
					result = this.sqlCommand.ExecuteScalar(Connection.TransactionOption.NeedTransaction);
				}
				else
				{
					result = this.sqlCommand.ExecuteNonQuery(Connection.TransactionOption.NeedTransaction);
				}
			}
			base.TraceOperationResult("ExecuteScalar", base.ColumnToFetch, result);
			return result;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.sqlCommand != null)
			{
				this.sqlCommand.Dispose();
			}
			base.InternalDispose(calledFromDispose);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SqlInsertOperator>(this);
		}

		private SqlCommand sqlCommand;
	}
}
