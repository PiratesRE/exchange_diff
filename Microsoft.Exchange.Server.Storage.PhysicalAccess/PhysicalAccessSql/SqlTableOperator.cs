using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlTableOperator : TableOperator, ISqlSimpleQueryOperator
	{
		internal SqlTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation) : this(connectionProvider, new TableOperator.TableOperatorDefinition(culture, table, index, columnsToFetch, longValueColumnsToPreread, restriction, renameDictionary, skipTo, maxRows, keyRanges, backwards, true, frequentOperation))
		{
		}

		internal SqlTableOperator(IConnectionProvider connectionProvider, TableOperator.TableOperatorDefinition definition) : base(connectionProvider, definition)
		{
		}

		public static IList<Column> RemoveDuplicateColumns(IList<Column> columns)
		{
			HashSet<string> hashSet = new HashSet<string>();
			List<Column> list = new List<Column>(columns.Count);
			foreach (Column column in columns)
			{
				if (!hashSet.Contains(column.Name))
				{
					hashSet.Add(column.Name);
					list.Add(column);
				}
			}
			return list;
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
				sqlCommand.Append(") ");
			}
			this.AppendSelectList(sqlCommand, model, true, orderedResultsNeeded);
			sqlCommand.Append(" FROM [Exchange].[");
			sqlCommand.Append(base.Table.Name);
			sqlCommand.Append("]");
			this.AppendWhereClause(sqlCommand, model);
			if (this.sqlCommand == sqlCommand || base.MaxRows > 0)
			{
				sqlCommand.Append(" ORDER BY ");
				this.AppendOrderByList(sqlCommand);
			}
		}

		private static void AppendKeyRangeCriteria(CultureInfo culture, Connection connection, SqlCommand sqlCommand, SortOrder keySortOrder, bool backwards, KeyRange keyRange, int commonPrefixLength)
		{
			sqlCommand.Append("(");
			if (keyRange.StartKey.Count > commonPrefixLength)
			{
				sqlCommand.Append("(");
				for (int i = commonPrefixLength; i < keyRange.StartKey.Count; i++)
				{
					if (i > commonPrefixLength)
					{
						sqlCommand.Append(" OR ");
					}
					sqlCommand.Append("(");
					for (int j = commonPrefixLength; j < i; j++)
					{
						SqlTableOperator.AppendKeyColumnComparison(culture, sqlCommand, keySortOrder[j].Column, SearchCriteriaCompare.SearchRelOp.Equal, keyRange.StartKey.Values[j]);
						sqlCommand.Append(" AND ");
					}
					bool flag = keySortOrder.Ascending[i];
					if (backwards)
					{
						flag = !flag;
					}
					bool flag2 = i == keyRange.StartKey.Count - 1;
					bool flag3 = flag2 && keyRange.StartKey.Inclusive;
					SearchCriteriaCompare.SearchRelOp relOp = flag ? (flag3 ? SearchCriteriaCompare.SearchRelOp.GreaterThanEqual : SearchCriteriaCompare.SearchRelOp.GreaterThan) : (flag3 ? SearchCriteriaCompare.SearchRelOp.LessThanEqual : SearchCriteriaCompare.SearchRelOp.LessThan);
					SqlTableOperator.AppendKeyColumnComparison(culture, sqlCommand, keySortOrder[i].Column, relOp, keyRange.StartKey.Values[i]);
					sqlCommand.Append(")");
				}
				sqlCommand.Append(")");
			}
			if (keyRange.StopKey.Count > commonPrefixLength)
			{
				if (keyRange.StartKey.Count > commonPrefixLength)
				{
					sqlCommand.Append(" AND ");
				}
				sqlCommand.Append("(");
				for (int k = commonPrefixLength; k < keyRange.StopKey.Count; k++)
				{
					if (k > commonPrefixLength)
					{
						sqlCommand.Append(" OR ");
					}
					sqlCommand.Append("(");
					for (int l = 0; l < k; l++)
					{
						SqlTableOperator.AppendKeyColumnComparison(culture, sqlCommand, keySortOrder[l].Column, SearchCriteriaCompare.SearchRelOp.Equal, keyRange.StopKey.Values[l]);
						sqlCommand.Append(" AND ");
					}
					bool flag4 = keySortOrder.Ascending[k];
					if (backwards)
					{
						flag4 = !flag4;
					}
					flag4 = !flag4;
					bool flag5 = k == keyRange.StopKey.Count - 1;
					bool flag6 = flag5 && keyRange.StopKey.Inclusive;
					SearchCriteriaCompare.SearchRelOp relOp2 = flag4 ? (flag6 ? SearchCriteriaCompare.SearchRelOp.GreaterThanEqual : SearchCriteriaCompare.SearchRelOp.GreaterThan) : (flag6 ? SearchCriteriaCompare.SearchRelOp.LessThanEqual : SearchCriteriaCompare.SearchRelOp.LessThan);
					SqlTableOperator.AppendKeyColumnComparison(culture, sqlCommand, keySortOrder[k].Column, relOp2, keyRange.StopKey.Values[k]);
					sqlCommand.Append(")");
				}
				sqlCommand.Append(")");
			}
			sqlCommand.Append(")");
		}

		private static int AppendCommonKeyRangeCriteria(CultureInfo culture, Connection connection, SqlCommand sqlCommand, SortOrder keySortOrder, IList<KeyRange> keyRanges)
		{
			int num = 0;
			CompareInfo compareInfo = (culture == null) ? null : culture.CompareInfo;
			for (int i = 0; i < keyRanges.Count; i++)
			{
				int num2 = StartStopKey.CommonKeyPrefix(keyRanges[i].StartKey, keyRanges[i].StopKey, compareInfo);
				if (i == 0)
				{
					num = num2;
				}
				else
				{
					num = Math.Min(num, num2);
					num2 = StartStopKey.CommonKeyPrefix(keyRanges[i].StartKey, keyRanges[i - 1].StopKey, compareInfo);
					num = Math.Min(num, num2);
				}
			}
			for (int j = 0; j < num; j++)
			{
				if (j != 0)
				{
					sqlCommand.Append(" AND ");
				}
				Column column = keySortOrder[j].Column;
				SearchCriteriaCompare.SearchRelOp relOp = SearchCriteriaCompare.SearchRelOp.Equal;
				StartStopKey startKey = keyRanges[0].StartKey;
				SqlTableOperator.AppendKeyColumnComparison(culture, sqlCommand, column, relOp, startKey.Values[j]);
			}
			return num;
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
				model.AppendColumnToQuery(column, ColumnUse.FetchList, sqlCommand);
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

		public void AppendSelectList(SqlCommand sqlCommand, SqlQueryModel model, bool orderedResultsNeeded)
		{
			this.AppendSelectList(sqlCommand, model, false, orderedResultsNeeded);
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
			return DisposeTracker.Get<SqlTableOperator>(this);
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
			this.BuildSqlStatement(this.sqlCommand, true);
			this.sqlCommand.AppendQueryHints(base.FrequentOperation);
		}

		private void AppendWhereClause(SqlCommand sqlCommand, SqlQueryModel model)
		{
			if (base.KeyRanges.Count == 0)
			{
				sqlCommand.Append(" WHERE (0 = 1) ");
				return;
			}
			StartStopKey startKey = base.KeyRanges[0].StartKey;
			if (startKey.IsEmpty)
			{
				StartStopKey stopKey = base.KeyRanges[0].StopKey;
				if (stopKey.IsEmpty && base.Criteria == null)
				{
					return;
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
				this.AppendCriteria(sqlCommand, model, base.Criteria);
				sqlCommand.Append(")");
			}
		}

		private void AppendOrderByList(SqlCommand sqlCommand)
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

		private void AppendCriteria(SqlCommand sqlCommand, SqlQueryModel model, SearchCriteria restriction)
		{
			ISqlSearchCriteria sqlSearchCriteria = restriction as ISqlSearchCriteria;
			sqlSearchCriteria.AppendQueryText(base.Culture, model, sqlCommand);
		}

		internal static void AppendKeyColumnComparison(CultureInfo culture, SqlCommand sqlCommand, Column column, SearchCriteriaCompare.SearchRelOp relOp, object value)
		{
			switch (relOp)
			{
			case SearchCriteriaCompare.SearchRelOp.Equal:
				if (value == null)
				{
					if (!column.IsNullable)
					{
						sqlCommand.Append("1=0");
						return;
					}
					sqlCommand.Append(column.Name);
					sqlCommand.Append(" IS NULL");
					return;
				}
				else
				{
					if (column.IsNullable)
					{
						sqlCommand.Append("(");
						sqlCommand.Append(column.Name);
						sqlCommand.Append(" IS NOT NULL AND ");
					}
					sqlCommand.Append(column.Name);
					sqlCommand.Append("=");
					sqlCommand.AppendParameter(value);
					string collation = SqlCollationHelper.GetCollation(column.Type, culture);
					if (collation != null)
					{
						sqlCommand.Append(" COLLATE ");
						sqlCommand.Append(collation);
					}
					if (column.IsNullable)
					{
						sqlCommand.Append(")");
						return;
					}
				}
				break;
			case SearchCriteriaCompare.SearchRelOp.NotEqual:
				if (value == null)
				{
					if (!column.IsNullable)
					{
						sqlCommand.Append("1=1");
						return;
					}
					sqlCommand.Append(column.Name);
					sqlCommand.Append(" IS NOT NULL");
					return;
				}
				else
				{
					if (column.IsNullable)
					{
						sqlCommand.Append("(");
						sqlCommand.Append(column.Name);
						sqlCommand.Append(" IS NULL OR ");
					}
					sqlCommand.Append(column.Name);
					sqlCommand.Append("<>");
					sqlCommand.AppendParameter(value);
					string collation2 = SqlCollationHelper.GetCollation(column.Type, culture);
					if (collation2 != null)
					{
						sqlCommand.Append(" COLLATE ");
						sqlCommand.Append(collation2);
					}
					if (column.IsNullable)
					{
						sqlCommand.Append(")");
						return;
					}
				}
				break;
			case SearchCriteriaCompare.SearchRelOp.LessThan:
			{
				if (value == null)
				{
					sqlCommand.Append("1=0");
					return;
				}
				if (column.IsNullable)
				{
					sqlCommand.Append("(");
					sqlCommand.Append(column.Name);
					sqlCommand.Append(" IS NULL OR ");
				}
				sqlCommand.Append(column.Name);
				sqlCommand.Append("<");
				sqlCommand.AppendParameter(value);
				string collation3 = SqlCollationHelper.GetCollation(column.Type, culture);
				if (collation3 != null)
				{
					sqlCommand.Append(" COLLATE ");
					sqlCommand.Append(collation3);
				}
				if (column.IsNullable)
				{
					sqlCommand.Append(")");
					return;
				}
				break;
			}
			case SearchCriteriaCompare.SearchRelOp.LessThanEqual:
				if (value == null)
				{
					if (!column.IsNullable)
					{
						sqlCommand.Append("1=0");
						return;
					}
					sqlCommand.Append(column.Name);
					sqlCommand.Append(" IS NULL");
					return;
				}
				else
				{
					if (column.IsNullable)
					{
						sqlCommand.Append("(");
						sqlCommand.Append(column.Name);
						sqlCommand.Append(" IS NULL OR ");
					}
					sqlCommand.Append(column.Name);
					sqlCommand.Append("<=");
					sqlCommand.AppendParameter(value);
					string collation4 = SqlCollationHelper.GetCollation(column.Type, culture);
					if (collation4 != null)
					{
						sqlCommand.Append(" COLLATE ");
						sqlCommand.Append(collation4);
					}
					if (column.IsNullable)
					{
						sqlCommand.Append(")");
						return;
					}
				}
				break;
			case SearchCriteriaCompare.SearchRelOp.GreaterThan:
				if (value == null)
				{
					if (!column.IsNullable)
					{
						sqlCommand.Append("1=1");
						return;
					}
					sqlCommand.Append(column.Name);
					sqlCommand.Append(" IS NOT NULL");
					return;
				}
				else
				{
					if (column.IsNullable)
					{
						sqlCommand.Append("(");
						sqlCommand.Append(column.Name);
						sqlCommand.Append(" IS NOT NULL AND ");
					}
					sqlCommand.Append(column.Name);
					sqlCommand.Append(">");
					sqlCommand.AppendParameter(value);
					string collation5 = SqlCollationHelper.GetCollation(column.Type, culture);
					if (collation5 != null)
					{
						sqlCommand.Append(" COLLATE ");
						sqlCommand.Append(collation5);
					}
					if (column.IsNullable)
					{
						sqlCommand.Append(")");
						return;
					}
				}
				break;
			case SearchCriteriaCompare.SearchRelOp.GreaterThanEqual:
			{
				if (value == null)
				{
					sqlCommand.Append("1=1");
					return;
				}
				if (column.IsNullable)
				{
					sqlCommand.Append("(");
					sqlCommand.Append(column.Name);
					sqlCommand.Append(" IS NOT NULL AND ");
				}
				sqlCommand.Append(column.Name);
				sqlCommand.Append(">=");
				sqlCommand.AppendParameter(value);
				string collation6 = SqlCollationHelper.GetCollation(column.Type, culture);
				if (collation6 != null)
				{
					sqlCommand.Append(" COLLATE ");
					sqlCommand.Append(collation6);
				}
				if (column.IsNullable)
				{
					sqlCommand.Append(")");
				}
				break;
			}
			default:
				return;
			}
		}

		internal static void AppendKeyRangeCriteria(CultureInfo culture, Connection connection, SqlCommand sqlCommand, SortOrder keySortOrder, bool backwards, IList<KeyRange> keyRanges)
		{
			sqlCommand.Append("(");
			bool flag = false;
			bool flag2 = false;
			int num = SqlTableOperator.AppendCommonKeyRangeCriteria(culture, connection, sqlCommand, keySortOrder, keyRanges);
			int i = 0;
			while (i < keyRanges.Count)
			{
				StartStopKey startKey = keyRanges[i].StartKey;
				if (startKey.Count > num)
				{
					goto IL_54;
				}
				StartStopKey stopKey = keyRanges[i].StopKey;
				if (stopKey.Count > num)
				{
					goto IL_54;
				}
				IL_A4:
				i++;
				continue;
				IL_54:
				if (!flag && num > 0)
				{
					sqlCommand.Append(" AND (");
					flag = true;
				}
				else if (flag2)
				{
					sqlCommand.Append(" OR ");
				}
				sqlCommand.Append("(");
				SqlTableOperator.AppendKeyRangeCriteria(culture, connection, sqlCommand, keySortOrder, backwards, keyRanges[i], num);
				sqlCommand.Append(")");
				flag2 = true;
				goto IL_A4;
			}
			if (flag)
			{
				sqlCommand.Append(")");
			}
			sqlCommand.Append(")");
		}

		private SqlCommand sqlCommand;
	}
}
