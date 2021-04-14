using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_DBINFOMISC6
	{
		public NATIVE_DBINFOMISC4 dbinfo4;

		public uint ulIncrementalReseedCount;

		public JET_LOGTIME logtimeIncrementalReseed;

		public uint ulIncrementalReseedCountOld;

		public uint ulPagePatchCount;

		public JET_LOGTIME logtimePagePatch;

		public uint ulPagePatchCountOld;

		public JET_LOGTIME logtimeChecksumPrev;

		public JET_LOGTIME logtimeChecksumStart;

		public uint cpgDatabaseChecked;
	}
}
