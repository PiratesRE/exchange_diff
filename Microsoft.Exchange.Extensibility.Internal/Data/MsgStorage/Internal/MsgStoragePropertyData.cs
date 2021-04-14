using System;
using System.IO;
using Microsoft.Exchange.Data.ContentTypes.Tnef;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	internal static class MsgStoragePropertyData
	{
		internal static TnefPropertyTag ReadPropertyTag(byte[] data, int offset)
		{
			return BitConverter.ToInt32(data, offset);
		}

		internal static int ReadPropertyByteCount(byte[] data, int propertyOffset)
		{
			return BitConverter.ToInt32(data, propertyOffset + MsgStoragePropertyData.byteCountOffset);
		}

		internal static short ReadValueAsInt16(byte[] data, int propertyOffset)
		{
			return BitConverter.ToInt16(data, propertyOffset + MsgStoragePropertyData.valueOffset);
		}

		internal static int ReadValueAsInt32(byte[] data, int propertyOffset)
		{
			return BitConverter.ToInt32(data, propertyOffset + MsgStoragePropertyData.valueOffset);
		}

		internal static long ReadValueAsInt64(byte[] data, int propertyOffset)
		{
			return BitConverter.ToInt64(data, propertyOffset + MsgStoragePropertyData.valueOffset);
		}

		internal static float ReadValueAsSingle(byte[] data, int propertyOffset)
		{
			return BitConverter.ToSingle(data, propertyOffset + MsgStoragePropertyData.valueOffset);
		}

		internal static double ReadValueAsDouble(byte[] data, int propertyOffset)
		{
			return BitConverter.ToDouble(data, propertyOffset + MsgStoragePropertyData.valueOffset);
		}

		internal static int WriteProperty(BinaryWriter writer, TnefPropertyTag propertyTag, short propertyValue)
		{
			writer.Write(propertyTag);
			writer.Write(6);
			writer.Write(propertyValue);
			writer.Write(MsgStoragePropertyData.padding, 0, 6);
			return 16;
		}

		internal static int WriteProperty(BinaryWriter writer, TnefPropertyTag propertyTag, int propertyValue)
		{
			writer.Write(propertyTag);
			writer.Write(6);
			writer.Write(propertyValue);
			writer.Write(MsgStoragePropertyData.padding, 0, 4);
			return 16;
		}

		internal static int WriteProperty(BinaryWriter writer, TnefPropertyTag propertyTag, long propertyValue)
		{
			writer.Write(propertyTag);
			writer.Write(6);
			writer.Write(propertyValue);
			return 16;
		}

		internal static int WriteProperty(BinaryWriter writer, TnefPropertyTag propertyTag, float propertyValue)
		{
			writer.Write(propertyTag);
			writer.Write(6);
			writer.Write(propertyValue);
			writer.Write(MsgStoragePropertyData.padding, 0, 4);
			return 16;
		}

		internal static int WriteProperty(BinaryWriter writer, TnefPropertyTag propertyTag, double propertyValue)
		{
			writer.Write(propertyTag);
			writer.Write(6);
			writer.Write(propertyValue);
			return 16;
		}

		internal static int WriteStream(BinaryWriter writer, TnefPropertyTag propertyTag, int streamSize)
		{
			writer.Write(propertyTag);
			writer.Write(6);
			writer.Write(streamSize);
			writer.Write(3);
			writer.Write(MsgStoragePropertyData.padding, 0, 2);
			return 16;
		}

		internal static int WriteObject(BinaryWriter writer, TnefPropertyTag propertyTag, MsgStoragePropertyData.ObjectType objectType)
		{
			writer.Write(propertyTag);
			writer.Write(6);
			writer.Write(uint.MaxValue);
			writer.Write((ushort)objectType);
			writer.Write(MsgStoragePropertyData.padding, 0, 2);
			return 16;
		}

		internal const int Size = 16;

		private static int valueOffset = 8;

		private static int byteCountOffset = 8;

		private static byte[] padding = new byte[8];

		internal enum ObjectType
		{
			Ms,
			Message,
			Attachment,
			Stream,
			Storage,
			Recipient,
			EnumStatStg
		}

		private enum PropAttribute
		{
			Readable = 2,
			Writeable = 4,
			Default = 6
		}
	}
}
