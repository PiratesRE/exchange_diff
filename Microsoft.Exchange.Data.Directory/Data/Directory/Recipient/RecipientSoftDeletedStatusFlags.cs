using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	public enum RecipientSoftDeletedStatusFlags
	{
		None = 0,
		Removed = 1,
		Disabled = 2,
		IncludeInGarbageCollection = 4,
		Inactive = 8
	}
}
