using System;

namespace Microsoft.Mapi
{
	internal enum TableStatus
	{
		Complete,
		QChanged = 7,
		Sorting = 9,
		Sort_Error,
		Setting_Cols,
		SetCol_Error = 13,
		Restricting,
		Restrict_Error
	}
}
