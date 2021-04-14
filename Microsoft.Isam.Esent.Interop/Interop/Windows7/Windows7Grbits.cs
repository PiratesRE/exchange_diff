using System;

namespace Microsoft.Isam.Esent.Interop.Windows7
{
	public static class Windows7Grbits
	{
		public const ColumndefGrbit ColumnCompressed = (ColumndefGrbit)524288;

		public const SetColumnGrbit Compressed = (SetColumnGrbit)131072;

		public const SetColumnGrbit Uncompressed = (SetColumnGrbit)65536;

		public const InitGrbit ReplayIgnoreLostLogs = (InitGrbit)128;

		public const TermGrbit Dirty = (TermGrbit)8;

		public const TempTableGrbit IntrinsicLVsOnly = (TempTableGrbit)128;

		public const EnumerateColumnsGrbit EnumerateInRecordOnly = (EnumerateColumnsGrbit)2097152;

		public const CommitTransactionGrbit ForceNewLog = (CommitTransactionGrbit)16;

		public const SnapshotPrepareGrbit ExplicitPrepare = (SnapshotPrepareGrbit)8;

		public const SetTableSequentialGrbit Forward = (SetTableSequentialGrbit)1;

		public const DefragGrbit NoPartialMerges = (DefragGrbit)128;

		public const DefragGrbit DefragmentBTree = (DefragGrbit)256;

		public const SetTableSequentialGrbit Backward = (SetTableSequentialGrbit)2;

		public const AttachDatabaseGrbit EnableAttachDbBackgroundMaintenance = (AttachDatabaseGrbit)2048;

		public const CreateDatabaseGrbit EnableCreateDbBackgroundMaintenance = (CreateDatabaseGrbit)2048;
	}
}
