using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public sealed class SqlConversionColumn : ConversionColumn, ISqlColumn
	{
		internal SqlConversionColumn(string name, Type type, int size, int maxLength, Table table, Func<object, object> conversionFunction, string functionName, Column argumentColumn) : base(name, type, size, maxLength, table, conversionFunction, functionName, argumentColumn)
		{
		}

		public void AppendExpressionToQuery(SqlQueryModel model, ColumnUse use, SqlCommand command)
		{
			command.Append(base.FunctionName);
			command.Append("(");
			model.AppendColumnToQuery(base.ArgumentColumn, use, command);
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
