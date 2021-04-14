using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class ErrorJsonDeserializer
	{
		public static ErrorInformation Deserialize(Stream stream)
		{
			ErrorInformation result;
			try
			{
				StreamReader streamReader = new StreamReader(stream);
				IDictionary dictionary = ErrorJsonDeserializer.jsSerializer.Deserialize<Dictionary<string, object>>(streamReader.ReadToEnd());
				ErrorCode code = ErrorCode.Unknown;
				if (dictionary.Contains("code"))
				{
					Enum.TryParse<ErrorCode>(dictionary["code"].ToString(), out code);
				}
				ErrorSubcode subcode = ErrorSubcode.None;
				if (dictionary.Contains("subcode"))
				{
					Enum.TryParse<ErrorSubcode>(dictionary["subcode"].ToString(), out subcode);
				}
				string message = null;
				if (dictionary.Contains("message"))
				{
					message = (dictionary["message"] as string);
				}
				ErrorInformation errorInformation = new ErrorInformation
				{
					Code = code,
					Subcode = subcode,
					Message = message
				};
				if (dictionary.Contains("debugInfo"))
				{
					IDictionary dictionary2 = dictionary["debugInfo"] as IDictionary;
					foreach (object obj in dictionary2.Keys)
					{
						string key = (string)obj;
						errorInformation.DebugInfo.Add(key, dictionary2[key] as string);
					}
				}
				if (dictionary.Contains("parameters"))
				{
					IEnumerable enumerable = dictionary["parameters"] as IEnumerable;
					foreach (object obj2 in enumerable)
					{
						IDictionary dictionary3 = (IDictionary)obj2;
						errorInformation.Parameters[dictionary3["name"] as string] = (dictionary3["reason"] as string);
					}
				}
				result = errorInformation;
			}
			catch (Exception innerException)
			{
				throw new SerializationException("Failed to deserialize a UCWA error response", innerException);
			}
			return result;
		}

		private static readonly JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

		private static class StringConstants
		{
			public const string Code = "code";

			public const string Subcode = "subcode";

			public const string Message = "message";

			public const string DebugInfo = "debugInfo";

			public const string Parameters = "parameters";

			public const string Name = "name";

			public const string Reason = "reason";
		}
	}
}
