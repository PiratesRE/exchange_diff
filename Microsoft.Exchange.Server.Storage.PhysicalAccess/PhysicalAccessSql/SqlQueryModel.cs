using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public abstract class SqlQueryModel
	{
		public abstract void AppendFromList(SqlCommand command);

		public abstract void AppendSelectList(IList<Column> columnsToFetch, SqlCommand command);

		public abstract void AppendOrderByList(CultureInfo culture, SortOrder sortOrder, bool reverse, SqlCommand command);

		public abstract void AppendColumnToQuery(Column column, ColumnUse use, SqlCommand command);

		public abstract void AppendSimpleColumnToQuery(Column column, ColumnUse use, SqlCommand command);

		public virtual bool AllowCiSearch
		{
			get
			{
				return false;
			}
		}

		public virtual string BaseTablePrefix
		{
			get
			{
				return null;
			}
		}

		public static SqlQueryModel Shorthand = new SqlQueryModel.ShorthandQueryModel();

		public enum PlanCacheHint
		{
			CachePlan,
			DoNotCachePlan
		}

		public enum IsolationLevelHint
		{
			ReadCommitted,
			ReadPast,
			ReadUncommitted
		}

		private sealed class ShorthandQueryModel : SqlQueryModel
		{
			public override void AppendFromList(SqlCommand command)
			{
			}

			public override void AppendSelectList(IList<Column> columnsToFetch, SqlCommand command)
			{
				for (int i = 0; i < columnsToFetch.Count; i++)
				{
					if (i != 0)
					{
						command.Append(", ");
					}
					((ISqlColumn)columnsToFetch[i]).AppendNameToQuery(command);
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
					((ISqlColumn)sortOrder[i].Column).AppendNameToQuery(command);
					SqlCollationHelper.AppendCollation(sortOrder[i].Column, culture, command);
					if ((!reverse && !sortOrder[i].Ascending) || (reverse && sortOrder[i].Ascending))
					{
						command.Append(" DESC");
					}
				}
			}

			public override void AppendColumnToQuery(Column column, ColumnUse use, SqlCommand command)
			{
				this.AppendSimpleColumnToQuery(column, use, command);
			}

			public override void AppendSimpleColumnToQuery(Column column, ColumnUse use, SqlCommand command)
			{
				((ISqlColumn)column).AppendNameToQuery(command);
			}
		}
	}
}
