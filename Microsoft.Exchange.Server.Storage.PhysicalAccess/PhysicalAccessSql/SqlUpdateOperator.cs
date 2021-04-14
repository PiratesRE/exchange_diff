using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlUpdateOperator : UpdateOperator
	{
		internal SqlUpdateOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableOperator tableOperator, IList<Column> columnsToUpdate, IList<object> valuesToUpdate, bool frequentOperation) : base(culture, connectionProvider, tableOperator, columnsToUpdate, valuesToUpdate, frequentOperation)
		{
		}

		public override object ExecuteScalar()
		{
			base.TraceOperation("ExecuteScalar");
			if (base.TableOperator.Table.ReadOnly)
			{
				throw new NonFatalDatabaseException("Cannot update a readonly table.");
			}
			this.sqlCommand = new SqlCommand(base.Connection);
			this.sqlCommand.StartNewStatement(Connection.OperationType.Update);
			this.sqlCommand.Append("UPDATE [Exchange].[");
			this.sqlCommand.Append(base.TableOperator.Table.Name);
			this.sqlCommand.Append("] SET ");
			for (int i = 0; i < base.ColumnsToUpdate.Count; i++)
			{
				if (i != 0)
				{
					this.sqlCommand.Append(", ");
				}
				this.sqlCommand.Append(base.ColumnsToUpdate[i].Name);
				this.sqlCommand.Append("=");
				if (base.ValuesToUpdate[i] is Column)
				{
					((ISqlColumn)base.ValuesToUpdate[i]).AppendExpressionToQuery(SqlQueryModel.Shorthand, ColumnUse.FetchList, this.sqlCommand);
				}
				else
				{
					this.sqlCommand.AppendParameter(base.ValuesToUpdate[i]);
				}
			}
			SqlTableOperator sqlTableOperator = base.TableOperator as SqlTableOperator;
			sqlTableOperator.AddToUpdateDelete(this.sqlCommand);
			this.sqlCommand.AppendQueryHints(base.FrequentOperation);
			object result;
			using (base.Connection.TrackDbOperationExecution(this))
			{
				result = this.sqlCommand.ExecuteNonQuery(Connection.TransactionOption.NeedTransaction);
			}
			base.TraceOperationResult("ExecuteScalar", null, result);
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
			return DisposeTracker.Get<SqlUpdateOperator>(this);
		}

		private SqlCommand sqlCommand;
	}
}
