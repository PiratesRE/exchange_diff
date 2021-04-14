using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public sealed class SqlFunctionColumn : FunctionColumn, ISqlColumn
	{
		internal SqlFunctionColumn(string name, Type type, int size, int maxLength, Table table, Func<object[], object> function, string functionName, params Column[] argumentColumns) : base(name, type, size, maxLength, table, function, functionName, argumentColumns)
		{
			if (argumentColumns != null)
			{
				foreach (Column column in argumentColumns)
				{
				}
			}
		}

		public void AppendExpressionToQuery(SqlQueryModel model, ColumnUse use, SqlCommand command)
		{
			command.Append(base.FunctionName);
			command.Append("(");
			if (this.ArgumentColumns != null)
			{
				for (int i = 0; i < this.ArgumentColumns.Length; i++)
				{
					if (i != 0)
					{
						command.Append(", ");
					}
					if (this.ArgumentColumns[i] is ConstantColumn)
					{
						((ISqlColumn)this.ArgumentColumns[i]).AppendQueryText(model, command);
					}
					else
					{
						model.AppendColumnToQuery(this.ArgumentColumns[i], use, command);
					}
				}
			}
			command.Append(")");
		}

		public void AppendNameToQuery(SqlCommand command)
		{
			command.Append(this.Name);
		}

		public void AppendQueryText(SqlQueryModel model, SqlCommand command)
		{
			command.AppendColumn(this, model, ColumnUse.Criteria);
		}
	}
}
