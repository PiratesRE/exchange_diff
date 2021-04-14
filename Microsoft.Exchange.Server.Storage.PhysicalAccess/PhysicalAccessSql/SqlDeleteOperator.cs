using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlDeleteOperator : DeleteOperator
	{
		internal SqlDeleteOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableOperator tableOperator, bool frequentOperation) : base(culture, connectionProvider, tableOperator, frequentOperation)
		{
		}

		public override object ExecuteScalar()
		{
			base.TraceOperation("ExecuteScalar");
			if (base.TableOperator.Table.ReadOnly)
			{
				throw new NonFatalDatabaseException("Cannot delete from a readonly table.");
			}
			this.sqlCommand = new SqlCommand(base.Connection);
			this.sqlCommand.StartNewStatement(Connection.OperationType.Delete);
			this.sqlCommand.Append("DELETE ");
			if (base.TableOperator.MaxRows != 0)
			{
				this.sqlCommand.Append("TOP(");
				this.sqlCommand.Append(base.TableOperator.MaxRows.ToString());
				this.sqlCommand.Append(") ");
			}
			this.sqlCommand.Append("FROM [Exchange].[");
			this.sqlCommand.Append(base.TableOperator.Table.Name);
			this.sqlCommand.Append("] ");
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
			return DisposeTracker.Get<SqlDeleteOperator>(this);
		}

		private SqlCommand sqlCommand;
	}
}
