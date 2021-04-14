using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Misc
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct PROPBAG2
	{
		public int dwType;

		public ushort vt;

		private IntPtr cfType;

		public int dwHint;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string pstrName;

		public Guid clsid;
	}
}
