using System;
using System.Runtime.InteropServices;

namespace Microsoft.Mapi.Security
{
	[Flags]
	[ComVisible(false)]
	internal enum ExchangeFolderFreeBusyRights
	{
		None = 0,
		Simple = 1,
		Detailed = 2,
		All = 3
	}
}
