using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HPALETTE
	{
		public int fContext;

		public AnonymousUnionHandle u;
	}
}
