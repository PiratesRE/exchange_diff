using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class BinaryReaderExtensions
	{
		public static ushort ReadUInt16(this BinaryReader reader, string elementName)
		{
			return BinaryReaderExtensions.ReadAndHandleExceptions<ushort>(() => reader.ReadUInt16(), reader.BaseStream.Position, elementName);
		}

		public static uint ReadUInt32(this BinaryReader reader, string elementName)
		{
			return BinaryReaderExtensions.ReadAndHandleExceptions<uint>(() => reader.ReadUInt32(), reader.BaseStream.Position, elementName);
		}

		public static ulong ReadUInt64(this BinaryReader reader, string elementName)
		{
			return BinaryReaderExtensions.ReadAndHandleExceptions<ulong>(() => reader.ReadUInt64(), reader.BaseStream.Position, elementName);
		}

		public static byte ReadByte(this BinaryReader reader, string elementName)
		{
			return BinaryReaderExtensions.ReadAndHandleExceptions<byte>(() => reader.ReadByte(), reader.BaseStream.Position, elementName);
		}

		public static Guid ReadGuid(this BinaryReader reader, string elementName)
		{
			byte[] b = reader.ReadBytes(16, elementName);
			return new Guid(b);
		}

		public static byte[] ReadBytes(this BinaryReader reader, int count, string elementName)
		{
			long position = reader.BaseStream.Position;
			byte[] array = BinaryReaderExtensions.ReadAndHandleExceptions<byte[]>(() => reader.ReadBytes(count), position, elementName);
			if (array.Length < count)
			{
				throw new InvalidDataException(string.Format("Unable to completely read '{0}' element from stream at position {1}, only {2} were read instead of {3}", new object[]
				{
					elementName,
					position,
					array.Length,
					count
				}));
			}
			return array;
		}

		private static T ReadAndHandleExceptions<T>(Func<T> read, long position, string elementName)
		{
			T result;
			try
			{
				result = read();
			}
			catch (EndOfStreamException innerException)
			{
				throw new InvalidDataException(string.Format("Unexpected end of stream when reading '{0}' element from stream at position {1}", elementName, position), innerException);
			}
			return result;
		}
	}
}
