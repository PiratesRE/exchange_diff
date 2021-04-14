using System;
using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	public struct NativeOverlapped
	{
		public IntPtr InternalLow;

		public IntPtr InternalHigh;

		public int OffsetLow;

		public int OffsetHigh;

		public IntPtr EventHandle;
	}
}
