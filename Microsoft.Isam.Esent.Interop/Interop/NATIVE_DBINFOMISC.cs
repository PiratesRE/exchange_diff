using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_DBINFOMISC
	{
		public uint ulVersion;

		public uint ulUpdate;

		public NATIVE_SIGNATURE signDb;

		public uint dbstate;

		public JET_LGPOS lgposConsistent;

		public JET_LOGTIME logtimeConsistent;

		public JET_LOGTIME logtimeAttach;

		public JET_LGPOS lgposAttach;

		public JET_LOGTIME logtimeDetach;

		public JET_LGPOS lgposDetach;

		public NATIVE_SIGNATURE signLog;

		public JET_BKINFO bkinfoFullPrev;

		public JET_BKINFO bkinfoIncPrev;

		public JET_BKINFO bkinfoFullCur;

		public uint fShadowingDisabled;

		public uint fUpgradeDb;

		public uint dwMajorVersion;

		public uint dwMinorVersion;

		public uint dwBuildNumber;

		public uint lSPNumber;

		public uint cbPageSize;
	}
}
