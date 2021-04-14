using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96406BCE-2B2B-11d3-B36B-00C04F6108FF")]
	[ComImport]
	internal interface IWMMediaProps
	{
		void GetType(out Guid pguidType);

		void GetMediaType(IntPtr pType, [In] [Out] ref uint pcbType);

		void SetMediaType([In] ref WindowsMediaNativeMethods.WM_MEDIA_TYPE pType);
	}
}
