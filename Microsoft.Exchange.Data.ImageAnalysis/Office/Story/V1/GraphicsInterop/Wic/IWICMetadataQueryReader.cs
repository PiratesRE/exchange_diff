using System;
using System.Runtime.InteropServices;
using Microsoft.Office.Story.V1.GraphicsInterop.Misc;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("30989668-E1C9-4597-B395-458EEDB808DF")]
	internal interface IWICMetadataQueryReader
	{
		void GetContainerFormat(out Guid pguidContainerFormat);

		void GetLocation([In] int cchMaxLength, [In] [Out] ref ushort wzNamespace, out int pcchActualLength);

		[PreserveSig]
		int GetMetadataByName([MarshalAs(UnmanagedType.LPWStr)] [In] string wzName, out PROPVARIANT pvarValue);

		void GetEnumerator([MarshalAs(UnmanagedType.Interface)] out IEnumString ppIEnumString);
	}
}
