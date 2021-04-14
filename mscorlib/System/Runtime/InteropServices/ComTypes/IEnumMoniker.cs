using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Guid("00000102-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[__DynamicallyInvokable]
	[ComImport]
	public interface IEnumMoniker
	{
		[PreserveSig]
		int Next(int celt, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [Out] IMoniker[] rgelt, IntPtr pceltFetched);

		[__DynamicallyInvokable]
		[PreserveSig]
		int Skip(int celt);

		[__DynamicallyInvokable]
		void Reset();

		[__DynamicallyInvokable]
		void Clone(out IEnumMoniker ppenum);
	}
}
