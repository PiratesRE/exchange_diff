using System;
using System.Diagnostics;

namespace Microsoft.Isam.Esent.Interop
{
	internal static class Caches
	{
		public static MemoryCache ColumnCache
		{
			[DebuggerStepThrough]
			get
			{
				return Caches.TheColumnCache;
			}
		}

		public static MemoryCache BookmarkCache
		{
			[DebuggerStepThrough]
			get
			{
				return Caches.TheBookmarkCache;
			}
		}

		public static MemoryCache SecondaryBookmarkCache
		{
			[DebuggerStepThrough]
			get
			{
				return Caches.TheSecondaryBookmarkCache;
			}
		}

		private const int KeyMostMost = 2000;

		private const int MaxBuffers = 16;

		private static readonly MemoryCache TheColumnCache = new MemoryCache(131072, 16);

		private static readonly MemoryCache TheBookmarkCache = new MemoryCache(2000, 16);

		private static readonly MemoryCache TheSecondaryBookmarkCache = new MemoryCache(2000, 16);
	}
}
