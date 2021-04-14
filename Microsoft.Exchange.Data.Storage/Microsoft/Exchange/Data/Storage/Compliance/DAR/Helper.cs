using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Compliance.DAR
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Helper
	{
		public static string FromBytes(byte[] bytes)
		{
			if (bytes != null && bytes.Length > 0)
			{
				return Encoding.UTF8.GetString(bytes);
			}
			return null;
		}

		public static byte[] ToBytes(string s)
		{
			if (!string.IsNullOrEmpty(s))
			{
				return Encoding.UTF8.GetBytes(s);
			}
			return null;
		}

		public static string ToDefaultString(string input, string defaultValue = null)
		{
			if (string.IsNullOrEmpty(input))
			{
				return defaultValue;
			}
			return input;
		}
	}
}
