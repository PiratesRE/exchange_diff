using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	public enum StatusPersistTreeNodeType
	{
		None = 0,
		CurrentNode = 1,
		FavoritesRoot = 2,
		BuddyListRoot = 4
	}
}
