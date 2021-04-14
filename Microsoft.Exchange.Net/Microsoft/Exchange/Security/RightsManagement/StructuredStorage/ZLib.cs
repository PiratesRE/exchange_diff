using System;

namespace Microsoft.Exchange.Security.RightsManagement.StructuredStorage
{
	internal static class ZLib
	{
		internal const string ZLibVersion = "1.2.3";

		internal enum ErrorCode
		{
			Success,
			StreamEnd,
			NeedDictionary,
			ErrorNo = -1,
			StreamError = -2,
			DataError = -3,
			MemError = -4,
			BufError = -5,
			VersionError = -6
		}

		internal enum FlushCodes
		{
			NoFlush,
			SyncFlush = 2,
			FullFlush,
			Finish
		}

		internal struct ZStream
		{
			public IntPtr PInBuf;

			public uint CbIn;

			public uint CbTotalIn;

			public IntPtr POutBuf;

			public uint CbOut;

			public uint CbTotalOut;

			public IntPtr PErrorMsgString;

			public IntPtr PState;

			public IntPtr PAlloc;

			public IntPtr PFree;

			public IntPtr POpaque;

			public int DataType;

			public uint Adler;

			public uint Reserved;
		}
	}
}
