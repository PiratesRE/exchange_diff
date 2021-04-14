using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Guid("B196B285-BAB4-101A-B69C-00AA00341D07")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[__DynamicallyInvokable]
	[ComImport]
	public interface IEnumConnectionPoints
	{
		[PreserveSig]
		int Next(int celt, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [Out] IConnectionPoint[] rgelt, IntPtr pceltFetched);

		[__DynamicallyInvokable]
		[PreserveSig]
		int Skip(int celt);

		[__DynamicallyInvokable]
		void Reset();

		[__DynamicallyInvokable]
		void Clone(out IEnumConnectionPoints ppenum);
	}
}
