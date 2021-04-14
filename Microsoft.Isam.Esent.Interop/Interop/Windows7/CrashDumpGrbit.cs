using System;

namespace Microsoft.Isam.Esent.Interop.Windows7
{
	[Flags]
	public enum CrashDumpGrbit
	{
		None = 0,
		Minimum = 1,
		Maximum = 2,
		CacheMinimum = 4,
		CacheMaximum = 8,
		CacheIncludeDirtyPages = 16,
		CacheIncludeCachedPages = 32,
		CacheIncludeCorruptedPages = 64
	}
}
