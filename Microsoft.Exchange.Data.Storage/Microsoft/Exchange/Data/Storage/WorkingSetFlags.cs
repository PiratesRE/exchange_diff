using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum WorkingSetFlags
	{
		Exchange = 1,
		WorkingSet = 2,
		Subscribed = 4,
		Pinned = 8,
		Groups = 16,
		SPO = 32,
		Yammer = 64
	}
}
