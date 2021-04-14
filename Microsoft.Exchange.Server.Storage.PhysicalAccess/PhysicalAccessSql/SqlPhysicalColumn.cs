using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlPhysicalColumn : PhysicalColumn, ISqlColumn
	{
		internal SqlPhysicalColumn(string name, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, bool schemaExtension, Visibility visibility, int maxLength, int size, Table table, int index, int maxInlineLength) : base(name, name, type, nullable, identity, streamSupport, notFetchedByDefault, schemaExtension, visibility, maxLength, size, table, index, maxInlineLength)
		{
		}

		internal SqlPhysicalColumn(string name, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, Visibility visibility, int maxLength, int size, Table table, int index, int maxInlineLength) : base(name, name, type, nullable, identity, streamSupport, notFetchedByDefault, visibility, maxLength, size, table, index, maxInlineLength)
		{
		}

		public virtual void AppendExpressionToQuery(SqlQueryModel model, ColumnUse use, SqlCommand command)
		{
			model.AppendSimpleColumnToQuery(this, use, command);
		}

		public virtual void AppendNameToQuery(SqlCommand command)
		{
			command.Append(this.name);
		}

		public virtual void AppendQueryText(SqlQueryModel model, SqlCommand command)
		{
			command.AppendColumn(this, model, ColumnUse.Criteria);
		}
	}
}
