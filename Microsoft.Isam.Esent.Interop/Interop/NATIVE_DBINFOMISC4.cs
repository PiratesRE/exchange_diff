using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_DBINFOMISC4
	{
		public NATIVE_DBINFOMISC dbinfo;

		public uint genMinRequired;

		public uint genMaxRequired;

		public JET_LOGTIME logtimeGenMaxCreate;

		public uint ulRepairCount;

		public JET_LOGTIME logtimeRepair;

		public uint ulRepairCountOld;

		public uint ulECCFixSuccess;

		public JET_LOGTIME logtimeECCFixSuccess;

		public uint ulECCFixSuccessOld;

		public uint ulECCFixFail;

		public JET_LOGTIME logtimeECCFixFail;

		public uint ulECCFixFailOld;

		public uint ulBadChecksum;

		public JET_LOGTIME logtimeBadChecksum;

		public uint ulBadChecksumOld;

		public uint genCommitted;

		public JET_BKINFO bkinfoCopyPrev;

		public JET_BKINFO bkinfoDiffPrev;
	}
}
