using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.Exchange.Conversion
{
	internal class JsonConverter
	{
		public static string Serialize<T>(T obj, IEnumerable<Type> types = null)
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				JsonConverter.Serialize<T>(obj, memoryStream, types);
				byte[] array = memoryStream.ToArray();
				string @string = Encoding.UTF8.GetString(array, 0, array.Length);
				result = @string;
			}
			return result;
		}

		public static string Serialize<T>(T obj, IEnumerable<Type> types, DateTimeFormat dateTimeFormat)
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				JsonConverter.Serialize<T>(obj, memoryStream, types, dateTimeFormat);
				byte[] array = memoryStream.ToArray();
				string @string = Encoding.UTF8.GetString(array, 0, array.Length);
				result = @string;
			}
			return result;
		}

		public static void Serialize<T>(T obj, Stream stream, IEnumerable<Type> types = null)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T), types);
			dataContractJsonSerializer.WriteObject(stream, obj);
		}

		public static void Serialize<T>(T obj, Stream stream, IEnumerable<Type> types, DateTimeFormat dateTimeFormat)
		{
			DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings
			{
				DateTimeFormat = dateTimeFormat,
				KnownTypes = types
			};
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T), settings);
			dataContractJsonSerializer.WriteObject(stream, obj);
		}

		public static T Deserialize<T>(string json, IEnumerable<Type> types = null)
		{
			T result;
			using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
			{
				result = JsonConverter.Deserialize<T>(memoryStream, types);
			}
			return result;
		}

		public static T Deserialize<T>(string json, IEnumerable<Type> types, DateTimeFormat dateTimeFormat)
		{
			T result;
			using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
			{
				result = JsonConverter.Deserialize<T>(memoryStream, types, dateTimeFormat);
			}
			return result;
		}

		public static T Deserialize<T>(Stream stream, IEnumerable<Type> types = null)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T), types);
			return (T)((object)dataContractJsonSerializer.ReadObject(stream));
		}

		public static T Deserialize<T>(Stream stream, IEnumerable<Type> types, DateTimeFormat dateTimeFormat)
		{
			DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings
			{
				DateTimeFormat = dateTimeFormat,
				KnownTypes = types
			};
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T), settings);
			return (T)((object)dataContractJsonSerializer.ReadObject(stream));
		}

		public static readonly DateTimeFormat RoundTripDateTimeFormat = new DateTimeFormat("O");
	}
}
