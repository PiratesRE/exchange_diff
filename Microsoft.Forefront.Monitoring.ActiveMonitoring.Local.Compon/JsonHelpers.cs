using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class JsonHelpers
	{
		public static string SerializeJson(IEnumerable<Type> knownTypes, Type t, object o)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(t, knownTypes);
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				dataContractJsonSerializer.WriteObject(memoryStream, o);
				memoryStream.Position = 0L;
				result = new StreamReader(memoryStream).ReadToEnd();
			}
			return result;
		}

		public static object DeserializeJson(IEnumerable<Type> knownTypes, Type t, string s)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(t, knownTypes);
			object result;
			using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(s)))
			{
				result = dataContractJsonSerializer.ReadObject(memoryStream);
			}
			return result;
		}
	}
}
