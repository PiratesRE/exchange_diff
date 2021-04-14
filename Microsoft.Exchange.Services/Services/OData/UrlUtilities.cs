using System;

namespace Microsoft.Exchange.Services.OData
{
	internal static class UrlUtilities
	{
		public static string TrimKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return key;
			}
			return key.Trim(new char[]
			{
				'\'',
				'"',
				' '
			});
		}
	}
}
