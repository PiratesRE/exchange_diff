using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal static class Utils
	{
		public static object SafeGetProperty(StoreObject message, PropertyDefinition propertyDefinition, object defaultValue)
		{
			if (message == null)
			{
				return defaultValue;
			}
			object obj = message.TryGetProperty(propertyDefinition);
			if (obj == null || obj is PropertyError)
			{
				return defaultValue;
			}
			return obj;
		}

		internal static string GetOUFromLegacyDN(string legacyDN)
		{
			string text = legacyDN.ToUpper();
			int num = text.IndexOf("OU=") + 3;
			int num2 = text.IndexOf("/CN=");
			return text.Substring(num, num2 - num);
		}
	}
}
