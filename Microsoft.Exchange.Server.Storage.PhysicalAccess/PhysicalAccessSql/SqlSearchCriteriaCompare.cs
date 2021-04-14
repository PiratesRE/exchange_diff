using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlSearchCriteriaCompare : SearchCriteriaCompare, ISqlSearchCriteria
	{
		public SqlSearchCriteriaCompare(Column lhs, SearchCriteriaCompare.SearchRelOp op, Column rhs) : base(lhs, op, rhs)
		{
		}

		public void AppendQueryText(CultureInfo culture, SqlQueryModel model, SqlCommand command)
		{
			Column column = base.Lhs;
			Column column2 = base.Rhs;
			SearchCriteriaCompare.SearchRelOp op = base.RelOp;
			if (column is ConstantColumn && (!(column2 is ConstantColumn) || ((ConstantColumn)column).Value == null))
			{
				column = base.Rhs;
				column2 = base.Lhs;
				op = base.InvertSearchRelOp();
			}
			if (!(column2 is ConstantColumn) || ((ConstantColumn)column2).Value != null)
			{
				bool flag = false;
				if (column.IsNullable || column2.IsNullable)
				{
					command.Append("(");
					switch (op)
					{
					case SearchCriteriaCompare.SearchRelOp.Equal:
						if (column.IsNullable && column2.IsNullable)
						{
							command.Append("(");
							((ISqlColumn)column).AppendQueryText(model, command);
							command.Append(" IS NULL AND ");
							((ISqlColumn)column2).AppendQueryText(model, command);
							command.Append(" IS NULL) OR (");
							((ISqlColumn)column).AppendQueryText(model, command);
							command.Append(" IS NOT NULL AND ");
							((ISqlColumn)column2).AppendQueryText(model, command);
							command.Append(" IS NOT NULL AND ");
							flag = true;
						}
						else if (column2.IsNullable)
						{
							((ISqlColumn)column2).AppendQueryText(model, command);
							command.Append(" IS NOT NULL AND ");
						}
						else
						{
							((ISqlColumn)column).AppendQueryText(model, command);
							command.Append(" IS NOT NULL AND ");
						}
						break;
					case SearchCriteriaCompare.SearchRelOp.NotEqual:
						if (column.IsNullable && column2.IsNullable)
						{
							command.Append("(");
							((ISqlColumn)column).AppendQueryText(model, command);
							command.Append(" IS NULL AND ");
							((ISqlColumn)column2).AppendQueryText(model, command);
							command.Append(" IS NOT NULL) OR (");
							((ISqlColumn)column).AppendQueryText(model, command);
							command.Append(" IS NOT NULL AND ");
							((ISqlColumn)column2).AppendQueryText(model, command);
							command.Append(" IS NULL) OR (");
							((ISqlColumn)column).AppendQueryText(model, command);
							command.Append(" IS NOT NULL AND ");
							((ISqlColumn)column2).AppendQueryText(model, command);
							command.Append(" IS NOT NULL AND ");
							flag = true;
						}
						else if (column2.IsNullable)
						{
							((ISqlColumn)column2).AppendQueryText(model, command);
							command.Append(" IS NULL OR ");
						}
						else
						{
							((ISqlColumn)column).AppendQueryText(model, command);
							command.Append(" IS NULL OR ");
						}
						break;
					case SearchCriteriaCompare.SearchRelOp.LessThan:
						if (column.IsNullable && column2.IsNullable)
						{
							((ISqlColumn)column2).AppendQueryText(model, command);
							command.Append(" IS NOT NULL AND (");
							((ISqlColumn)column).AppendQueryText(model, command);
							command.Append(" IS NULL OR ");
							flag = true;
						}
						else if (column2.IsNullable)
						{
							((ISqlColumn)column2).AppendQueryText(model, command);
							command.Append(" IS NOT NULL AND ");
						}
						else
						{
							((ISqlColumn)column).AppendQueryText(model, command);
							command.Append(" IS NULL OR ");
						}
						break;
					case SearchCriteriaCompare.SearchRelOp.LessThanEqual:
						if (column.IsNullable && column2.IsNullable)
						{
							((ISqlColumn)column).AppendQueryText(model, command);
							command.Append(" IS NULL OR (");
							((ISqlColumn)column2).AppendQueryText(model, command);
							command.Append(" IS NOT NULL AND ");
							flag = true;
						}
						else if (column2.IsNullable)
						{
							((ISqlColumn)column2).AppendQueryText(model, command);
							command.Append(" IS NOT NULL AND ");
						}
						else
						{
							((ISqlColumn)column).AppendQueryText(model, command);
							command.Append(" IS NULL OR ");
						}
						break;
					case SearchCriteriaCompare.SearchRelOp.GreaterThan:
						if (column.IsNullable && column2.IsNullable)
						{
							((ISqlColumn)column).AppendQueryText(model, command);
							command.Append(" IS NOT NULL AND (");
							((ISqlColumn)column2).AppendQueryText(model, command);
							command.Append(" IS NULL OR ");
							flag = true;
						}
						else if (column2.IsNullable)
						{
							((ISqlColumn)column2).AppendQueryText(model, command);
							command.Append(" IS NULL OR ");
						}
						else
						{
							((ISqlColumn)column).AppendQueryText(model, command);
							command.Append(" IS NOT NULL AND ");
						}
						break;
					case SearchCriteriaCompare.SearchRelOp.GreaterThanEqual:
						if (column.IsNullable && column2.IsNullable)
						{
							((ISqlColumn)column2).AppendQueryText(model, command);
							command.Append(" IS NULL OR (");
							((ISqlColumn)column).AppendQueryText(model, command);
							command.Append(" IS NOT NULL AND ");
							flag = true;
						}
						else if (column2.IsNullable)
						{
							((ISqlColumn)column2).AppendQueryText(model, command);
							command.Append(" IS NULL OR ");
						}
						else
						{
							((ISqlColumn)column).AppendQueryText(model, command);
							command.Append(" IS NOT NULL AND ");
						}
						break;
					}
				}
				((ISqlColumn)column).AppendQueryText(model, command);
				SearchCriteriaCompare.RelOpAsString(op, command.Sb);
				((ISqlColumn)column2).AppendQueryText(model, command);
				SqlCollationHelper.AppendCollation(column, culture, command);
				if (column.IsNullable || column2.IsNullable)
				{
					if (flag)
					{
						command.Append(")");
					}
					command.Append(")");
				}
				return;
			}
			switch (op)
			{
			case SearchCriteriaCompare.SearchRelOp.Equal:
			case SearchCriteriaCompare.SearchRelOp.LessThanEqual:
				if (column.IsNullable)
				{
					((ISqlColumn)column).AppendQueryText(model, command);
					command.Append(" IS NULL");
					return;
				}
				command.Append("1 = 0");
				return;
			case SearchCriteriaCompare.SearchRelOp.NotEqual:
			case SearchCriteriaCompare.SearchRelOp.GreaterThan:
				if (column.IsNullable)
				{
					((ISqlColumn)column).AppendQueryText(model, command);
					command.Append(" IS NOT NULL");
					return;
				}
				command.Append("1 = 1");
				return;
			case SearchCriteriaCompare.SearchRelOp.LessThan:
				command.Append("1 = 0");
				return;
			case SearchCriteriaCompare.SearchRelOp.GreaterThanEqual:
				command.Append("1 = 1");
				return;
			default:
				return;
			}
		}
	}
}
