using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public interface ISqlColumn
	{
		void AppendExpressionToQuery(SqlQueryModel model, ColumnUse use, SqlCommand command);

		void AppendNameToQuery(SqlCommand command);

		void AppendQueryText(SqlQueryModel model, SqlCommand command);
	}
}
