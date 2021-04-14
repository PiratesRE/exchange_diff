using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Guid("0000000c-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[__DynamicallyInvokable]
	[ComImport]
	public interface IStream
	{
		void Read([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [Out] byte[] pv, int cb, IntPtr pcbRead);

		void Write([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, int cb, IntPtr pcbWritten);

		void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition);

		[__DynamicallyInvokable]
		void SetSize(long libNewSize);

		void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten);

		[__DynamicallyInvokable]
		void Commit(int grfCommitFlags);

		[__DynamicallyInvokable]
		void Revert();

		[__DynamicallyInvokable]
		void LockRegion(long libOffset, long cb, int dwLockType);

		[__DynamicallyInvokable]
		void UnlockRegion(long libOffset, long cb, int dwLockType);

		[__DynamicallyInvokable]
		void Stat(out STATSTG pstatstg, int grfStatFlag);

		[__DynamicallyInvokable]
		void Clone(out IStream ppstm);
	}
}
