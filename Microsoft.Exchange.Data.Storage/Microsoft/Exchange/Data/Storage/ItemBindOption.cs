using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ItemBindOption
	{
		None = 0,
		LoadRequiredPropertiesOnly = 1,
		SoftDeletedItem = 2
	}
}
