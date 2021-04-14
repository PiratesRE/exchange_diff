using System;
using System.Runtime.InteropServices;
using Microsoft.Office.Story.V1.GraphicsInterop.Misc;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[Guid("A721791A-0DEF-4D06-BD91-2118BF1DB10B")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IWICMetadataQueryWriter : IWICMetadataQueryReader
	{
		void GetContainerFormat(out Guid pguidContainerFormat);

		void GetLocation([In] int cchMaxLength, [In] [Out] ref ushort wzNamespace, out int pcchActualLength);

		void GetMetadataByName([MarshalAs(UnmanagedType.LPWStr)] [In] string wzName, out PROPVARIANT pvarValue);

		void GetEnumerator([MarshalAs(UnmanagedType.Interface)] out IEnumString ppIEnumString);

		void SetMetadataByName([MarshalAs(UnmanagedType.LPWStr)] [In] string wzName, [In] ref PROPVARIANT pvarValue);

		void RemoveMetadataByName([MarshalAs(UnmanagedType.LPWStr)] [In] string wzName);
	}
}
