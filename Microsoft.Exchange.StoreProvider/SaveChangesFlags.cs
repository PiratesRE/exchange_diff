using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum SaveChangesFlags
	{
		KeepOpenRead = 1,
		KeepOpenReadWrite = 2,
		ForceSave = 4,
		SkipQuotaCheck = 64,
		ChangeIMAPId = 128,
		ForceNotificationPublish = 256
	}
}
