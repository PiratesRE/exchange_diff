using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LogTableSchema<T>
	{
		public LogTableSchema(string columnName, Func<T, string> getter)
		{
			this.ColumnName = columnName;
			this.Getter = getter;
		}

		public string ColumnName { get; private set; }

		public Func<T, string> Getter { get; private set; }
	}
}
