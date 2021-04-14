using System;
using System.Runtime.InteropServices;
using Microsoft.Office.Story.V1.GraphicsInterop.Misc;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00000105-A8F2-4877-BA0A-FD2B6645FB94")]
	internal interface IWICBitmapFrameEncode
	{
		void Initialize([MarshalAs(UnmanagedType.Interface)] [In] IPropertyBag2 pIEncoderOptions);

		void SetSize([In] int uiWidth, [In] int uiHeight);

		void SetResolution([In] double dpiX, [In] double dpiY);

		void SetPixelFormat([In] [Out] ref Guid pPixelFormat);

		void SetColorContexts([In] int cCount, [MarshalAs(UnmanagedType.Interface)] [In] ref IWICColorContext ppIColorContext);

		void SetPalette([MarshalAs(UnmanagedType.Interface)] [In] IWICPalette pIPalette);

		void SetThumbnail([MarshalAs(UnmanagedType.Interface)] [In] IWICBitmapSource pIThumbnail);

		void WritePixels([In] int lineCount, [In] int cbStride, [In] int cbBufferSize, [In] ref byte pbPixels);

		void WriteSource([MarshalAs(UnmanagedType.Interface)] [In] IWICBitmapSource pIBitmapSource, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] [In] params WICRect[] prc);

		void Commit();

		void GetMetadataQueryWriter([MarshalAs(UnmanagedType.Interface)] out IWICMetadataQueryWriter ppIMetadataQueryWriter);
	}
}
