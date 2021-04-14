using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace Microsoft.Exchange.Extensions
{
	public static class StringExtensions
	{
		public static bool IsNullOrBlank(this string value)
		{
			return string.IsNullOrEmpty(value) || value.Trim().Length == 0;
		}

		public static bool Contains(this string value, string search, StringComparison sc)
		{
			return value.IndexOf(search, sc) >= 0;
		}

		public static string FormatWith(this string template, params object[] args)
		{
			return string.Format(template, args);
		}

		public unsafe static SecureString ConvertToSecureString(this string password)
		{
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			IntPtr intPtr2;
			IntPtr intPtr = intPtr2 = password;
			if (intPtr != 0)
			{
				intPtr2 = (IntPtr)((int)intPtr + RuntimeHelpers.OffsetToStringData);
			}
			char* value = intPtr2;
			SecureString secureString = new SecureString(value, password.Length);
			secureString.MakeReadOnly();
			return secureString;
		}

		public static SecureString AsSecureString(this string password)
		{
			if (password == null)
			{
				return null;
			}
			return password.ConvertToSecureString();
		}
	}
}
