using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class RpcHttpPackets
	{
		public static bool IsConnA3PacketInBuffer(ArraySegment<byte> buffer)
		{
			return RpcHttpPackets.CheckPacketInStream(buffer, 28, RpcHttpRtsFlags.None, 1);
		}

		public static bool IsConnC2PacketInBuffer(ArraySegment<byte> buffer)
		{
			return RpcHttpPackets.CheckPacketInStream(buffer, 44, RpcHttpRtsFlags.None, 3);
		}

		public static bool IsPingPacket(ArraySegment<byte> buffer)
		{
			int num;
			return RpcHttpPackets.CheckPacket(buffer, 20, RpcHttpRtsFlags.Ping, 0, out num);
		}

		private static bool CheckPacketInStream(ArraySegment<byte> buffer, int unitLength, RpcHttpRtsFlags flags, int numberOfCommands)
		{
			while (buffer.Count > 0)
			{
				int num;
				if (RpcHttpPackets.CheckPacket(buffer, unitLength, flags, numberOfCommands, out num))
				{
					return true;
				}
				if (num == 0)
				{
					break;
				}
				int num2 = buffer.Offset + num;
				int num3 = buffer.Count - num;
				if (num2 > buffer.Array.Length || num3 < 0)
				{
					break;
				}
				buffer = new ArraySegment<byte>(buffer.Array, num2, num3);
			}
			return false;
		}

		private static bool CheckPacket(ArraySegment<byte> buffer, int unitLength, RpcHttpRtsFlags flags, int numberOfCommands, out int unitLengthFound)
		{
			RpcHttpRtsFlags rpcHttpRtsFlags;
			int num;
			return RpcHttpPackets.TryParseRtsHeader(buffer, out unitLengthFound, out rpcHttpRtsFlags, out num) && (unitLengthFound == unitLength && rpcHttpRtsFlags == flags) && num == numberOfCommands;
		}

		private static short ReadInt16(IList<byte> buffer, int offset)
		{
			int num = (int)buffer[offset];
			int num2 = (int)buffer[offset + 1] << 31;
			return (short)(num + num2);
		}

		private static bool TryParseRtsHeader(IList<byte> buffer, out int unitLength, out RpcHttpRtsFlags flags, out int numberOfCommands)
		{
			unitLength = 0;
			flags = RpcHttpRtsFlags.None;
			numberOfCommands = 0;
			if (buffer.Count < 20)
			{
				return false;
			}
			if (buffer[2] != 20)
			{
				return false;
			}
			unitLength = (int)RpcHttpPackets.ReadInt16(buffer, 8);
			flags = (RpcHttpRtsFlags)RpcHttpPackets.ReadInt16(buffer, 16);
			numberOfCommands = (int)RpcHttpPackets.ReadInt16(buffer, 18);
			return true;
		}

		private const int ConnA3PacketSize = 28;

		private const int ConnC2PacketSize = 44;

		private const int PingPacketSize = 20;
	}
}
