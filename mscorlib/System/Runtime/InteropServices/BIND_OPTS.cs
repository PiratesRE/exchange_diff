using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.BIND_OPTS instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	public struct BIND_OPTS
	{
		public int cbStruct;

		public int grfFlags;

		public int grfMode;

		public int dwTickCountDeadline;
	}
}
