using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Guid("0000000c-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IFastStream
	{
		[PreserveSig]
		int Read(IntPtr pv, uint cb, out uint cbRead);

		[PreserveSig]
		int Write([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, int cb, out int pcbWritten);

		[PreserveSig]
		int Seek(long dlibMove, int dwOrigin, out long plibNewPosition);

		[PreserveSig]
		int SetSize(long libNewSize);

		[PreserveSig]
		int CopyTo(IFastStream pstm, long cb, IntPtr pcbRead, out long pcbWritten);

		[PreserveSig]
		int Commit(int grfCommitFlags);

		[PreserveSig]
		int Revert();

		[PreserveSig]
		int LockRegion(long libOffset, long cb, int dwLockType);

		[PreserveSig]
		int UnlockRegion(long libOffset, long cb, int dwLockType);

		[PreserveSig]
		int Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag);

		[PreserveSig]
		int Clone(out IFastStream ppstm);
	}
}
