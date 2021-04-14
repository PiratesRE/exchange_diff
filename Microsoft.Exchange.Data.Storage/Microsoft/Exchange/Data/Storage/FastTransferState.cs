using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum FastTransferState : ushort
	{
		Error,
		Partial,
		NoRoom,
		Done
	}
}
