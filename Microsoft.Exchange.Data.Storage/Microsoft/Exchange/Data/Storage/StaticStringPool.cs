using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class StaticStringPool : DataPoolBase<string>
	{
		public static StaticStringPool Instance
		{
			get
			{
				return StaticStringPool.instance;
			}
		}

		protected override void ProcessStream(BinaryReader reader, ComponentDataPool componentDataPool, out uint hashCode, out int startIndex, out int length)
		{
			MemoryStream memoryStream = (MemoryStream)componentDataPool.ConstStringDataReader.BaseStream;
			memoryStream.Seek(0L, SeekOrigin.Begin);
			length = this.Read7BitEncodedInt(reader, memoryStream);
			if (length < 0)
			{
				throw new FormatException("size tag should >= 0");
			}
			startIndex = (int)memoryStream.Position;
			memoryStream.SetLength((long)(startIndex + length));
			byte[] copyBuffer = componentDataPool.CopyBuffer;
			int num;
			for (int i = length; i > 0; i -= num)
			{
				num = ((i > copyBuffer.Length) ? copyBuffer.Length : i);
				if (reader.Read(copyBuffer, 0, num) != num)
				{
					throw new FormatException("there are less bytes than what tag says");
				}
				memoryStream.Write(copyBuffer, 0, num);
			}
			hashCode = ComputeCRC.Compute(0U, memoryStream.GetBuffer(), startIndex, length);
		}

		protected override void ProcessData(string data, out uint hashCode, out byte[] bytes)
		{
			bytes = Encoding.UTF8.GetBytes(data);
			hashCode = ComputeCRC.Compute(0U, bytes);
		}

		private int Read7BitEncodedInt(BinaryReader reader, MemoryStream outputStream)
		{
			int num = 0;
			int num2 = 0;
			while (num2 != 35)
			{
				byte b = reader.ReadByte();
				outputStream.WriteByte(b);
				num |= (int)(b & 127) << num2;
				num2 += 7;
				if ((b & 128) == 0)
				{
					return num;
				}
			}
			throw new FormatException("Format_Bad7BitInt32");
		}

		private static StaticStringPool instance = new StaticStringPool();
	}
}
