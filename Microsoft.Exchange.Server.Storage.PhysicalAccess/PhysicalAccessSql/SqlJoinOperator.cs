using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlJoinOperator : JoinOperator, ISqlSimpleQueryOperator
	{
		internal SqlJoinOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<Column> keyColumns, SimpleQueryOperator outerQuery, bool frequentOperation) : this(connectionProvider, new JoinOperator.JoinOperatorDefinition(culture, table, columnsToFetch, longValueColumnsToPreread, restriction, renameDictionary, skipTo, maxRows, keyColumns, outerQuery.OperatorDefinition, frequentOperation))
		{
		}

		internal SqlJoinOperator(IConnectionProvider connectionProvider, JoinOperator.JoinOperatorDefinition definition) : base(connectionProvider, definition)
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
			this.BuildSqlStatement(sqlCommand, !base.SortOrder.IsEmpty);
		}

		public void BuildCteForSqlStatement(SqlCommand sqlCommand)
		{
			ISqlSimpleQueryOperator sqlSimpleQueryOperator = (ISqlSimpleQueryOperator)base.OuterQuery;
			if (sqlSimpleQueryOperator.NeedCteForSqlStatement())
			{
				sqlSimpleQueryOperator.BuildCteForSqlStatement(sqlCommand);
				sqlCommand.Append(", ");
			}
			SqlQueryModel model = new SqlJoinOperator.JoinQueryModel(this, "bt", "ol");
			string str = (base.OuterQuery is TableOperator) ? string.Empty : "2";
			sqlCommand.Append("drivingLeg");
			sqlCommand.Append(str);
			sqlCommand.Append(" AS (");
			sqlSimpleQueryOperator.BuildSqlStatement(sqlCommand, !base.SortOrder.IsEmpty);
			sqlCommand.Append("), innerLeg");
			sqlCommand.Append(str);
			sqlCommand.Append(" AS (SELECT ");
			this.AppendSelectList(sqlCommand, model, !base.SortOrder.IsEmpty);
			sqlCommand.Append(" FROM ");
			sqlCommand.AppendFromList(model);
			sqlCommand.Append(" ON ");
			for (int i = 0; i < base.KeyColumns.Count; i++)
			{
				if (i > 0)
				{
					sqlCommand.Append(" AND ");
				}
				bool flag = base.KeyColumns[i].IsNullable && base.OuterQuery.ColumnsToFetch[i].IsNullable;
				if (flag)
				{
					sqlCommand.Append(" ((");
					sqlCommand.Append("bt.");
					((ISqlColumn)base.KeyColumns[i]).AppendNameToQuery(sqlCommand);
					sqlCommand.Append(" IS NULL AND ");
					sqlCommand.Append("ol.");
					((ISqlColumn)base.OuterQuery.ColumnsToFetch[i]).AppendNameToQuery(sqlCommand);
					sqlCommand.Append(" IS NULL) OR ");
				}
				sqlCommand.Append("bt.");
				((ISqlColumn)base.KeyColumns[i]).AppendNameToQuery(sqlCommand);
				sqlCommand.Append(" = ");
				sqlCommand.Append("ol.");
				((ISqlColumn)base.OuterQuery.ColumnsToFetch[i]).AppendNameToQuery(sqlCommand);
				SqlCollationHelper.AppendCollation(base.KeyColumns[i], base.Culture, sqlCommand);
				if (flag)
				{
					sqlCommand.Append(")");
				}
			}
			this.AppendWhereClause(sqlCommand, model);
			sqlCommand.Append(")");
		}

		public bool NeedCteForSqlStatement()
		{
			return true;
		}

		public void BuildSqlStatement(SqlCommand sqlCommand, bool orderedResultsNeeded)
		{
			string str = (base.OuterQuery is TableOperator) ? string.Empty : "2";
			sqlCommand.Append("SELECT ");
			if (base.MaxRows != 0)
			{
				sqlCommand.Append(" TOP(");
				sqlCommand.Append((base.SkipTo + base.MaxRows).ToString());
				sqlCommand.Append(")");
			}
			this.AppendSelectList(sqlCommand, SqlQueryModel.Shorthand, orderedResultsNeeded);
			sqlCommand.Append(" FROM innerLeg");
			sqlCommand.Append(str);
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
			if (orderedResultsNeeded)
			{
				SortOrder sortOrder = base.OuterQuery.SortOrder;
				for (int j = 0; j < sortOrder.Count; j++)
				{
					sqlCommand.Append(", OB_");
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
			this.AppendWhereClause(sqlCommand, SqlQueryModel.Shorthand);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SqlJoinOperator>(this);
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
			}
			this.BuildSqlStatement(this.sqlCommand);
			this.AppendDefaultOrderByList(this.sqlCommand);
			this.sqlCommand.AppendQueryHints(base.FrequentOperation);
		}

		private void AppendWhereClause(SqlCommand sqlCommand, SqlQueryModel model)
		{
			if (base.Criteria != null)
			{
				sqlCommand.Append(" WHERE ");
				sqlCommand.Append("(");
				this.AppendCriteria(sqlCommand, model, base.Criteria);
				sqlCommand.Append(")");
			}
		}

		private void AppendDefaultOrderByList(SqlCommand sqlCommand)
		{
			if (!base.SortOrder.IsEmpty)
			{
				sqlCommand.Append(" ORDER BY ");
				SortOrder sortOrder = base.OuterQuery.SortOrder;
				int count = sortOrder.Count;
				for (int i = 0; i < count; i++)
				{
					if (i != 0)
					{
						sqlCommand.Append(", ");
					}
					sqlCommand.Append("OB_");
					sqlCommand.Append(sortOrder[i].Column.Name);
					SqlCollationHelper.AppendCollation(sortOrder[i].Column, base.Culture, sqlCommand);
					bool flag = !sortOrder[i].Ascending;
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
		}

		private void AppendCriteria(SqlCommand sqlCommand, SqlQueryModel model, SearchCriteria restriction)
		{
			ISqlSearchCriteria sqlSearchCriteria = (ISqlSearchCriteria)restriction;
			sqlSearchCriteria.AppendQueryText(base.Culture, model, sqlCommand);
		}

		private SqlCommand sqlCommand;

		private class JoinQueryModel : SqlQueryModel
		{
			public JoinQueryModel(SqlJoinOperator joinOperator, string baseTablePrefix, string outerLegPrefix)
			{
				this.joinOperator = joinOperator;
				this.baseTablePrefix = baseTablePrefix;
				this.outerLegPrefix = outerLegPrefix;
			}

			public override void AppendFromList(SqlCommand command)
			{
				string str = (this.joinOperator.OuterQuery is TableOperator) ? string.Empty : "2";
				command.Append("drivingLeg");
				command.Append(str);
				command.Append(" AS ");
				command.Append(this.outerLegPrefix);
				command.Append(" JOIN [Exchange].[");
				command.Append(this.joinOperator.Table.Name);
				command.Append("] AS ");
				command.Append(this.baseTablePrefix);
			}

			public override void AppendColumnToQuery(Column column, ColumnUse use, SqlCommand command)
			{
				if (this.joinOperator.OuterQuery.ColumnsToFetch.Contains(column))
				{
					command.Append(this.outerLegPrefix);
					command.Append(".");
					((ISqlColumn)column).AppendNameToQuery(command);
					return;
				}
				((ISqlColumn)column).AppendExpressionToQuery(this, use, command);
			}

			public override void AppendSelectList(IList<Column> columnsToFetch, SqlCommand command)
			{
				for (int i = 0; i < columnsToFetch.Count; i++)
				{
					if (i != 0)
					{
						command.Append(", ");
					}
					this.AppendSimpleColumnToQuery(columnsToFetch[i], ColumnUse.FetchList, command);
				}
			}

			public override void AppendOrderByList(CultureInfo culture, SortOrder sortOrder, bool reverse, SqlCommand command)
			{
				int count = sortOrder.Count;
				for (int i = 0; i < count; i++)
				{
					if (i != 0)
					{
						command.Append(", ");
					}
					this.AppendSimpleColumnToQuery(sortOrder[i].Column, ColumnUse.OrderBy, command);
					SqlCollationHelper.AppendCollation(sortOrder[i].Column, culture, command);
					if ((!reverse && !sortOrder[i].Ascending) || (reverse && sortOrder[i].Ascending))
					{
						command.Append(" DESC");
					}
				}
			}

			public override void AppendSimpleColumnToQuery(Column column, ColumnUse use, SqlCommand command)
			{
				if (this.joinOperator.OuterQuery.ColumnsToFetch.Contains(column))
				{
					command.Append(this.outerLegPrefix);
					command.Append(".");
				}
				else
				{
					command.Append(this.baseTablePrefix);
					command.Append(".");
				}
				((ISqlColumn)column).AppendNameToQuery(command);
			}

			private SqlJoinOperator joinOperator;

			private string baseTablePrefix;

			private string outerLegPrefix;
		}
	}
}
