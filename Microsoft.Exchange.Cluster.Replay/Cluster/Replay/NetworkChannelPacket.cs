using System;
using System.Text;
using Microsoft.Exchange.Conversion;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class NetworkChannelPacket
	{
		internal byte[] Buffer
		{
			get
			{
				return this.m_buf;
			}
		}

		internal int Position
		{
			get
			{
				return this.m_position;
			}
			set
			{
				this.m_position = value;
			}
		}

		internal int Length
		{
			get
			{
				return this.m_length;
			}
		}

		internal int Capacity
		{
			get
			{
				if (this.m_buf == null)
				{
					return 0;
				}
				return this.m_buf.Length;
			}
		}

		internal void SetCapacity(int lenNeeded)
		{
			if (this.m_buf == null)
			{
				this.m_buf = new byte[lenNeeded];
				return;
			}
			if (this.m_buf.Length < lenNeeded)
			{
				int num = Math.Max(lenNeeded, this.m_buf.Length * 2);
				byte[] array = new byte[num];
				Array.Copy(this.m_buf, 0, array, 0, this.m_buf.Length);
				this.m_buf = array;
			}
		}

		internal void SetLength(int newLen)
		{
			if (newLen > this.Capacity)
			{
				this.SetCapacity(newLen);
				this.m_length = newLen;
			}
		}

		private static ushort CheckStringLen(int expectedLen)
		{
			if (expectedLen > 65535)
			{
				NetworkChannel.StaticTraceError("CheckStringLen: {0} exceeds max string len: {1}", new object[]
				{
					expectedLen,
					65535
				});
				throw new NetworkDataOverflowGenericException();
			}
			return (ushort)expectedLen;
		}

		private static int CheckByteArrayLen(int expectedLen)
		{
			if (expectedLen > 2097152)
			{
				NetworkChannel.StaticTraceError("CheckByteArrayLen: {0} exceeds max byteArray len: {1}", new object[]
				{
					expectedLen,
					2097152
				});
				throw new NetworkDataOverflowGenericException();
			}
			return expectedLen;
		}

		private void MakeSpaceToAppend(int len)
		{
			if (this.GrowthDisabled)
			{
				return;
			}
			int num = this.m_position + len;
			this.SetCapacity(num);
			this.m_length = num;
		}

		internal NetworkChannelPacket()
		{
		}

		internal NetworkChannelPacket(int size)
		{
			this.m_buf = new byte[size];
		}

		internal NetworkChannelPacket(byte[] initBuf)
		{
			this.m_buf = initBuf;
			this.m_position = 0;
			this.m_length = this.m_buf.Length;
		}

		internal NetworkChannelPacket(byte[] initBuf, int initialWritePos)
		{
			this.m_buf = initBuf;
			this.m_position = initialWritePos;
			this.m_length = this.m_buf.Length;
		}

		internal bool GrowthDisabled { get; set; }

		internal void PrepareToWrite()
		{
			this.m_position = 0;
		}

		internal void Append(int val)
		{
			this.MakeSpaceToAppend(4);
			ExBitConverter.Write(val, this.m_buf, this.m_position);
			this.m_position += 4;
		}

		internal void Append(uint val)
		{
			this.MakeSpaceToAppend(4);
			ExBitConverter.Write(val, this.m_buf, this.m_position);
			this.m_position += 4;
		}

		internal void Append(long val)
		{
			this.MakeSpaceToAppend(8);
			ExBitConverter.Write(val, this.m_buf, this.m_position);
			this.m_position += 8;
		}

		internal void Append(ulong val)
		{
			this.MakeSpaceToAppend(8);
			ExBitConverter.Write(val, this.m_buf, this.m_position);
			this.m_position += 8;
		}

		internal void Append(ushort val)
		{
			this.MakeSpaceToAppend(2);
			ExBitConverter.Write(val, this.m_buf, this.m_position);
			this.m_position += 2;
		}

		internal void Append(DateTime time)
		{
			long val = time.ToBinary();
			this.Append(val);
		}

		internal void Append(Guid g)
		{
			this.MakeSpaceToAppend(16);
			ExBitConverter.Write(g, this.m_buf, this.m_position);
			this.m_position += 16;
		}

		internal void Append(byte b)
		{
			this.MakeSpaceToAppend(1);
			this.m_buf[this.m_position++] = b;
		}

		internal void Append(bool b)
		{
			byte b2 = b ? 1 : 0;
			this.Append(b2);
		}

		internal void Append(byte[] inBuf, int inOffset, int len)
		{
			this.MakeSpaceToAppend(len);
			Array.Copy(inBuf, inOffset, this.m_buf, this.m_position, len);
			this.m_position += len;
		}

		internal void Append(byte[] buf, int len)
		{
			this.Append(buf, 0, len);
		}

		internal void Append(byte[] buf)
		{
			this.Append(buf, 0, buf.Length);
		}

		internal bool ExtractBool()
		{
			byte b = this.ExtractUInt8();
			return b != 0;
		}

		internal byte ExtractUInt8()
		{
			return this.m_buf[this.m_position++];
		}

		internal ushort ExtractUInt16()
		{
			ushort result = BitConverter.ToUInt16(this.m_buf, this.m_position);
			this.m_position += 2;
			return result;
		}

		internal uint ExtractUInt32()
		{
			uint result = BitConverter.ToUInt32(this.m_buf, this.m_position);
			this.m_position += 4;
			return result;
		}

		internal int ExtractInt32()
		{
			int result = BitConverter.ToInt32(this.m_buf, this.m_position);
			this.m_position += 4;
			return result;
		}

		internal ulong ExtractUInt64()
		{
			ulong result = BitConverter.ToUInt64(this.m_buf, this.m_position);
			this.m_position += 8;
			return result;
		}

		internal long ExtractInt64()
		{
			long result = BitConverter.ToInt64(this.m_buf, this.m_position);
			this.m_position += 8;
			return result;
		}

		internal DateTime ExtractDateTime()
		{
			long dateData = this.ExtractInt64();
			return DateTime.FromBinary(dateData);
		}

		internal Guid ExtractGuid()
		{
			byte[] b = this.ExtractBytes(16);
			return new Guid(b);
		}

		internal byte[] ExtractBytes(int len)
		{
			byte[] array = new byte[len];
			Array.Copy(this.m_buf, this.m_position, array, 0, len);
			this.m_position += len;
			return array;
		}

		internal byte[] ExtractBytes(int len, out int position)
		{
			position = this.m_position;
			this.m_position += len;
			return this.m_buf;
		}

		public static byte[] EncodeString(string str)
		{
			return Encoding.UTF8.GetBytes(str);
		}

		public static string DecodeString(byte[] buf, int offset, int len)
		{
			return Encoding.UTF8.GetString(buf, offset, len);
		}

		public void Append(string str)
		{
			ushort num = 0;
			if (str != null)
			{
				byte[] array = NetworkChannelPacket.EncodeString(str);
				int expectedLen = array.Length;
				num = NetworkChannelPacket.CheckStringLen(expectedLen);
				this.Append(num);
				this.Append(array, (int)num);
				return;
			}
			this.Append(num);
		}

		public string ExtractString()
		{
			ushort num = this.ExtractUInt16();
			int num2 = this.Length - this.Position;
			if ((int)num > num2)
			{
				NetworkChannel.StaticTraceError("ExtractString: {0} exceeds max string len: {1}", new object[]
				{
					num,
					num2
				});
				throw new NetworkCorruptDataGenericException();
			}
			string result;
			if (num > 0)
			{
				result = NetworkChannelPacket.DecodeString(this.m_buf, this.Position, (int)num);
				this.m_position += (int)num;
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		public void AppendByteArray(byte[] bytes)
		{
			int num = bytes.Length;
			NetworkChannelPacket.CheckByteArrayLen(num);
			this.Append(num);
			this.Append(bytes, 0, num);
		}

		public byte[] ExtractByteArray()
		{
			int num = this.ExtractInt32();
			int num2 = this.Length - this.Position;
			if (num > num2 || num < 0)
			{
				NetworkChannel.StaticTraceError("ExtractByteArray: {0} exceeds max byte len: {1}", new object[]
				{
					num,
					num2
				});
				throw new NetworkCorruptDataGenericException();
			}
			return this.ExtractBytes(num);
		}

		private const int GuidLen = 16;

		public const int MaxStringLength = 65535;

		public const int MaxByteArrayLength = 2097152;

		protected int m_length;

		protected int m_position;

		protected byte[] m_buf;
	}
}
