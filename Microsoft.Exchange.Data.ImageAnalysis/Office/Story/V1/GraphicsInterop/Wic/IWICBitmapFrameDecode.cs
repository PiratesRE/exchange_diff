using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[Guid("3B16811B-6A43-4EC9-A813-3D930C13B940")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IWICBitmapFrameDecode : IWICBitmapSource
	{
		void GetSize(out int puiWidth, out int puiHeight);

		void GetPixelFormat(out Guid pPixelFormat);

		void GetResolution(out double pDpiX, out double pDpiY);

		void CopyPalette([MarshalAs(UnmanagedType.Interface)] [In] IWICPalette pIPalette);

		void CopyPixels([In] ref WICRect prc, [In] int cbStride, [In] int cbBufferSize, IntPtr pbBuffer);

		void GetMetadataQueryReader([MarshalAs(UnmanagedType.Interface)] out IWICMetadataQueryReader ppIMetadataQueryReader);

		void GetColorContexts([In] int cCount, [MarshalAs(UnmanagedType.Interface)] [In] [Out] ref IWICColorContext ppIColorContexts, out int pcActualCount);

		void GetThumbnail([MarshalAs(UnmanagedType.Interface)] out IWICBitmapSource ppIThumbnail);
	}
}
