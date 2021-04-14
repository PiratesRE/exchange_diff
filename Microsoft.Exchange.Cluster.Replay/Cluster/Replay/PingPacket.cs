using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class PingPacket
	{
		public static byte[] FormPacket()
		{
			PingPacket pingPacket = new PingPacket();
			pingPacket.Type = 8;
			pingPacket.SubCode = 0;
			pingPacket.CheckSum = 0;
			pingPacket.Identifier = 45;
			pingPacket.SequenceNumber = 0;
			for (int i = 0; i < 32; i++)
			{
				pingPacket.Data[i] = 35;
			}
			byte[] value = PingPacket.Serialize(pingPacket);
			ushort[] array = new ushort[20];
			for (int j = 0; j < 20; j++)
			{
				array[j] = BitConverter.ToUInt16(value, j * 2);
			}
			pingPacket.CheckSum = PingPacket.Checksum(array, 20);
			return PingPacket.Serialize(pingPacket);
		}

		public static byte[] Serialize(PingPacket packetInfo)
		{
			byte[] array = new byte[40];
			array[0] = packetInfo.Type;
			array[1] = packetInfo.SubCode;
			int num = 2;
			byte[] bytes = BitConverter.GetBytes(packetInfo.CheckSum);
			Array.Copy(bytes, 0, array, num, bytes.Length);
			num += bytes.Length;
			bytes = BitConverter.GetBytes(packetInfo.Identifier);
			Array.Copy(bytes, 0, array, num, bytes.Length);
			num += bytes.Length;
			bytes = BitConverter.GetBytes(packetInfo.SequenceNumber);
			Array.Copy(bytes, 0, array, num, bytes.Length);
			num += bytes.Length;
			Array.Copy(packetInfo.Data, 0, array, num, packetInfo.Data.Length);
			num += packetInfo.Data.Length;
			return array;
		}

		public static ushort Checksum(ushort[] buffer, int size)
		{
			int num = 0;
			int num2 = 0;
			while (size > 0)
			{
				num += Convert.ToInt32(buffer[num2]);
				num2++;
				size--;
			}
			num = (num >> 16) + (num & 65535);
			num += num >> 16;
			return (ushort)(~(ushort)num);
		}

		private const int ICMP_ECHO = 8;

		private const int PacketSize = 40;

		private const int PacketDataLen = 32;

		public byte Type;

		public byte SubCode;

		public ushort CheckSum;

		public ushort Identifier;

		public ushort SequenceNumber;

		public byte[] Data = new byte[32];
	}
}
