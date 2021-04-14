using System;

namespace Microsoft.Exchange.Data.Storage
{
	public enum BusyType
	{
		Unknown = -1,
		Free,
		Tentative,
		Busy,
		OOF,
		WorkingElseWhere
	}
}
