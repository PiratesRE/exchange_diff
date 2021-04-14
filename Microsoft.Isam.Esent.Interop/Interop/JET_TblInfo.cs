using System;

namespace Microsoft.Isam.Esent.Interop
{
	public enum JET_TblInfo
	{
		Default,
		Name,
		Dbid,
		SpaceUsage = 7,
		SpaceAlloc = 9,
		SpaceOwned,
		SpaceAvailable,
		TemplateTableName
	}
}
