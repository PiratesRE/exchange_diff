using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SingleTableQueryModel : SqlQueryModel
	{
		public SingleTableQueryModel(string viewName) : this(viewName, null, SqlQueryModel.IsolationLevelHint.ReadCommitted, SqlQueryModel.PlanCacheHint.CachePlan, false)
		{
		}

		public SingleTableQueryModel(string viewName, bool allowCiSearch) : this(viewName, null, SqlQueryModel.IsolationLevelHint.ReadCommitted, SqlQueryModel.PlanCacheHint.CachePlan, allowCiSearch)
		{
		}

		public SingleTableQueryModel(string viewName, string tablePrefix) : this(viewName, tablePrefix, SqlQueryModel.IsolationLevelHint.ReadCommitted, SqlQueryModel.PlanCacheHint.CachePlan, false)
		{
		}

		public SingleTableQueryModel(string viewName, string tablePrefix, SqlQueryModel.IsolationLevelHint isolationLevelHint, SqlQueryModel.PlanCacheHint planCacheHint) : this(viewName, tablePrefix, isolationLevelHint, planCacheHint, false)
		{
		}

		public SingleTableQueryModel(string viewName, string tablePrefix, SqlQueryModel.IsolationLevelHint isolationLevelHint, SqlQueryModel.PlanCacheHint planCacheHint, bool allowCiSearch)
		{
			this.viewName = viewName;
			this.tablePrefix = tablePrefix;
			this.isolationLevelHint = isolationLevelHint;
			this.allowCiSearch = allowCiSearch;
		}

		public string ViewName
		{
			get
			{
				return this.viewName;
			}
		}

		public string TablePrefix
		{
			get
			{
				return this.tablePrefix;
			}
		}

		public override string BaseTablePrefix
		{
			get
			{
				return this.TablePrefix;
			}
		}

		public SqlQueryModel.IsolationLevelHint IsolationLevel
		{
			get
			{
				return this.isolationLevelHint;
			}
			set
			{
				this.isolationLevelHint = value;
			}
		}

		public override bool AllowCiSearch
		{
			get
			{
				return this.allowCiSearch;
			}
		}

		public override void AppendFromList(SqlCommand command)
		{
			command.Append("[Exchange].[");
			command.Append(this.viewName);
			command.Append("]");
			if (this.tablePrefix != null)
			{
				command.Append(" AS ");
				command.Append(this.tablePrefix);
			}
			if (this.isolationLevelHint == SqlQueryModel.IsolationLevelHint.ReadPast)
			{
				command.Append(" WITH(READPAST)");
				return;
			}
			if (this.isolationLevelHint == SqlQueryModel.IsolationLevelHint.ReadUncommitted)
			{
				command.Append(" WITH(READUNCOMMITTED)");
			}
		}

		public override void AppendSelectList(IList<Column> columnsToFetch, SqlCommand command)
		{
			for (int i = 0; i < columnsToFetch.Count; i++)
			{
				if (i != 0)
				{
					command.Append(", ");
				}
				Column column = columnsToFetch[i];
				command.AppendColumn(column, this, ColumnUse.FetchList);
				command.Append(" AS ");
				((ISqlColumn)column).AppendNameToQuery(command);
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
				command.AppendColumn(sortOrder[i].Column, this, ColumnUse.OrderBy);
				SqlCollationHelper.AppendCollation(sortOrder[i].Column, culture, command);
				if ((!reverse && !sortOrder[i].Ascending) || (reverse && sortOrder[i].Ascending))
				{
					command.Append(" DESC");
				}
			}
		}

		public override void AppendColumnToQuery(Column column, ColumnUse use, SqlCommand command)
		{
			((ISqlColumn)column).AppendExpressionToQuery(this, use, command);
		}

		public override void AppendSimpleColumnToQuery(Column column, ColumnUse use, SqlCommand command)
		{
			if (this.tablePrefix != null)
			{
				command.Append(this.tablePrefix);
				command.Append(".");
			}
			((ISqlColumn)column).AppendNameToQuery(command);
		}

		private string viewName;

		private string tablePrefix;

		private SqlQueryModel.IsolationLevelHint isolationLevelHint;

		private bool allowCiSearch;
	}
}
