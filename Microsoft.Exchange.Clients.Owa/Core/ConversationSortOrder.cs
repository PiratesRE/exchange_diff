using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	public enum ConversationSortOrder
	{
		Chronological = 1,
		Tree = 2,
		NewestOnTop = 4,
		NewestOnBottom = 8,
		ChronologicalNewestOnTop = 5,
		ChronologicalNewestOnBottom = 9,
		TreeNewestOnBottom = 10
	}
}
