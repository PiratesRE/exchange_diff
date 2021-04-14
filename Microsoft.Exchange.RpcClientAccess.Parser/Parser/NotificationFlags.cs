using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum NotificationFlags : ushort
	{
		CriticalError = 1,
		NewMail = 2,
		ObjectCreated = 4,
		ObjectDeleted = 8,
		ObjectModified = 16,
		ObjectMoved = 32,
		ObjectCopied = 64,
		SearchComplete = 128,
		TableModified = 256,
		Ics = 512,
		Extended = 1024
	}
}
