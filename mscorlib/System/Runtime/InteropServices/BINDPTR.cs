using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.BINDPTR instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
	public struct BINDPTR
	{
		[FieldOffset(0)]
		public IntPtr lpfuncdesc;

		[FieldOffset(0)]
		public IntPtr lpvardesc;

		[FieldOffset(0)]
		public IntPtr lptcomp;
	}
}
