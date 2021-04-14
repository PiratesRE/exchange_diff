using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlTableFunctionOperator : TableFunctionOperator, ISqlSimpleQueryOperator
	{
		internal SqlTableFunctionOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableFunction tableFunction, object[] parameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation) : this(connectionProvider, new TableFunctionOperator.TableFunctionOperatorDefinition(culture, tableFunction, parameters, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, keyRanges, backwards, frequentOperation))
		{
		}

		internal SqlTableFunctionOperator(IConnectionProvider connectionProvider, TableFunctionOperator.TableFunctionOperatorDefinition definition) : base(connectionProvider, definition)
		{
		}

		public override Reader ExecuteReader(bool disposeQueryOperator)
		{
			base.TraceOperation("ExecuteReader");
			this.BuildSqlStatement();
			Reader result;
			using (base.Connection.TrackDbOperationExecution(this))
			{
				result = this.sqlCommand.ExecuteReader(Connection.TransactionOption.DontNeedTransaction, base.SkipTo, this, disposeQueryOperator);
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

		public void BuildSqlStatement(SqlCommand sqlCommand)
		{
			this.BuildSqlStatement(sqlCommand, true);
		}

		public void BuildCteForSqlStatement(SqlCommand sqlCommand)
		{
		}

		public bool NeedCteForSqlStatement()
		{
			return false;
		}

		public void BuildSqlStatement(SqlCommand sqlCommand, bool orderedResultsNeeded)
		{
			SqlQueryModel model = new SingleTableColumnRenameQueryModel(base.Table.Name, base.RenameDictionary);
			sqlCommand.Append("SELECT ");
			if (base.MaxRows != 0)
			{
				sqlCommand.Append("TOP(");
				sqlCommand.Append((base.SkipTo + base.MaxRows).ToString());
				sqlCommand.Append(")");
			}
			this.AppendSelectList(sqlCommand, model, true, orderedResultsNeeded);
			sqlCommand.Append(" FROM [Exchange].[");
			sqlCommand.Append(base.Table.Name);
			sqlCommand.Append("](");
			for (int i = 0; i < base.Parameters.Length; i++)
			{
				if (i > 0)
				{
					sqlCommand.Append(", ");
				}
				sqlCommand.AppendParameter(base.Parameters[i]);
			}
			sqlCommand.Append(")");
			this.AppendWhereClause(sqlCommand);
			if (this.sqlCommand == sqlCommand || base.MaxRows > 0)
			{
				sqlCommand.Append(" ORDER BY ");
				this.AppendDefaultOrderByList(sqlCommand);
			}
		}

		public void AppendSelectList(SqlCommand sqlCommand, SqlQueryModel model, bool orderedResultsNeeded)
		{
			this.AppendSelectList(sqlCommand, model, false, orderedResultsNeeded);
		}

		private void AppendSelectList(SqlCommand sqlCommand, SqlQueryModel model, bool baseTable, bool orderedResultsNeeded)
		{
			IList<Column> list = SqlTableOperator.RemoveDuplicateColumns(base.ColumnsToFetch);
			for (int i = 0; i < list.Count; i++)
			{
				if (i != 0)
				{
					sqlCommand.Append(", ");
				}
				Column column = list[i];
				if (base.RenameDictionary != null)
				{
					column = base.ResolveColumn(column);
				}
				((ISqlColumn)column).AppendExpressionToQuery(model, ColumnUse.FetchList, sqlCommand);
				sqlCommand.Append(" AS ");
				((ISqlColumn)list[i]).AppendNameToQuery(sqlCommand);
			}
			if (orderedResultsNeeded)
			{
				SortOrder sortOrder = base.SortOrder;
				for (int j = 0; j < sortOrder.Count; j++)
				{
					sqlCommand.Append(", ");
					if (baseTable)
					{
						model.AppendColumnToQuery(sortOrder[j].Column, ColumnUse.FetchList, sqlCommand);
						sqlCommand.Append(" AS ");
					}
					sqlCommand.Append("OB_");
					((ISqlColumn)sortOrder[j].Column).AppendNameToQuery(sqlCommand);
				}
			}
		}

		public void AddToInsert(SqlCommand sqlCommand)
		{
			this.BuildSqlStatement(sqlCommand, false);
		}

		internal void AddToUpdateDelete(SqlCommand sqlCommand)
		{
			this.AppendWhereClause(sqlCommand);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SqlTableFunctionOperator>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.sqlCommand != null)
			{
				this.sqlCommand.Dispose();
			}
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

		private void AppendWhereClause(SqlCommand sqlCommand)
		{
			if (base.KeyRanges.Count == 0)
			{
				sqlCommand.Append(" WHERE (0 = 1) ");
				return;
			}
			if (base.Criteria == null)
			{
				StartStopKey startKey = base.KeyRanges[0].StartKey;
				if (startKey.IsEmpty)
				{
					StartStopKey stopKey = base.KeyRanges[0].StopKey;
					if (stopKey.IsEmpty)
					{
						return;
					}
				}
			}
			bool flag = false;
			sqlCommand.Append(" WHERE ");
			StartStopKey startKey2 = base.KeyRanges[0].StartKey;
			if (startKey2.IsEmpty)
			{
				StartStopKey stopKey2 = base.KeyRanges[0].StopKey;
				if (stopKey2.IsEmpty)
				{
					goto IL_C4;
				}
			}
			SqlTableOperator.AppendKeyRangeCriteria(base.Culture, base.Connection, sqlCommand, base.SortOrder, base.Backwards, base.KeyRanges);
			flag = true;
			IL_C4:
			if (base.Criteria != null)
			{
				if (flag)
				{
					sqlCommand.Append(" AND ");
				}
				sqlCommand.Append("(");
				this.AppendCriteria(sqlCommand, base.Criteria);
				sqlCommand.Append(")");
			}
		}

		private void AppendDefaultOrderByList(SqlCommand sqlCommand)
		{
			int count = base.SortOrder.Count;
			for (int i = 0; i < count; i++)
			{
				if (i != 0)
				{
					sqlCommand.Append(", ");
				}
				sqlCommand.Append(base.SortOrder.Columns[i].Name);
				SqlCollationHelper.AppendCollation(base.SortOrder.Columns[i], base.Culture, sqlCommand);
				bool flag = !base.SortOrder.Ascending[i];
				if (base.Backwards)
				{
					flag = !flag;
				}
				if (flag)
				{
					sqlCommand.Append(" DESC");
				}
			}
		}

		private void AppendCriteria(SqlCommand sqlCommand, SearchCriteria restriction)
		{
			SqlQueryModel model = new SingleTableColumnRenameQueryModel(base.Table.Name, base.RenameDictionary);
			ISqlSearchCriteria sqlSearchCriteria = restriction as ISqlSearchCriteria;
			sqlSearchCriteria.AppendQueryText(base.Culture, model, sqlCommand);
		}

		private SqlCommand sqlCommand;
	}
}
