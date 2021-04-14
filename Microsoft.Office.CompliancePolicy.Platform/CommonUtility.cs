using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Office.CompliancePolicy
{
	internal static class CommonUtility
	{
		public static string ObjectToString(object valueToConvert)
		{
			string empty = string.Empty;
			if (valueToConvert != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
				{
					new XmlSerializer(valueToConvert.GetType()).Serialize(stringWriter, valueToConvert);
				}
				return stringBuilder.ToString();
			}
			return empty;
		}

		public static T StringToObject<T>(string str)
		{
			T result = default(T);
			if (!string.IsNullOrEmpty(str))
			{
				using (StringReader stringReader = new StringReader(str))
				{
					result = (T)((object)new XmlSerializer(typeof(T)).Deserialize(XmlReader.Create(stringReader)));
				}
			}
			return result;
		}

		public static byte[] ObjectToBytes(object valueToConvert)
		{
			byte[] result = null;
			if (valueToConvert != null)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
					binaryFormatter.Serialize(memoryStream, valueToConvert);
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		public static object BytesToObject(byte[] valueToConvert)
		{
			object result = null;
			if (valueToConvert != null)
			{
				using (MemoryStream memoryStream = new MemoryStream(valueToConvert))
				{
					BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
					binaryFormatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
					result = binaryFormatter.Deserialize(memoryStream);
				}
			}
			return result;
		}
	}
}
