using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum DetachDatabaseGrbit
	{
		None = 0,
		[Obsolete("ForceDetach is no longer used.")]
		ForceDetach = 1,
		[Obsolete("ForceClose is no longer used.")]
		ForceClose = 2,
		[Obsolete("ForceCloseAndDetach is no longer used.")]
		ForceCloseAndDetach = 3
	}
}
