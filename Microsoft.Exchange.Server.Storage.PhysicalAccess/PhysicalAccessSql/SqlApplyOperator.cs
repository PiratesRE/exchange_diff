using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlApplyOperator : ApplyOperator, ISqlSimpleQueryOperator
	{
		internal SqlApplyOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableFunction tableFunction, IList<Column> tableFunctionParameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, SimpleQueryOperator outerQuery, bool frequentOperation) : this(connectionProvider, new ApplyOperator.ApplyOperatorDefinition(culture, tableFunction, tableFunctionParameters, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, outerQuery.OperatorDefinition, frequentOperation))
		{
		}

		internal SqlApplyOperator(IConnectionProvider connectionProvider, ApplyOperator.ApplyOperatorDefinition definition) : base(connectionProvider, definition)
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

		public void BuildCteForSqlStatement(SqlCommand sqlCommand)
		{
			ISqlSimpleQueryOperator sqlSimpleQueryOperator = (ISqlSimpleQueryOperator)base.OuterQuery;
			if (sqlSimpleQueryOperator.NeedCteForSqlStatement())
			{
				sqlSimpleQueryOperator.BuildCteForSqlStatement(sqlCommand);
				sqlCommand.Append(", ");
			}
			SqlQueryModel model = new SqlApplyOperator.ApplyQueryModel(this, "bt", "ol");
			sqlCommand.Append("applyDrivingLeg AS (");
			sqlSimpleQueryOperator.BuildSqlStatement(sqlCommand, true);
			sqlCommand.Append("), applyInnerLeg AS (SELECT ");
			this.AppendSelectList(sqlCommand, model, true);
			sqlCommand.Append(" FROM ");
			sqlCommand.AppendFromList(model);
			this.AppendWhereClause(sqlCommand, model);
			sqlCommand.Append(") ");
		}

		public bool NeedCteForSqlStatement()
		{
			return true;
		}

		public void BuildSqlStatement(SqlCommand sqlCommand, bool orderedResultsNeeded)
		{
			sqlCommand.Append("SELECT ");
			if (base.MaxRows != 0)
			{
				sqlCommand.Append(" TOP(");
				sqlCommand.Append((base.SkipTo + base.MaxRows).ToString());
				sqlCommand.Append(")");
			}
			this.AppendSelectList(sqlCommand, SqlQueryModel.Shorthand, orderedResultsNeeded);
			sqlCommand.Append(" FROM applyInnerLeg");
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
			return DisposeTracker.Get<SqlApplyOperator>(this);
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
			this.BuildSqlStatement(this.sqlCommand, true);
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

		private void AppendCriteria(SqlCommand sqlCommand, SqlQueryModel model, SearchCriteria restriction)
		{
			ISqlSearchCriteria sqlSearchCriteria = (ISqlSearchCriteria)restriction;
			sqlSearchCriteria.AppendQueryText(base.Culture, model, sqlCommand);
		}

		private SqlCommand sqlCommand;

		private class ApplyQueryModel : SqlQueryModel
		{
			public ApplyQueryModel(SqlApplyOperator applyOperator, string baseTablePrefix, string outerLegPrefix)
			{
				this.applyOperator = applyOperator;
				this.baseTablePrefix = baseTablePrefix;
				this.outerLegPrefix = outerLegPrefix;
			}

			public override void AppendFromList(SqlCommand command)
			{
				command.Append("applyDrivingLeg AS ");
				command.Append(this.outerLegPrefix);
				command.Append(" CROSS APPLY [Exchange].[");
				command.Append(this.applyOperator.TableFunction.Name);
				command.Append("](");
				for (int i = 0; i < this.applyOperator.TableFunctionParameters.Count; i++)
				{
					if (i > 0)
					{
						command.Append(", ");
					}
					command.Append(this.outerLegPrefix);
					command.Append(".");
					command.Append(this.applyOperator.TableFunctionParameters[i].Name);
				}
				command.Append(") AS ");
				command.Append(this.baseTablePrefix);
			}

			public override void AppendColumnToQuery(Column column, ColumnUse use, SqlCommand command)
			{
				if (this.applyOperator.RenameDictionary != null)
				{
					column = this.applyOperator.ResolveColumn(column);
				}
				if (this.applyOperator.OuterQuery.ColumnsToFetch.Contains(column))
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
					if ((!reverse && !sortOrder[i].Ascending) || (reverse && sortOrder[i].Ascending))
					{
						command.Append(" DESC");
					}
				}
			}

			public override void AppendSimpleColumnToQuery(Column column, ColumnUse use, SqlCommand command)
			{
				if (this.applyOperator.RenameDictionary != null)
				{
					column = this.applyOperator.ResolveColumn(column);
				}
				if (this.applyOperator.OuterQuery.ColumnsToFetch.Contains(column))
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

			private SqlApplyOperator applyOperator;

			private string baseTablePrefix;

			private string outerLegPrefix;
		}
	}
}
