using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.VARDESC instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct VARDESC
	{
		public int memid;

		public string lpstrSchema;

		public ELEMDESC elemdescVar;

		public short wVarFlags;

		public VarEnum varkind;

		[ComVisible(false)]
		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct DESCUNION
		{
			[FieldOffset(0)]
			public int oInst;

			[FieldOffset(0)]
			public IntPtr lpvarValue;
		}
	}
}
