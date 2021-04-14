using System;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.Data
{
	internal static class UrlTokenConverter
	{
		internal static string UrlTokenEncode(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			return HttpServerUtility.UrlTokenEncode(bytes);
		}

		internal static bool TryUrlTokenDecode(string value, out string decodedValue)
		{
			decodedValue = null;
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			bool result;
			try
			{
				byte[] array = HttpServerUtility.UrlTokenDecode(value);
				if (array == null || array.Length == 0)
				{
					result = false;
				}
				else
				{
					decodedValue = Encoding.UTF8.GetString(array, 0, array.Length);
					result = true;
				}
			}
			catch (FormatException)
			{
				result = false;
			}
			return result;
		}
	}
}
