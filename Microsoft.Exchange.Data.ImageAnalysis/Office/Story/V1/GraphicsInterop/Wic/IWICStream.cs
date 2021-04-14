using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Office.Story.V1.GraphicsInterop.Misc;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("135FF860-22B7-4DDF-B0F6-218F4F299A43")]
	internal interface IWICStream : IStream
	{
		void Read([In] [Out] byte[] pv, [In] int cb, [In] IntPtr pcbRead);

		void Write([In] [Out] byte[] pv, [In] int cb, [In] IntPtr pcbWritten);

		void Seek([In] long dlibMove, [In] int dwOrigin, [In] IntPtr plibNewPosition);

		void SetSize([In] long libNewSize);

		void CopyTo([MarshalAs(UnmanagedType.Interface)] [In] IStream pstm, [In] long cb, IntPtr pcbRead, IntPtr pcbWritten);

		void Commit([In] int grfCommitFlags);

		void Revert();

		void LockRegion([In] long libOffset, [In] long cb, [In] int dwLockType);

		void UnlockRegion([In] long libOffset, [In] long cb, [In] int dwLockType);

		void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, [In] int grfStatFlag);

		void Clone([MarshalAs(UnmanagedType.Interface)] out IStream ppstm);

		void InitializeFromIStream([MarshalAs(UnmanagedType.Interface)] [In] IStream pIStream);

		void InitializeFromFilename([MarshalAs(UnmanagedType.LPWStr)] [In] string wzFilename, [In] GenericAccess dwDesiredAccess);

		void InitializeFromMemory([In] IntPtr pbBuffer, [In] int cbBufferSize);

		void InitializeFromIStreamRegion([MarshalAs(UnmanagedType.Interface)] [In] IStream pIStream, [In] long ulOffset, [In] long ulMaxSize);
	}
}
