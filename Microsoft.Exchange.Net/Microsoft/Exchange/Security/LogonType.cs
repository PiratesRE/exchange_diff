using System;

namespace Microsoft.Exchange.Security
{
	internal enum LogonType
	{
		Unknown,
		Interactive = 2,
		Network,
		Batch,
		Service,
		Unlock = 7,
		NetworkCleartext,
		NewCredentials
	}
}
