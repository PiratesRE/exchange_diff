using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Cluster.Common.Extensions
{
	internal class Serialization : ISerialization
	{
		private Serialization()
		{
		}

		public static Serialization Instance
		{
			get
			{
				return Serialization.instance;
			}
		}

		public string ObjectToXml(object obj, out Exception ex)
		{
			ex = null;
			if (obj == null)
			{
				return null;
			}
			try
			{
				using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
				{
					XmlSerializer xmlSerializer = new SafeXmlSerializer(obj.GetType());
					xmlSerializer.Serialize(stringWriter, obj);
					return stringWriter.ToString();
				}
			}
			catch (InvalidOperationException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			return null;
		}

		public string ObjectToXml(object obj)
		{
			Exception ex;
			string result = this.ObjectToXml(obj, out ex);
			if (ex != null)
			{
				throw new SerializationException(ex.Message, ex);
			}
			return result;
		}

		public object XmlToObject(string xmlText, Type objType, out Exception ex)
		{
			ex = null;
			if (xmlText == null || xmlText.Length == 0)
			{
				return null;
			}
			try
			{
				using (StringReader stringReader = new StringReader(xmlText))
				{
					XmlSerializer xmlSerializer = new SafeXmlSerializer(objType);
					return xmlSerializer.Deserialize(stringReader);
				}
			}
			catch (InvalidOperationException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			return null;
		}

		public object XmlToObject(string xmlText, Type objType)
		{
			Exception ex;
			object result = this.XmlToObject(xmlText, objType, out ex);
			if (ex != null)
			{
				throw new SerializationException(ex.Message, ex);
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

		private static Serialization instance = new Serialization();
	}
}
