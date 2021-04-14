using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00000123-A8F2-4877-BA0A-FD2B6645FB94")]
	internal interface IWICBitmapLock
	{
		void GetSize(out int puiWidth, out int puiHeight);

		void GetStride(out int pcbStride);

		void GetDataPointer(out int pcbBufferSize, out IntPtr ppbData);

		void GetPixelFormat(out Guid pPixelFormat);
	}
}
