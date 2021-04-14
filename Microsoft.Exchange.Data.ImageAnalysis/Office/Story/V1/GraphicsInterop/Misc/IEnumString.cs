using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Misc
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00000101-0000-0000-C000-000000000046")]
	internal interface IEnumString
	{
		[PreserveSig]
		int RemoteNext([In] int celt, [MarshalAs(UnmanagedType.LPWStr)] out string rgelt, out int pceltFetched);

		[PreserveSig]
		int Skip([In] int celt);

		void Reset();

		void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumString ppenum);
	}
}
