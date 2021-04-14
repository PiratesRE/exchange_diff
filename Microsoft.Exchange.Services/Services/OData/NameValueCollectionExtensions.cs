using System;
using System.Collections.Specialized;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.OData
{
	internal static class NameValueCollectionExtensions
	{
		public static TEnum? GetQueryEnumValue<TEnum>(this NameValueCollection queryString, string paramName) where TEnum : struct
		{
			ArgumentValidator.ThrowIfNull("queryString", queryString);
			ArgumentValidator.ThrowIfNullOrEmpty("paramName", paramName);
			TEnum? result = null;
			string value = queryString[paramName];
			TEnum value2;
			if (!string.IsNullOrEmpty(value) && Enum.TryParse<TEnum>(value, out value2))
			{
				result = new TEnum?(value2);
			}
			return result;
		}

		public static bool GetCountQueryString(this NameValueCollection queryString)
		{
			ArgumentValidator.ThrowIfNull("queryString", queryString);
			string text = queryString["$count"];
			if (text == null)
			{
				return false;
			}
			bool result;
			if (bool.TryParse(text, out result))
			{
				return result;
			}
			throw new InvalidValueForCountSystemQueryOptionException();
		}
	}
}
