using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Office.Story.V1.GraphicsInterop.Misc;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[Guid("00000103-A8F2-4877-BA0A-FD2B6645FB94")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IWICBitmapEncoder
	{
		void Initialize([MarshalAs(UnmanagedType.Interface)] [In] IStream pIStream, [In] WICBitmapEncoderCacheOption cacheOption);

		void GetContainerFormat(out Guid pguidContainerFormat);

		void GetEncoderInfo([MarshalAs(UnmanagedType.Interface)] out IWICBitmapEncoderInfo ppIEncoderInfo);

		void SetColorContexts([In] int cCount, [MarshalAs(UnmanagedType.Interface)] [In] ref IWICColorContext ppIColorContext);

		void SetPalette([MarshalAs(UnmanagedType.Interface)] [In] IWICPalette pIPalette);

		void SetThumbnail([MarshalAs(UnmanagedType.Interface)] [In] IWICBitmapSource pIThumbnail);

		void SetPreview([MarshalAs(UnmanagedType.Interface)] [In] IWICBitmapSource pIPreview);

		void CreateNewFrame([MarshalAs(UnmanagedType.Interface)] out IWICBitmapFrameEncode ppIFrameEncode, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.IUnknown, SizeConst = 1)] [Out] IPropertyBag2[] ppIEncoderOptions);

		void Commit();

		void GetMetadataQueryWriter([MarshalAs(UnmanagedType.Interface)] out IWICMetadataQueryWriter ppIMetadataQueryWriter);
	}
}
