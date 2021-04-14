using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	[Flags]
	public enum StreamFlags
	{
		AllowCreate = 1,
		AllowAppend = 2,
		AllowRead = 4,
		AllowWrite = 8
	}
}
