using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	[Flags]
	internal enum NavigationTreeDirtyFlag
	{
		None = 0,
		Favorites = 1,
		Calendar = 2,
		Contact = 4,
		Task = 8
	}
}
