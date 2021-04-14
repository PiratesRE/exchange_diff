using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Exchange.Security.RightsManagement.StructuredStorage
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("0000000C-0000-0000-C000-000000000046")]
	[ComImport]
	internal interface IStream
	{
		int Read(IntPtr buf, int len);

		int Write(IntPtr buf, int len);

		[return: MarshalAs(UnmanagedType.I8)]
		long Seek([MarshalAs(UnmanagedType.I8)] [In] long dlibMove, int dwOrigin);

		void SetSize([MarshalAs(UnmanagedType.I8)] [In] long libNewSize);

		[return: MarshalAs(UnmanagedType.I8)]
		long CopyTo([MarshalAs(UnmanagedType.Interface)] [In] IStream pstm, [MarshalAs(UnmanagedType.I8)] [In] long cb, [MarshalAs(UnmanagedType.LPArray)] [Out] long[] pcbRead);

		void Commit([MarshalAs(UnmanagedType.I4)] [In] STGC grfCommitFlags);

		void Revert();

		void LockRegion([MarshalAs(UnmanagedType.I8)] [In] long libOffset, [MarshalAs(UnmanagedType.I8)] [In] long cb, int dwLockType);

		void UnlockRegion([MarshalAs(UnmanagedType.I8)] [In] long libOffset, [MarshalAs(UnmanagedType.I8)] [In] long cb, int dwLockType);

		void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pStatstg, [MarshalAs(UnmanagedType.I4)] [In] STATFLAG grfStatFlag);

		[return: MarshalAs(UnmanagedType.Interface)]
		IStream Clone();
	}
}
