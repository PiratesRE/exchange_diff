using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum PropertyDependencyType
	{
		None = 0,
		NeedForRead = 1,
		NeedToReadForWrite = 2,
		AllRead = 3,
		All = 3
	}
}
