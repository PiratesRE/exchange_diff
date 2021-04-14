using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	[Guid("96406BD5-2B2B-11d3-B36B-00C04F6108FF")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IWMInputMediaProps : IWMMediaProps
	{
		void GetType(out Guid pguidType);

		void GetMediaType(IntPtr pType, [In] [Out] ref uint pcbType);

		void SetMediaType([In] ref WindowsMediaNativeMethods.WM_MEDIA_TYPE pType);

		void GetConnectionName();

		void GetGroupName();
	}
}
