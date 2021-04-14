using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[Guid("23BC3F0A-698B-4357-886B-F24D50671334")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IWICComponentInfo
	{
		void GetComponentType(out WICComponentType pType);

		void GetCLSID(out Guid pclsid);

		void GetSigningStatus(out int pStatus);

		void GetAuthor([In] int cchAuthor, [In] [Out] ref ushort wzAuthor, out int pcchActual);

		void GetVendorGUID(out Guid pguidVendor);

		void GetVersion([In] int cchVersion, [In] [Out] ref ushort wzVersion, out int pcchActual);

		void GetSpecVersion([In] int cchSpecVersion, [In] [Out] ref ushort wzSpecVersion, out int pcchActual);

		void GetFriendlyName([In] int cchFriendlyName, [In] [Out] ref ushort wzFriendlyName, out int pcchActual);
	}
}
