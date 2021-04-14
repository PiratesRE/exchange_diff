using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public static class JsonConverter
	{
		public static string ToJSON(object instance)
		{
			return JsonConverter.ToJSON(instance, Array<Type>.Empty);
		}

		public static string ToJSON(object instance, IEnumerable<Type> knownTypes)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (knownTypes == null)
			{
				throw new ArgumentNullException("knownTypes");
			}
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(instance.GetType(), knownTypes);
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				dataContractJsonSerializer.WriteObject(memoryStream, instance);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (StreamReader streamReader = new StreamReader(memoryStream))
				{
					result = streamReader.ReadToEnd();
				}
			}
			return result;
		}

		public static T FromJSON<T>(string jsonString)
		{
			return JsonConverter.FromJSON<T>(jsonString, Array<Type>.Empty);
		}

		public static T FromJSON<T>(string jsonString, IEnumerable<Type> knownTypes)
		{
			if (jsonString == null)
			{
				throw new ArgumentNullException("jsonString");
			}
			if (knownTypes == null)
			{
				throw new ArgumentNullException("knownTypes");
			}
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T), knownTypes);
			T result;
			using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
			{
				result = (T)((object)dataContractJsonSerializer.ReadObject(stream));
			}
			return result;
		}
	}
}
