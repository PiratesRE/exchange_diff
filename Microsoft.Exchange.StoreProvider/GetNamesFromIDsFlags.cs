using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum GetNamesFromIDsFlags
	{
		None = 0,
		NoStrings = 1,
		NoIds = 2
	}
}
