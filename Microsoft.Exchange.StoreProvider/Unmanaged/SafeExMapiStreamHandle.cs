using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComVisible(false)]
	internal class SafeExMapiStreamHandle : SafeExInterfaceHandle, IExMapiStream, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExMapiStreamHandle()
		{
		}

		internal SafeExMapiStreamHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExMapiStreamHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExMapiStreamHandle>(this);
		}

		public int Read(byte[] pv, uint cb, out uint cbRead)
		{
			GCHandle gchandle = default(GCHandle);
			int result;
			try
			{
				gchandle = GCHandle.Alloc(pv, GCHandleType.Pinned);
				IntPtr pv2 = Marshal.UnsafeAddrOfPinnedArrayElement(pv, 0);
				result = SafeExMapiStreamHandle.IStream_Read(this.handle, pv2, cb, out cbRead);
			}
			finally
			{
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
			}
			return result;
		}

		public int Write(byte[] pv, int cb, out int pcbWritten)
		{
			return SafeExMapiStreamHandle.IStream_Write(this.handle, pv, cb, out pcbWritten);
		}

		public int Seek(long dlibMove, int dwOrigin, out long plibNewPosition)
		{
			return SafeExMapiStreamHandle.IStream_Seek(this.handle, dlibMove, dwOrigin, out plibNewPosition);
		}

		public int SetSize(long libNewSize)
		{
			return SafeExMapiStreamHandle.IStream_SetSize(this.handle, libNewSize);
		}

		public int CopyTo(IFastStream pstm, long cb, IntPtr pcbRead, out long pcbWritten)
		{
			return SafeExMapiStreamHandle.IStream_CopyTo(this.handle, pstm, cb, pcbRead, out pcbWritten);
		}

		public int Commit(int grfCommitFlags)
		{
			return SafeExMapiStreamHandle.IStream_Commit(this.handle, grfCommitFlags);
		}

		public int Revert()
		{
			return SafeExMapiStreamHandle.IStream_Revert(this.handle);
		}

		public int LockRegion(long libOffset, long cb, int dwLockType)
		{
			return SafeExMapiStreamHandle.IStream_LockRegion(this.handle, libOffset, cb, dwLockType);
		}

		public int UnlockRegion(long libOffset, long cb, int dwLockType)
		{
			return SafeExMapiStreamHandle.IStream_UnlockRegion(this.handle, libOffset, cb, dwLockType);
		}

		public int Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
		{
			return SafeExMapiStreamHandle.IStream_Stat(this.handle, out pstatstg, grfStatFlag);
		}

		public int Clone(out IExInterface iStreamNew)
		{
			SafeExInterfaceHandle safeExInterfaceHandle = null;
			int result = SafeExMapiStreamHandle.IStream_Clone(this.handle, out safeExInterfaceHandle);
			iStreamNew = safeExInterfaceHandle;
			return result;
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IStream_Read(IntPtr iStream, IntPtr pv, uint cb, out uint cbRead);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IStream_Write(IntPtr iStream, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, int cb, out int pcbWritten);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IStream_Seek(IntPtr iStream, long dlibMove, int dwOrigin, out long plibNewPosition);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IStream_SetSize(IntPtr iStream, long libNewSize);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IStream_CopyTo(IntPtr iStream, IFastStream pstm, long cb, IntPtr pcbRead, out long pcbWritten);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IStream_Commit(IntPtr iStream, int grfCommitFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IStream_Revert(IntPtr iStream);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IStream_LockRegion(IntPtr iStream, long libOffset, long cb, int dwLockType);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IStream_UnlockRegion(IntPtr iStream, long libOffset, long cb, int dwLockType);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IStream_Stat(IntPtr iStream, out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IStream_Clone(IntPtr iStream, out SafeExInterfaceHandle iStreamNew);
	}
}
