using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.ELEMDESC instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct ELEMDESC
	{
		public TYPEDESC tdesc;

		public ELEMDESC.DESCUNION desc;

		[ComVisible(false)]
		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct DESCUNION
		{
			[FieldOffset(0)]
			public IDLDESC idldesc;

			[FieldOffset(0)]
			public PARAMDESC paramdesc;
		}
	}
}
