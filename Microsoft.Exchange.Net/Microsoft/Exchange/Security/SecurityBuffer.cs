using System;

namespace Microsoft.Exchange.Security
{
	internal struct SecurityBuffer
	{
		public int count;

		public BufferType type;

		public IntPtr token;

		public static readonly int Size = sizeof(SecurityBuffer);
	}
}
