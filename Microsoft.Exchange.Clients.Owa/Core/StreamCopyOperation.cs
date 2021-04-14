using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	internal enum StreamCopyOperation
	{
		SyncRead = 1,
		SyncWrite = 2,
		AsyncRead = 4,
		AsyncWrite = 8,
		Read = 5,
		Write = 10
	}
}
