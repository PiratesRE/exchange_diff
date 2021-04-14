using System;

namespace Microsoft.Isam.Esent.Interop.Windows8
{
	public static class Windows8Grbits
	{
		public const InitGrbit KeepDbAttachedAtEndOfRecovery = (InitGrbit)4096;

		public const AttachDatabaseGrbit PurgeCacheOnAttach = (AttachDatabaseGrbit)4096;

		public const CreateIndexGrbit IndexDotNetGuid = (CreateIndexGrbit)262144;

		public const TempTableGrbit TTDotNetGuid = (TempTableGrbit)256;
	}
}
