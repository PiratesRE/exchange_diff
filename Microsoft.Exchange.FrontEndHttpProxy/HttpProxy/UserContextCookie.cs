using System;
using System.Net;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.HttpProxy
{
	public sealed class UserContextCookie
	{
		private UserContextCookie(string cookieId, string userContextId, string mailboxUniqueKey)
		{
			this.cookieId = cookieId;
			this.userContextId = userContextId;
			this.mailboxUniqueKey = mailboxUniqueKey;
		}

		internal HttpCookie HttpCookie
		{
			get
			{
				if (this.httpCookie == null)
				{
					this.httpCookie = new HttpCookie(this.CookieName, this.CookieValue);
				}
				return this.httpCookie;
			}
		}

		internal Cookie NetCookie
		{
			get
			{
				if (this.netCookie == null)
				{
					this.netCookie = new Cookie(this.CookieName, this.CookieValue);
				}
				return this.netCookie;
			}
		}

		internal string CookieName
		{
			get
			{
				string text = "UserContext";
				if (this.cookieId != null)
				{
					text = text + "_" + this.cookieId;
				}
				return text;
			}
		}

		internal string UserContextId
		{
			get
			{
				return this.userContextId;
			}
		}

		internal string MailboxUniqueKey
		{
			get
			{
				return this.mailboxUniqueKey;
			}
		}

		internal string CookieValue
		{
			get
			{
				if (this.cookieValue == null)
				{
					this.cookieValue = this.userContextId;
					if (this.mailboxUniqueKey != null)
					{
						UTF8Encoding utf8Encoding = new UTF8Encoding();
						byte[] bytes = utf8Encoding.GetBytes(this.mailboxUniqueKey);
						this.cookieValue = this.cookieValue + "&" + UserContextCookie.ValidTokenBase64Encode(bytes);
					}
				}
				return this.cookieValue;
			}
		}

		public static string ValidTokenBase64Encode(byte[] byteArray)
		{
			if (byteArray == null)
			{
				throw new ArgumentNullException("byteArray");
			}
			int num = (int)(1.3333333333333333 * (double)byteArray.Length);
			if (num % 4 != 0)
			{
				num += 4 - num % 4;
			}
			char[] array = new char[num];
			Convert.ToBase64CharArray(byteArray, 0, byteArray.Length, array, 0);
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == '\\')
				{
					array[i] = '-';
				}
				else if (array[i] == '=')
				{
					num2++;
				}
			}
			return new string(array, 0, array.Length - num2);
		}

		public static byte[] ValidTokenBase64Decode(string tokenValidBase64String)
		{
			if (tokenValidBase64String == null)
			{
				throw new ArgumentNullException("tokenValidBase64String");
			}
			long num = (long)tokenValidBase64String.Length;
			if (tokenValidBase64String.Length % 4 != 0)
			{
				num += (long)(4 - tokenValidBase64String.Length % 4);
			}
			char[] array = new char[num];
			tokenValidBase64String.CopyTo(0, array, 0, tokenValidBase64String.Length);
			for (long num2 = 0L; num2 < (long)tokenValidBase64String.Length; num2 += 1L)
			{
				checked
				{
					if (array[(int)((IntPtr)num2)] == '-')
					{
						array[(int)((IntPtr)num2)] = '\\';
					}
				}
			}
			for (long num3 = (long)tokenValidBase64String.Length; num3 < (long)array.Length; num3 += 1L)
			{
				array[(int)(checked((IntPtr)num3))] = '=';
			}
			return Convert.FromBase64CharArray(array, 0, array.Length);
		}

		public static bool IsValidGuid(string guid)
		{
			if (guid == null || guid.Length != 32)
			{
				return false;
			}
			for (int i = 0; i < 32; i++)
			{
				if (!UserContextCookie.IsHexChar(guid[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsHexChar(char c)
		{
			return char.IsDigit(c) || (char.ToUpperInvariant(c) >= 'A' && char.ToUpperInvariant(c) <= 'F');
		}

		public override string ToString()
		{
			return this.CookieName + "=" + this.CookieValue;
		}

		internal static UserContextCookie Create(string cookieId, string userContextId, string mailboxUniqueKey)
		{
			return new UserContextCookie(cookieId, userContextId, mailboxUniqueKey);
		}

		internal static UserContextCookie TryCreateFromHttpCookie(HttpCookie cookie)
		{
			string text = null;
			string text2 = null;
			if (string.IsNullOrEmpty(cookie.Value))
			{
				return null;
			}
			if (!UserContextCookie.TryParseCookieValue(cookie.Value, out text, out text2))
			{
				return null;
			}
			string text3 = null;
			if (!UserContextCookie.TryParseCookieName(cookie.Name, out text3))
			{
				return null;
			}
			return UserContextCookie.Create(text3, text, text2);
		}

		internal static UserContextCookie TryCreateFromNetCookie(Cookie cookie)
		{
			string text = null;
			string text2 = null;
			if (string.IsNullOrEmpty(cookie.Value))
			{
				return null;
			}
			if (!UserContextCookie.TryParseCookieValue(cookie.Value, out text, out text2))
			{
				return null;
			}
			string text3 = null;
			if (!UserContextCookie.TryParseCookieName(cookie.Name, out text3))
			{
				return null;
			}
			return UserContextCookie.Create(text3, text, text2);
		}

		internal static bool TryParseCookieValue(string cookieValue, out string userContextId, out string mailboxUniqueKey)
		{
			userContextId = null;
			mailboxUniqueKey = null;
			if (cookieValue.Length == 32)
			{
				userContextId = cookieValue;
			}
			else
			{
				if (cookieValue.Length < 34)
				{
					return false;
				}
				int num = cookieValue.IndexOf('&');
				if (num != 32)
				{
					return false;
				}
				num++;
				userContextId = cookieValue.Substring(0, num - 1);
				string tokenValidBase64String = cookieValue.Substring(num, cookieValue.Length - num);
				byte[] bytes = null;
				try
				{
					bytes = UserContextCookie.ValidTokenBase64Decode(tokenValidBase64String);
				}
				catch (FormatException)
				{
					return false;
				}
				UTF8Encoding utf8Encoding = new UTF8Encoding();
				mailboxUniqueKey = utf8Encoding.GetString(bytes);
			}
			return UserContextCookie.IsValidUserContextId(userContextId);
		}

		internal static bool TryParseCookieName(string cookieName, out string cookieId)
		{
			cookieId = null;
			if (!cookieName.StartsWith("UserContext", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			int length = "UserContext".Length;
			if (cookieName.Length == length)
			{
				return true;
			}
			cookieId = cookieName.Substring(length + 1, cookieName.Length - length - 1);
			return UserContextCookie.IsValidGuid(cookieId);
		}

		private static bool IsValidUserContextId(string userContextId)
		{
			return UserContextCookie.IsValidGuid(userContextId);
		}

		public const string UserContextCookiePrefix = "UserContext";

		internal const int UserContextIdLength = 32;

		private readonly string userContextId;

		private readonly string mailboxUniqueKey;

		private readonly string cookieId;

		private HttpCookie httpCookie;

		private Cookie netCookie;

		private string cookieValue;
	}
}
