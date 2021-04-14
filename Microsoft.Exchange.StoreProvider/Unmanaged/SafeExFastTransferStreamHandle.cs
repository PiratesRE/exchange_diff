using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExFastTransferStreamHandle : SafeExInterfaceHandle
	{
		protected SafeExFastTransferStreamHandle()
		{
		}

		internal SafeExFastTransferStreamHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExFastTransferStreamHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		internal unsafe int Configure(int cValues, SPropValue* lpPropArray)
		{
			return SafeExFastTransferStreamHandle.IFastTransferStream_Configure(this.handle, cValues, lpPropArray);
		}

		internal int Download(out int cbBuffer, out SafeExMemoryHandle buffer)
		{
			return SafeExFastTransferStreamHandle.IFastTransferStream_Download(this.handle, out cbBuffer, out buffer);
		}

		internal unsafe int Upload(ArraySegment<byte> buffer)
		{
			fixed (byte* array = buffer.Array)
			{
				return SafeExFastTransferStreamHandle.IFastTransferStream_Upload(this.handle, buffer.Count, array + buffer.Offset);
			}
		}

		internal int Flush()
		{
			return SafeExFastTransferStreamHandle.IFastTransferStream_Flush(this.handle);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IFastTransferStream_Configure(IntPtr pIFastTransferStream, int cValues, SPropValue* lpPropArray);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IFastTransferStream_Download(IntPtr pIFastTransferStream, out int cbBuffer, out SafeExMemoryHandle buffer);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IFastTransferStream_Upload(IntPtr pIFastTransferStream, int cb, byte* lpb);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IFastTransferStream_Flush(IntPtr pIFastTransferStream);
	}
}
