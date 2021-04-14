using System;

namespace Microsoft.Isam.Esent.Interop
{
	public enum JET_prep
	{
		Insert,
		Replace = 2,
		Cancel,
		ReplaceNoLock,
		InsertCopy,
		InsertCopyDeleteOriginal = 7
	}
}
