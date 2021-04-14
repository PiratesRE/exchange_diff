using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	internal enum NavigationNodeFlags
	{
		None = 0,
		PublicFolder = 1,
		PublicFolderFavorite = 2,
		ImapFolder = 4,
		DavFolder = 8,
		SharepointFolder = 16,
		RootFolder = 32,
		FATFolder = 64,
		WebFolder = 128,
		SharedOut = 256,
		SharedIn = 512,
		PersonFolder = 1024,
		IcalFolder = 2048,
		CalendarOverlaid = 4096,
		OneOffName = 8192,
		TodoFolder = 16384,
		IpfNote = 32768,
		IpfDocument = 65536,
		IsDefaultStore = 1048576
	}
}
