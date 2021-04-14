using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[Guid("00000302-A8F2-4877-BA0A-FD2B6645FB94")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IWICBitmapScaler : IWICBitmapSource
	{
		void GetSize(out int puiWidth, out int puiHeight);

		void GetPixelFormat(out Guid pPixelFormat);

		void GetResolution(out double pDpiX, out double pDpiY);

		void CopyPalette([MarshalAs(UnmanagedType.Interface)] [In] IWICPalette pIPalette);

		void CopyPixels([In] ref WICRect prc, [In] int cbStride, [In] int cbBufferSize, IntPtr pbBuffer);

		void Initialize([MarshalAs(UnmanagedType.Interface)] [In] IWICBitmapSource pISource, [In] int uiWidth, [In] int uiHeight, [In] WICBitmapInterpolationMode mode);
	}
}
