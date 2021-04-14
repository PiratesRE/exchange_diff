using System;

namespace System
{
	[Serializable]
	internal enum InternalGCCollectionMode
	{
		NonBlocking = 1,
		Blocking,
		Optimized = 4,
		Compacting = 8
	}
}
