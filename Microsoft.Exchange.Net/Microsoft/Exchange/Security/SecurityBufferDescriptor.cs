using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security
{
	[StructLayout(LayoutKind.Sequential)]
	internal class SecurityBufferDescriptor
	{
		public SecurityBufferDescriptor(int count)
		{
			this.Count = count;
		}

		public readonly int Version;

		public readonly int Count;

		public unsafe void* UnmanagedPointer;
	}
}
