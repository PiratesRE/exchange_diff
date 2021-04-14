using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	[Serializable]
	internal struct NATIVE_EMITDATACTX
	{
		public uint cbStruct;

		public uint dwVersion;

		public ulong qwSequenceNum;

		public uint grbitOperationalFlags;

		public JET_LOGTIME logtimeEmit;

		public JET_LGPOS lgposLogData;

		public uint cbLogData;
	}
}
