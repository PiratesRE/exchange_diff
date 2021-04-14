using System;
using Microsoft.Isam.Esent.Interop.Server2003;
using Microsoft.Isam.Esent.Interop.Windows8;
using Microsoft.Isam.Esent.Interop.Windows81;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public static class UnpublishedGrbits
	{
		public const IdleGrbit CompactAsync = (IdleGrbit)16;

		public const InitGrbit ExternalRecoveryControl = (InitGrbit)2048;

		public const ResizeDatabaseGrbit ShrinkCompactSize = (ResizeDatabaseGrbit)4;

		public const RetrieveColumnGrbit RetrievePageNumber = (RetrieveColumnGrbit)4096;

		public const RetrieveColumnGrbit RetrievePrereadOnly = (RetrieveColumnGrbit)16384;

		public const RetrieveColumnGrbit RetrievePrereadMany = (RetrieveColumnGrbit)32768;

		public const RetrieveColumnGrbit RetrievePhysicalSize = (RetrieveColumnGrbit)65536;

		public const ShrinkDatabaseGrbit Eof = (ShrinkDatabaseGrbit)16384;

		public const ShrinkDatabaseGrbit Periodically = (ShrinkDatabaseGrbit)32768;

		public const TermGrbit TermShrink = (TermGrbit)16;

		public const UpdateGrbit UpdateNoVersion = (UpdateGrbit)2;

		public const DurableCommitCallbackGrbit LogUnavailable = (DurableCommitCallbackGrbit)1;

		public const CreateTableColumnIndexGrbit TableImmutableStructure = (CreateTableColumnIndexGrbit)8;

		public const CreateIndexGrbit IndexImmutableStructure = (CreateIndexGrbit)524288;
	}
}
