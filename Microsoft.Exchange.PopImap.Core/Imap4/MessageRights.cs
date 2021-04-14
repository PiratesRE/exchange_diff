using System;

namespace Microsoft.Exchange.Imap4
{
	[Flags]
	internal enum MessageRights
	{
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
		None = 0,
		ReadOnly = 1,
		ReadWrite = 33,
		All = 1531
	}
}
