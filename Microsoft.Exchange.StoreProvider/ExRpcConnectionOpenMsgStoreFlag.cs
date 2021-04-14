using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum ExRpcConnectionOpenMsgStoreFlag
	{
		None = 0,
		UseLocaleInfo = 1,
		AuthZ = 2
	}
}
