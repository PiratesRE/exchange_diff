using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SingleTableColumnRenameQueryModel : SingleTableQueryModel
	{
		public SingleTableColumnRenameQueryModel(string viewName, IReadOnlyDictionary<Column, Column> renameDictionary) : base(viewName)
		{
			this.renameDictionary = renameDictionary;
		}

		public override void AppendColumnToQuery(Column column, ColumnUse use, SqlCommand command)
		{
			Column column2;
			if (this.renameDictionary != null && this.renameDictionary.TryGetValue(column, out column2))
			{
				((ISqlColumn)column2).AppendExpressionToQuery(this, use, command);
				return;
			}
			((ISqlColumn)column).AppendExpressionToQuery(this, use, command);
		}

		private IReadOnlyDictionary<Column, Column> renameDictionary;
	}
}
