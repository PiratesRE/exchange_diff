using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class JsonExtension
	{
		public static string ToJsonString(this object dataContract, IEnumerable<Type> types = null)
		{
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(dataContract.GetType(), types, int.MaxValue, false, null, true);
			string text = JsonExtension.ToJsonStringCore(serializer, dataContract);
			if (!string.IsNullOrEmpty(text) && JsonExtension.unsafeCharactersRegex.IsMatch(text))
			{
				text = JsonExtension.unsafeCharactersRegex.Replace(text, new MatchEvaluator(JsonExtension.ReplaceWithEscapedCode));
			}
			return text;
		}

		private static string ToJsonStringCore(DataContractJsonSerializer serializer, object dataContract)
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				serializer.WriteObject(memoryStream, dataContract);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (StreamReader streamReader = new StreamReader(memoryStream))
				{
					result = streamReader.ReadToEnd();
				}
			}
			return result;
		}

		private static string ReplaceWithEscapedCode(Match m)
		{
			string value = m.Value;
			string text = ((int)value[0]).ToString("X");
			return "\\u0000".Substring(0, 6 - text.Length) + text;
		}

		public static T JsonDeserialize<T>(this string text, IEnumerable<Type> types = null)
		{
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), types);
			return (T)((object)JsonExtension.JsonDeserializeCore(text, serializer));
		}

		public static object JsonDeserialize(this string text, Type targetType, IEnumerable<Type> types = null)
		{
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(targetType, types);
			return JsonExtension.JsonDeserializeCore(text, serializer);
		}

		private static object JsonDeserializeCore(string text, DataContractJsonSerializer serializer)
		{
			object result;
			using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(text)))
			{
				result = serializer.ReadObject(memoryStream);
			}
			return result;
		}

		private const string UnsafeCharacters = "[\u007f-\u009f­؀-؄܏឴឵‌-‏\u2028-\u202f⁠-⁯﻿￰-￿]";

		private static Regex unsafeCharactersRegex = new Regex("[\u007f-\u009f­؀-؄܏឴឵‌-‏\u2028-\u202f⁠-⁯﻿￰-￿]", RegexOptions.Compiled);
	}
}
