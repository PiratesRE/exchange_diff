using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("D8CD007F-D08F-4191-9BFC-236EA7F0E4B5")]
	internal interface IWICBitmapDecoderInfo : IWICBitmapCodecInfo, IWICComponentInfo
	{
		void GetComponentType(out WICComponentType pType);

		void GetCLSID(out Guid pclsid);

		void GetSigningStatus(out int pStatus);

		void GetAuthor([In] int cchAuthor, [In] [Out] ref ushort wzAuthor, out int pcchActual);

		void GetVendorGUID(out Guid pguidVendor);

		void GetVersion([In] int cchVersion, [In] [Out] ref ushort wzVersion, out int pcchActual);

		void GetSpecVersion([In] int cchSpecVersion, [In] [Out] ref ushort wzSpecVersion, out int pcchActual);

		void GetFriendlyName([In] int cchFriendlyName, [In] [Out] ref ushort wzFriendlyName, out int pcchActual);

		void GetContainerFormat(out Guid pguidContainerFormat);

		void GetPixelFormats([In] int cFormats, [In] [Out] ref Guid pguidPixelFormats, out int pcActual);

		void GetColorManagementVersion([In] int cchColorManagementVersion, [In] [Out] ref ushort wzColorManagementVersion, out int pcchActual);

		void GetDeviceManufacturer([In] int cchDeviceManufacturer, [In] [Out] ref ushort wzDeviceManufacturer, out int pcchActual);

		void GetDeviceModels([In] int cchDeviceModels, [In] [Out] ref ushort wzDeviceModels, out int pcchActual);

		void GetMimeTypes([In] int cchMimeTypes, [In] [Out] ref ushort wzMimeTypes, out int pcchActual);

		void GetFileExtensions([In] int cchFileExtensions, [In] [Out] ref ushort wzFileExtensions, out int pcchActual);

		void DoesSupportAnimation(out int pfSupportAnimation);

		void DoesSupportChromakey(out int pfSupportChromakey);

		void DoesSupportLossless(out int pfSupportLossless);

		void DoesSupportMultiframe(out int pfSupportMultiframe);

		void MatchesMimeType([MarshalAs(UnmanagedType.LPWStr)] [In] string wzMimeType, out int pfMatches);

		void Remote_GetPatterns([Out] IntPtr ppPatterns, out int pcPatterns);

		void MatchesPattern([MarshalAs(UnmanagedType.Interface)] [In] IStream pIStream, out int pfMatches);

		void CreateInstance([MarshalAs(UnmanagedType.Interface)] out IWICBitmapDecoder ppIBitmapDecoder);
	}
}
