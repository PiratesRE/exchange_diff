using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class BufferOperations
	{
		internal static void Zero(byte[] buf, int offset, int len)
		{
			while (len > 0)
			{
				buf[offset++] = 0;
				len--;
			}
		}
	}
}
