using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	internal struct NATIVE_LOGINFOMISC
	{
		public uint ulGeneration;

		public NATIVE_SIGNATURE signLog;

		public JET_LOGTIME logtimeCreate;

		public JET_LOGTIME logtimePreviousGeneration;

		public JET_LogInfoFlag ulFlags;

		public uint ulVersionMajor;

		public uint ulVersionMinor;

		public uint ulVersionUpdate;

		public uint cbSectorSize;

		public uint cbHeader;

		public uint cbFile;

		public uint cbDatabasePageSize;
	}
}
