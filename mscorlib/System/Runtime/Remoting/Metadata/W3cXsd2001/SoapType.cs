using System;
using System.Text;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	internal static class SoapType
	{
		internal static string FilterBin64(string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] != ' ' && value[i] != '\n' && value[i] != '\r')
				{
					stringBuilder.Append(value[i]);
				}
			}
			return stringBuilder.ToString();
		}

		internal static string LineFeedsBin64(string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < value.Length; i++)
			{
				if (i % 76 == 0)
				{
					stringBuilder.Append('\n');
				}
				stringBuilder.Append(value[i]);
			}
			return stringBuilder.ToString();
		}

		internal static string Escape(string value)
		{
			if (value == null || value.Length == 0)
			{
				return value;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = value.IndexOf('&');
			if (num > -1)
			{
				stringBuilder.Append(value);
				stringBuilder.Replace("&", "&#38;", num, stringBuilder.Length - num);
			}
			num = value.IndexOf('"');
			if (num > -1)
			{
				if (stringBuilder.Length == 0)
				{
					stringBuilder.Append(value);
				}
				stringBuilder.Replace("\"", "&#34;", num, stringBuilder.Length - num);
			}
			num = value.IndexOf('\'');
			if (num > -1)
			{
				if (stringBuilder.Length == 0)
				{
					stringBuilder.Append(value);
				}
				stringBuilder.Replace("'", "&#39;", num, stringBuilder.Length - num);
			}
			num = value.IndexOf('<');
			if (num > -1)
			{
				if (stringBuilder.Length == 0)
				{
					stringBuilder.Append(value);
				}
				stringBuilder.Replace("<", "&#60;", num, stringBuilder.Length - num);
			}
			num = value.IndexOf('>');
			if (num > -1)
			{
				if (stringBuilder.Length == 0)
				{
					stringBuilder.Append(value);
				}
				stringBuilder.Replace(">", "&#62;", num, stringBuilder.Length - num);
			}
			num = value.IndexOf('\0');
			if (num > -1)
			{
				if (stringBuilder.Length == 0)
				{
					stringBuilder.Append(value);
				}
				stringBuilder.Replace('\0'.ToString(), "&#0;", num, stringBuilder.Length - num);
			}
			string result;
			if (stringBuilder.Length > 0)
			{
				result = stringBuilder.ToString();
			}
			else
			{
				result = value;
			}
			return result;
		}

		internal static Type typeofSoapTime = typeof(SoapTime);

		internal static Type typeofSoapDate = typeof(SoapDate);

		internal static Type typeofSoapYearMonth = typeof(SoapYearMonth);

		internal static Type typeofSoapYear = typeof(SoapYear);

		internal static Type typeofSoapMonthDay = typeof(SoapMonthDay);

		internal static Type typeofSoapDay = typeof(SoapDay);

		internal static Type typeofSoapMonth = typeof(SoapMonth);

		internal static Type typeofSoapHexBinary = typeof(SoapHexBinary);

		internal static Type typeofSoapBase64Binary = typeof(SoapBase64Binary);

		internal static Type typeofSoapInteger = typeof(SoapInteger);

		internal static Type typeofSoapPositiveInteger = typeof(SoapPositiveInteger);

		internal static Type typeofSoapNonPositiveInteger = typeof(SoapNonPositiveInteger);

		internal static Type typeofSoapNonNegativeInteger = typeof(SoapNonNegativeInteger);

		internal static Type typeofSoapNegativeInteger = typeof(SoapNegativeInteger);

		internal static Type typeofSoapAnyUri = typeof(SoapAnyUri);

		internal static Type typeofSoapQName = typeof(SoapQName);

		internal static Type typeofSoapNotation = typeof(SoapNotation);

		internal static Type typeofSoapNormalizedString = typeof(SoapNormalizedString);

		internal static Type typeofSoapToken = typeof(SoapToken);

		internal static Type typeofSoapLanguage = typeof(SoapLanguage);

		internal static Type typeofSoapName = typeof(SoapName);

		internal static Type typeofSoapIdrefs = typeof(SoapIdrefs);

		internal static Type typeofSoapEntities = typeof(SoapEntities);

		internal static Type typeofSoapNmtoken = typeof(SoapNmtoken);

		internal static Type typeofSoapNmtokens = typeof(SoapNmtokens);

		internal static Type typeofSoapNcName = typeof(SoapNcName);

		internal static Type typeofSoapId = typeof(SoapId);

		internal static Type typeofSoapIdref = typeof(SoapIdref);

		internal static Type typeofSoapEntity = typeof(SoapEntity);

		internal static Type typeofISoapXsd = typeof(ISoapXsd);
	}
}
