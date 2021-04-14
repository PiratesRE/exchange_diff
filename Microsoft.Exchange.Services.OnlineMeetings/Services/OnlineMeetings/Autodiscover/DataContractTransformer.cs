using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Microsoft.Exchange.Services.OnlineMeetings.Autodiscover.DataContract;

namespace Microsoft.Exchange.Services.OnlineMeetings.Autodiscover
{
	internal static class DataContractTransformer
	{
		public static AutodiscoverResponse TransformResponse(string responseBody)
		{
			AutodiscoverResponse result;
			try
			{
				Dictionary<string, object> dictionary = DataContractTransformer.jsSerializer.Deserialize<Dictionary<string, object>>(responseBody);
				AutodiscoverResponse autodiscoverResponse = AutodiscoverResponse.FromDictionary(dictionary);
				result = autodiscoverResponse;
			}
			catch (InvalidOperationException innerException)
			{
				throw new SerializationException("An exception occurred during response deserialization.", innerException);
			}
			return result;
		}

		private static readonly JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
	}
}
