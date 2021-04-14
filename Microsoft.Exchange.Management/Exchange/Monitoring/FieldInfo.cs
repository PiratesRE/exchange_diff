using System;

namespace Microsoft.Exchange.Monitoring
{
	internal struct FieldInfo
	{
		public FieldInfo(int columnNumber, string columnName)
		{
			this.ColumnName = columnName;
			this.ColumnNumber = columnNumber;
		}

		internal readonly string ColumnName;

		internal readonly int ColumnNumber;
	}
}
