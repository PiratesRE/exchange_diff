using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum ModifyPermissionsFlags : byte
	{
		None = 0,
		ReplaceRows = 1,
		IncludeFreeBusy = 2
	}
}
