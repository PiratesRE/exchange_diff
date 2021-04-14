using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("B66F034F-D0E2-40AB-B436-6DE39E321A94")]
	internal interface IWICColorTransform : IWICBitmapSource
	{
		void GetSize(out int puiWidth, out int puiHeight);

		void GetPixelFormat(out Guid pPixelFormat);

		void GetResolution(out double pDpiX, out double pDpiY);

		void CopyPalette([MarshalAs(UnmanagedType.Interface)] [In] IWICPalette pIPalette);

		void CopyPixels([In] ref WICRect prc, [In] int cbStride, [In] int cbBufferSize, IntPtr pbBuffer);

		void Initialize([MarshalAs(UnmanagedType.Interface)] [In] IWICBitmapSource pIBitmapSource, [MarshalAs(UnmanagedType.Interface)] [In] IWICColorContext pIContextSource, [MarshalAs(UnmanagedType.Interface)] [In] IWICColorContext pIContextDest, [In] ref Guid pixelFmtDest);
	}
}
