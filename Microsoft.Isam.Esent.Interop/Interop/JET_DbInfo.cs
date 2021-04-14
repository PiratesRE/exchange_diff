using System;

namespace Microsoft.Isam.Esent.Interop
{
	public enum JET_DbInfo
	{
		Filename,
		LCID = 3,
		Options = 6,
		Transactions,
		Version,
		Filesize = 10,
		SpaceOwned,
		SpaceAvailable,
		Misc = 14,
		DBInUse,
		PageSize = 17,
		FileType = 19
	}
}
