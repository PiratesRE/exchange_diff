using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00000100-0000-0000-C000-000000000046")]
	[ComImport]
	internal interface IEnumUnknown
	{
		[PreserveSig]
		int Next(uint celt, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.IUnknown)] [Out] object[] rgelt, ref uint celtFetched);

		[PreserveSig]
		int Skip(uint celt);

		[PreserveSig]
		int Reset();

		[PreserveSig]
		int Clone(out IEnumUnknown enumUnknown);
	}
}
