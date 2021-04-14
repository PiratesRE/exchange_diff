using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[Guid("00000121-A8F2-4877-BA0A-FD2B6645FB94")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IWICBitmap : IWICBitmapSource
	{
		void GetSize(out int puiWidth, out int puiHeight);

		void GetPixelFormat(out Guid pPixelFormat);

		void GetResolution(out double pDpiX, out double pDpiY);

		void CopyPalette([MarshalAs(UnmanagedType.Interface)] [In] IWICPalette pIPalette);

		void CopyPixels([In] ref WICRect prc, [In] int cbStride, [In] int cbBufferSize, IntPtr pbBuffer);

		void Lock([In] ref WICRect prcLock, [In] WICBitmapLockFlags flags, [MarshalAs(UnmanagedType.Interface)] out IWICBitmapLock ppILock);

		void SetPalette([MarshalAs(UnmanagedType.Interface)] [In] IWICPalette pIPalette);

		void SetResolution([In] double dpiX, [In] double dpiY);
	}
}
