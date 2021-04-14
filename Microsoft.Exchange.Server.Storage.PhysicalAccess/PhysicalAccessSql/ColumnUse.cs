using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public enum ColumnUse : uint
	{
		OrderBy,
		Criteria,
		FetchList
	}
}
