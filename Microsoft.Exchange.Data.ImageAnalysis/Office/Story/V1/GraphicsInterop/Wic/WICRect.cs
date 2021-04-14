using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct WICRect
	{
		public int X;

		public int Y;

		public int Width;

		public int Height;
	}
}
