using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[StructLayout(LayoutKind.Explicit, Pack = 8, Size = 8)]
	internal struct AnonymousUnionHandle
	{
		[FieldOffset(0)]
		public int hInproc;

		[FieldOffset(0)]
		public long hInproc64;
	}
}
