using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[Guid("9EDDE9E7-8DEE-47EA-99DF-E6FAF2ED44BF")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IWICBitmapDecoder
	{
		void QueryCapability([MarshalAs(UnmanagedType.Interface)] [In] IStream pIStream, out int pdwCapability);

		void Initialize([MarshalAs(UnmanagedType.Interface)] [In] IStream pIStream, [In] WICDecodeOptions cacheOptions);

		void GetContainerFormat(out Guid pguidContainerFormat);

		void GetDecoderInfo([MarshalAs(UnmanagedType.Interface)] out IWICBitmapDecoderInfo ppIDecoderInfo);

		void CopyPalette([MarshalAs(UnmanagedType.Interface)] [In] IWICPalette pIPalette);

		void GetMetadataQueryReader([MarshalAs(UnmanagedType.Interface)] out IWICMetadataQueryReader ppIMetadataQueryReader);

		void GetPreview([MarshalAs(UnmanagedType.Interface)] out IWICBitmapSource ppIBitmapSource);

		void GetColorContexts([In] int cCount, [MarshalAs(UnmanagedType.Interface)] [In] [Out] ref IWICColorContext ppIColorContexts, out int pcActualCount);

		void GetThumbnail([MarshalAs(UnmanagedType.Interface)] out IWICBitmapSource ppIThumbnail);

		void GetFrameCount(out int pCount);

		void GetFrame([In] int index, [MarshalAs(UnmanagedType.Interface)] out IWICBitmapFrameDecode ppIBitmapFrame);
	}
}
