using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Office.Story.V1.GraphicsInterop.Misc;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("EC5EC8A9-C395-4314-9C77-54D7A935FF70")]
	internal interface IWICImagingFactory
	{
		[return: MarshalAs(UnmanagedType.Interface)]
		IWICBitmapDecoder CreateDecoderFromFilename([MarshalAs(UnmanagedType.LPWStr)] [In] string wzFilename, [In] NullableGuid pguidVendor, [In] GenericAccess dwDesiredAccess, [In] WICDecodeOptions metadataOptions);

		[return: MarshalAs(UnmanagedType.Interface)]
		IWICBitmapDecoder CreateDecoderFromStream([MarshalAs(UnmanagedType.Interface)] [In] IStream pIStream, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] [In] [Out] Guid[] pguidVendor, [In] WICDecodeOptions metadataOptions);

		[return: MarshalAs(UnmanagedType.Interface)]
		IWICBitmapDecoder CreateDecoderFromFileHandle([In] IntPtr hFile, [In] ref Guid pguidVendor, [In] WICDecodeOptions metadataOptions);

		void CreateComponentInfo([In] ref Guid clsidComponent, [MarshalAs(UnmanagedType.Interface)] out IWICComponentInfo ppIInfo);

		[return: MarshalAs(UnmanagedType.Interface)]
		IWICBitmapDecoder CreateDecoder([In] ref Guid guidContainerFormat, [In] ref Guid pguidVendor);

		[return: MarshalAs(UnmanagedType.Interface)]
		IWICBitmapEncoder CreateEncoder([In] ref Guid guidContainerFormat, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] [In] [Out] Guid[] pguidVendor);

		void CreatePalette([MarshalAs(UnmanagedType.Interface)] out IWICPalette ppIPalette);

		void CreateFormatConverter([MarshalAs(UnmanagedType.Interface)] out IWICFormatConverter ppIFormatConverter);

		void CreateBitmapScaler([MarshalAs(UnmanagedType.Interface)] out IWICBitmapScaler ppIBitmapScaler);

		void CreateBitmapClipper([MarshalAs(UnmanagedType.Interface)] out IWICBitmapClipper ppIBitmapClipper);

		void CreateBitmapFlipRotator([MarshalAs(UnmanagedType.Interface)] out IWICBitmapFlipRotator ppIBitmapFlipRotator);

		void CreateStream([MarshalAs(UnmanagedType.Interface)] out IWICStream ppIWICStream);

		void CreateColorContext([MarshalAs(UnmanagedType.Interface)] out IWICColorContext ppIWICColorContext);

		void CreateColorTransformer([MarshalAs(UnmanagedType.Interface)] out IWICColorTransform ppIWICColorTransform);

		void CreateBitmap([In] int uiWidth, [In] int uiHeight, [In] ref Guid pixelFormat, [In] WICBitmapCreateCacheOption option, [MarshalAs(UnmanagedType.Interface)] out IWICBitmap ppIBitmap);

		void CreateBitmapFromSource([MarshalAs(UnmanagedType.Interface)] [In] IWICBitmapSource pIBitmapSource, [In] WICBitmapCreateCacheOption option, [MarshalAs(UnmanagedType.Interface)] out IWICBitmap ppIBitmap);

		void CreateBitmapFromSourceRect([MarshalAs(UnmanagedType.Interface)] [In] IWICBitmapSource pIBitmapSource, [In] int X, [In] int Y, [In] int Width, [In] int Height, [MarshalAs(UnmanagedType.Interface)] out IWICBitmap ppIBitmap);

		void CreateBitmapFromMemory([In] int uiWidth, [In] int uiHeight, [In] ref Guid pixelFormat, [In] int cbStride, [In] int cbBufferSize, [In] IntPtr pbBuffer, [MarshalAs(UnmanagedType.Interface)] out IWICBitmap ppIBitmap);

		void CreateBitmapFromHBITMAP([In] ref HBITMAP hBitmap, [In] ref HPALETTE hPalette, [In] WICBitmapAlphaChannelOption options, [MarshalAs(UnmanagedType.Interface)] out IWICBitmap ppIBitmap);

		void CreateBitmapFromHICON([In] ref IntPtr hIcon, [MarshalAs(UnmanagedType.Interface)] out IWICBitmap ppIBitmap);

		void CreateComponentEnumerator([In] int componentTypes, [In] int options, [MarshalAs(UnmanagedType.Interface)] out IEnumUnknown ppIEnumUnknown);

		void CreateFastMetadataEncoderFromDecoder([MarshalAs(UnmanagedType.Interface)] [In] IWICBitmapDecoder pIDecoder, [MarshalAs(UnmanagedType.Interface)] out IWICFastMetadataEncoder ppIFastEncoder);

		void CreateFastMetadataEncoderFromFrameDecode([MarshalAs(UnmanagedType.Interface)] [In] IWICBitmapFrameDecode pIFrameDecoder, [MarshalAs(UnmanagedType.Interface)] out IWICFastMetadataEncoder ppIFastEncoder);

		void CreateQueryWriter([In] ref Guid guidMetadataFormat, [In] ref Guid pguidVendor, [MarshalAs(UnmanagedType.Interface)] out IWICMetadataQueryWriter ppIQueryWriter);

		void CreateQueryWriterFromReader([MarshalAs(UnmanagedType.Interface)] [In] IWICMetadataQueryReader pIQueryReader, [In] ref Guid pguidVendor, [MarshalAs(UnmanagedType.Interface)] out IWICMetadataQueryWriter ppIQueryWriter);
	}
}
