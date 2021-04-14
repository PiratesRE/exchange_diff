using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[Guid("94C9B4EE-A09F-4F92-8A1E-4A9BCE7E76FB")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IWICBitmapEncoderInfo : IWICBitmapCodecInfo, IWICComponentInfo
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

		void CreateInstance([MarshalAs(UnmanagedType.Interface)] out IWICBitmapEncoder ppIBitmapEncoder);
	}
}
