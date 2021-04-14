using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public sealed class SqlMappedPropertyColumn : MappedPropertyColumn, ISqlColumn
	{
		internal SqlMappedPropertyColumn(Column actualColumn, StorePropTag propTag) : base(actualColumn, propTag)
		{
		}

		public void AppendExpressionToQuery(SqlQueryModel model, ColumnUse use, SqlCommand command)
		{
			((ISqlColumn)this.ActualColumn).AppendExpressionToQuery(model, use, command);
		}

		public void AppendNameToQuery(SqlCommand command)
		{
			command.Append(this.Name);
		}

		public void AppendQueryText(SqlQueryModel model, SqlCommand command)
		{
			((ISqlColumn)this.ActualColumn).AppendQueryText(model, command);
		}
	}
}
