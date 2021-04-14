using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum ObjectInfoGrbit
	{
		Bookmark = 1,
		Rollback = 2,
		Updatable = 4
	}
}
