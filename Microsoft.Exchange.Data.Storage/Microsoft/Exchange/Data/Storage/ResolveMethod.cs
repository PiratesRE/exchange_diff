using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ResolveMethod
	{
		Default = 0,
		LastWriterWins = 1,
		NoConflictNotification = 2
	}
}
