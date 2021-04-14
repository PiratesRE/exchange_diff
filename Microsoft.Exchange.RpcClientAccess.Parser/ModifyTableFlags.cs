using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal enum ModifyTableFlags : byte
	{
		Invalid,
		AddRow,
		ModifyRow,
		RemoveRow = 4
	}
}
