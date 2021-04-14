using System;
using System.Runtime.InteropServices;

namespace Microsoft.Mapi.Security
{
	[ComVisible(false)]
	[Flags]
	internal enum ExchangeRights
	{
		None = 0,
		fReadAny = 1,
		fCreate = 2,
		fEditOwned = 8,
		fDeleteOwned = 16,
		fEditAny = 32,
		fDeleteAny = 64,
		fCreateSubfolder = 128,
		fOwner = 256,
		fContact = 512,
		fVisible = 1024,
		frightsFreeBusySimple = 2048,
		frightsFreeBusyDetailed = 4096,
		ReadWrite = 33,
		AllRights = 1531,
		AllFreeBusyRights = 6144
	}
}
