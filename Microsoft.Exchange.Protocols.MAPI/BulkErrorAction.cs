using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	[Flags]
	public enum BulkErrorAction
	{
		Skip = 0,
		Incomplete = 1,
		Error = 2,
		Exception = 3
	}
}
