using System;

namespace Microsoft.Mapi
{
	internal enum FastTransferState : ushort
	{
		Error,
		Partial,
		NoRoom,
		Done
	}
}
