using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[Guid("3C613A02-34B2-44EA-9A7C-45AEA9C6FD6D")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IWICColorContext
	{
		void InitializeFromFilename([MarshalAs(UnmanagedType.LPWStr)] [In] string wzFilename);

		void InitializeFromMemory([In] ref byte pbBuffer, [In] int cbBufferSize);

		void InitializeFromExifColorSpace([In] int value);

		void GetType(out WICColorContextType pType);

		void GetProfileBytes([In] int cbBuffer, [In] [Out] ref byte pbBuffer, out int pcbActual);

		void GetExifColorSpace(out int pValue);
	}
}
