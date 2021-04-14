using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum OpenMode : byte
	{
		ReadOnly = 0,
		ReadWrite = 1,
		Create = 2,
		BestAccess = 3,
		OpenSoftDeleted = 4,
		Append = 4,
		NoBlock = 8
	}
}
