using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal enum FastTransferState : ushort
	{
		Error,
		Partial,
		NoRoom,
		Done
	}
}
