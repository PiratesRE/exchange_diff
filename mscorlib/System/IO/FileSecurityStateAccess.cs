using System;

namespace System.IO
{
	[Flags]
	internal enum FileSecurityStateAccess
	{
		NoAccess = 0,
		Read = 1,
		Write = 2,
		Append = 4,
		PathDiscovery = 8,
		AllAccess = 15
	}
}
