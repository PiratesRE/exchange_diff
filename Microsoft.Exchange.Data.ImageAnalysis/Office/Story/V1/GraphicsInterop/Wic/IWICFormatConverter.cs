using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[Guid("00000301-A8F2-4877-BA0A-FD2B6645FB94")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IWICFormatConverter : IWICBitmapSource
	{
		void GetSize(out int puiWidth, out int puiHeight);

		void GetPixelFormat(out Guid pPixelFormat);

		void GetResolution(out double pDpiX, out double pDpiY);

		void CopyPalette([MarshalAs(UnmanagedType.Interface)] [In] IWICPalette pIPalette);

		void CopyPixels([In] ref WICRect prc, [In] int cbStride, [In] int cbBufferSize, IntPtr pbBuffer);

		void Initialize([MarshalAs(UnmanagedType.Interface)] [In] IWICBitmapSource pISource, [In] ref Guid dstFormat, [In] WICBitmapDitherType dither, [MarshalAs(UnmanagedType.Interface)] [In] IWICPalette pIPalette, [In] double alphaThresholdPercent, [In] WICBitmapPaletteType paletteTranslate);

		void CanConvert([In] ref Guid srcPixelFormat, [In] ref Guid dstPixelFormat, out int pfCanConvert);
	}
}
