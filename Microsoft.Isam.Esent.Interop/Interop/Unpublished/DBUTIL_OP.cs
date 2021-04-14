using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public enum DBUTIL_OP
	{
		Consistency,
		DumpData,
		DumpMetaData,
		DumpPage,
		DumpNode,
		DumpSpace,
		SetHeaderState,
		DumpHeader,
		DumpLogfile,
		DumpLogfileTrackNode,
		DumpCheckpoint,
		EDBDump,
		EDBRepair,
		Munge,
		EDBScrub,
		[Obsolete]
		SLVMove,
		DBConvertRecords,
		DBDefragment,
		[Obsolete]
		DumpExchangeSLVInfo,
		DumpUnicodeFixupTable_ObsoleteAndUnused,
		DumpPageUsage,
		UpdateDBHeaderFromTrailer,
		ChecksumLogFromMemory
	}
}
