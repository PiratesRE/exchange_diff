using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum StateStorageFeatures
	{
		ContentState = 1,
		IdMap = 2,
		HierarchyState = 4
	}
}
