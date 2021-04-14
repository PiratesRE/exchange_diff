using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Guid("00020404-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[__DynamicallyInvokable]
	[ComImport]
	public interface IEnumVARIANT
	{
		[PreserveSig]
		int Next(int celt, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [Out] object[] rgVar, IntPtr pceltFetched);

		[__DynamicallyInvokable]
		[PreserveSig]
		int Skip(int celt);

		[__DynamicallyInvokable]
		[PreserveSig]
		int Reset();

		[__DynamicallyInvokable]
		IEnumVARIANT Clone();
	}
}
