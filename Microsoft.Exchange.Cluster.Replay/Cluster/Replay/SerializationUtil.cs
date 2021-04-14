using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Cluster.Common.Extensions;
using Microsoft.Exchange.Cluster.ReplayEventLog;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class SerializationUtil
	{
		public static void ThrowGeneralSerializationException(Exception ex)
		{
			string text = ex.ToString() + Environment.StackTrace;
			ReplayCrimsonEvents.GeneralSerializationError.LogPeriodic<string>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, text);
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
	}
}
