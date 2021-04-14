using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum SetReadFlags
	{
		None = 0,
		SuppressReceipt = 1,
		ClearRead = 4,
		DeferredErrors = 8,
		GenerateReceiptOnly = 16,
		ClearRnPending = 32,
		CleanNrnPending = 64
	}
}
