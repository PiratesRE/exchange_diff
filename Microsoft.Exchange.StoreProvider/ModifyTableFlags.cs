using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum ModifyTableFlags
	{
		None = 0,
		RowListReplace = 1,
		FreeBusy = 2
	}
}
