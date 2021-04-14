using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Misc
{
	[Guid("00000100-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IEnumUnknown
	{
		void RemoteNext([In] int celt, [MarshalAs(UnmanagedType.IUnknown)] out object rgelt, out int pceltFetched);

		void Skip([In] int celt);

		void Reset();

		void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumUnknown ppenum);
	}
}
