using System;

namespace Microsoft.Isam.Esent.Interop.Vista
{
	public static class VistaGrbits
	{
		public const CreateIndexGrbit IndexCrossProduct = (CreateIndexGrbit)16384;

		public const CreateIndexGrbit IndexDisallowTruncation = (CreateIndexGrbit)65536;

		public const CreateIndexGrbit IndexNestedTable = (CreateIndexGrbit)131072;

		public const EndExternalBackupGrbit TruncateDone = (EndExternalBackupGrbit)256;

		public const InitGrbit RecoveryWithoutUndo = (InitGrbit)8;

		public const InitGrbit TruncateLogsAfterRecovery = (InitGrbit)16;

		public const InitGrbit ReplayMissingMapEntryDB = (InitGrbit)32;

		public const InitGrbit LogStreamMustExist = (InitGrbit)64;

		public const SnapshotPrepareGrbit ContinueAfterThaw = (SnapshotPrepareGrbit)4;

		internal const CreateIndexGrbit IndexKeyMost = (CreateIndexGrbit)32768;

		internal const CreateIndexGrbit IndexUnicode = (CreateIndexGrbit)2048;
	}
}
