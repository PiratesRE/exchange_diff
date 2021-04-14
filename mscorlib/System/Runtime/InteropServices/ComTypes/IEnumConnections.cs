using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Guid("B196B287-BAB4-101A-B69C-00AA00341D07")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[__DynamicallyInvokable]
	[ComImport]
	public interface IEnumConnections
	{
		[PreserveSig]
		int Next(int celt, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [Out] CONNECTDATA[] rgelt, IntPtr pceltFetched);

		[__DynamicallyInvokable]
		[PreserveSig]
		int Skip(int celt);

		[__DynamicallyInvokable]
		void Reset();

		[__DynamicallyInvokable]
		void Clone(out IEnumConnections ppenum);
	}
}
