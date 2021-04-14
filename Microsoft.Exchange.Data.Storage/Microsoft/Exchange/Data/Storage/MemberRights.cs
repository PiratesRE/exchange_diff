using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum MemberRights
	{
		None = 0,
		ReadAny = 1,
		Create = 2,
		EditOwned = 8,
		DeleteOwned = 16,
		EditAny = 32,
		DeleteAny = 64,
		CreateSubfolder = 128,
		Owner = 256,
		Contact = 512,
		Visible = 1024,
		FreeBusySimple = 2048,
		FreeBusyDetailed = 4096
	}
}
