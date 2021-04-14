using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Serialization
{
	internal static class Serialization
	{
		public static byte[] ObjectToBytes(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			byte[] buffer;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				binaryFormatter.Serialize(memoryStream, obj);
				buffer = memoryStream.GetBuffer();
			}
			return buffer;
		}

		public static object BytesToObject(byte[] mBinaryData)
		{
			if (mBinaryData == null || mBinaryData.Length == 0)
			{
				return null;
			}
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			object result;
			using (MemoryStream memoryStream = new MemoryStream(mBinaryData, false))
			{
				result = binaryFormatter.Deserialize(memoryStream);
			}
			return result;
		}

		public static string ObjectToXML(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
				xmlSerializer.Serialize(stringWriter, obj);
				result = stringWriter.ToString();
			}
			return result;
		}

		public static object XMLToObject(string xmlText, Type objType)
		{
			if (xmlText == null || xmlText.Length == 0)
			{
				return null;
			}
			object result;
			using (StringReader stringReader = new StringReader(xmlText))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(objType);
				result = xmlSerializer.Deserialize(stringReader);
			}
			return result;
		}

		public static void SerializeUInt64(byte[] buf, ref int bytePos, ulong val)
		{
			for (int i = 0; i < 8; i++)
			{
				buf[bytePos++] = (byte)(val & 255UL);
				val >>= 8;
			}
		}

		public static void SerializeUInt32(byte[] buf, ref int bytePos, uint val)
		{
			for (int i = 0; i < 4; i++)
			{
				buf[bytePos++] = (byte)(val & 255U);
				val >>= 8;
			}
		}

		public static void SerializeUInt16(byte[] buf, ref int bytePos, ushort val)
		{
			buf[bytePos++] = (byte)(val & 255);
			buf[bytePos++] = (byte)(val >> 8 & 255);
		}

		public static void SerializeGuid(byte[] buf, ref int bytePos, Guid val)
		{
			byte[] array = val.ToByteArray();
			Buffer.BlockCopy(array, 0, buf, bytePos, array.Length);
			bytePos += array.Length;
		}

		public static ulong DeserializeUInt64(byte[] buf, ref int bytePos)
		{
			ulong num = 0UL;
			for (int i = 7; i >= 0; i--)
			{
				num <<= 8;
				num |= (ulong)buf[i + bytePos];
			}
			bytePos += 8;
			return num;
		}

		public static uint DeserializeUInt32(byte[] buf, ref int bytePos)
		{
			uint num = 0U;
			for (int i = 3; i >= 0; i--)
			{
				num <<= 8;
				num |= (uint)buf[i + bytePos];
			}
			bytePos += 4;
			return num;
		}

		public static ushort DeserializeUInt16(byte[] buf, ref int bytePos)
		{
			ushort num = (ushort)buf[bytePos++];
			return num | (ushort)(buf[bytePos++] << 8);
		}

		public static Guid DeserializeGuid(byte[] buf, ref int bytePos)
		{
			Guid result = new Guid(BitConverter.ToInt32(buf, bytePos), BitConverter.ToInt16(buf, bytePos + 4), BitConverter.ToInt16(buf, bytePos + 6), buf[bytePos + 8], buf[bytePos + 9], buf[bytePos + 10], buf[bytePos + 11], buf[bytePos + 12], buf[bytePos + 13], buf[bytePos + 14], buf[bytePos + 15]);
			bytePos += 16;
			return result;
		}

		public static void SerializeDateTime(byte[] buf, ref int bytePos, DateTime val)
		{
			long val2 = val.ToBinary();
			Serialization.SerializeUInt64(buf, ref bytePos, (ulong)val2);
		}

		public static DateTime DeserializeDateTime(byte[] buf, ref int bytePos)
		{
			long dateData = (long)Serialization.DeserializeUInt64(buf, ref bytePos);
			return DateTime.FromBinary(dateData);
		}

		public const int DateTimeSerializationLength = 8;
	}
}
