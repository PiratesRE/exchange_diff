using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal enum WacRequestType
	{
		Unknown,
		CheckFile,
		GetFile,
		Lock,
		UnLock,
		RefreshLock,
		UnlockAndRelock,
		PutFile,
		Cobalt,
		DeleteFile
	}
}
