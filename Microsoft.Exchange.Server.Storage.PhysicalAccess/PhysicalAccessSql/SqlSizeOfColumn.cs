using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public sealed class SqlSizeOfColumn : SizeOfColumn, ISqlColumn
	{
		internal SqlSizeOfColumn(string name, Column termColumn, bool compressedSize) : base(name, termColumn, compressedSize)
		{
		}

		public void AppendExpressionToQuery(SqlQueryModel model, ColumnUse use, SqlCommand command)
		{
			command.Append("ISNULL(CAST(DATALENGTH(");
			((ISqlColumn)base.TermColumn).AppendExpressionToQuery(model, use, command);
			command.Append(") AS int), 0)");
		}

		public void AppendNameToQuery(SqlCommand command)
		{
			command.Append(this.Name);
		}

		public void AppendQueryText(SqlQueryModel model, SqlCommand command)
		{
			command.Append("ISNULL(CAST(DATALENGTH(");
			((ISqlColumn)base.TermColumn).AppendQueryText(model, command);
			command.Append(") AS int), 0)");
		}

		private const string DatalengthExpressionPrefix = "ISNULL(CAST(DATALENGTH(";

		private const string DatalengthExpressionSuffix = ") AS int), 0)";
	}
}
