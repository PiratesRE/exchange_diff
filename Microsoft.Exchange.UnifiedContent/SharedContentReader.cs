using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.UnifiedContent
{
	internal class SharedContentReader : BinaryReader
	{
		internal SharedContentReader(Stream stream) : base(stream, Encoding.Unicode)
		{
		}

		public override string ReadString()
		{
			long num = this.ReadInt64();
			if (num % 2L != 0L)
			{
				throw new InvalidDataException();
			}
			int num2 = (int)(num / 2L);
			char[] array = new char[num2];
			int num3 = this.Read(array, 0, num2);
			if (num3 != num2)
			{
				throw new InvalidDataException();
			}
			if (array[num2 - 1] != '\0')
			{
				throw new InvalidDataException();
			}
			return new string(array, 0, num2 - 1);
		}

		internal void ValidateEntryId(uint entryIdToValidate)
		{
			uint num = this.ReadUInt32();
			if (num != entryIdToValidate)
			{
				throw new InvalidDataException();
			}
		}

		internal Stream ReadStream()
		{
			long num = this.ReadInt64();
			long position = this.BaseStream.Position;
			long num2 = position + num;
			if (num2 > this.BaseStream.Length)
			{
				throw new InvalidDataException();
			}
			this.BaseStream.Position = num2;
			return new StreamOnStream(this.BaseStream, position, num);
		}

		internal byte[] ReadBuffer()
		{
			int num = this.ReadInt32();
			byte[] array = new byte[num];
			this.Read(array, 0, num);
			return array;
		}

		internal void ValidateAtEndOfEntry()
		{
			if (this.BaseStream.Position != this.BaseStream.Length)
			{
				throw new FormatException("Shared Content Entry invalid");
			}
		}
	}
}
