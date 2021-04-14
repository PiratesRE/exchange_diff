using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal enum TableStatus : byte
	{
		Complete,
		QChanged = 7,
		Sorting = 9,
		SortError,
		SettingColumns,
		SetColumnsError = 13,
		Restricting,
		RestrictError
	}
}
