using System;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis
{
	internal sealed class PrivateHashAlgorithm2
	{
		public static uint GetUInt32HashCode(byte[] data)
		{
			uint num = 0U;
			for (int i = 0; i < data.Length; i++)
			{
				num += (uint)data[i];
				num += num << 10;
				num ^= num >> 6;
			}
			num += num << 3;
			num ^= num >> 11;
			return num + (num << 15);
		}
	}
}
