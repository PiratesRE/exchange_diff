using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public sealed class SqlConstantColumn : ConstantColumn, ISqlColumn
	{
		internal SqlConstantColumn(string name, Type type, Visibility visibility, int size, int maxLength, object value) : base(name, type, visibility, size, maxLength, value)
		{
		}

		public void AppendExpressionToQuery(SqlQueryModel model, ColumnUse use, SqlCommand command)
		{
			command.AppendParameter(base.Value);
		}

		public void AppendNameToQuery(SqlCommand command)
		{
			command.Append(this.Name);
		}

		public void AppendQueryText(SqlQueryModel model, SqlCommand command)
		{
			command.AppendParameter(base.Value);
		}
	}
}
