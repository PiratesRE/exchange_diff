using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Cluster.Common.Extensions;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.EseRepl
{
	internal static class SerializationUtil
	{
		public static void ThrowGeneralSerializationException(Exception ex)
		{
			string text = ex.ToString() + Environment.StackTrace;
			ReplayCrimsonEvents.GeneralSerializationError.LogPeriodic<string>(Environment.MachineName, Parameters.CurrentValues.DefaultEventSuppressionInterval, text);
			if (ex is SerializationException)
			{
				throw ex;
			}
			throw new SerializationException(ex.Message, ex);
		}

		public static string ObjectToXml(object obj, out Exception ex)
		{
			return Serialization.Instance.ObjectToXml(obj, out ex);
		}

		public static string ObjectToXml(object obj)
		{
			Exception ex;
			string result = SerializationUtil.ObjectToXml(obj, out ex);
			if (ex != null)
			{
				SerializationUtil.ThrowGeneralSerializationException(ex);
			}
			return result;
		}

		public static object XmlToObject(string xmlText, Type objType, out Exception ex)
		{
			return Serialization.Instance.XmlToObject(xmlText, objType, out ex);
		}

		public static object XmlToObject(string xmlText, Type objType)
		{
			Exception ex;
			object result = SerializationUtil.XmlToObject(xmlText, objType, out ex);
			if (ex != null)
			{
				SerializationUtil.ThrowGeneralSerializationException(ex);
			}
			return result;
		}

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
	}
}
