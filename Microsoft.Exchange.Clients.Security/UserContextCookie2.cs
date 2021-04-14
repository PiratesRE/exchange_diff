using System;
using System.Net;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.Clients.Security
{
	public sealed class UserContextCookie2
	{
		private UserContextCookie2(string cookieId, string userContextId, string mailboxUniqueKey, bool isSecure)
		{
			this.cookieId = cookieId;
			this.userContextId = userContextId;
			this.mailboxUniqueKey = mailboxUniqueKey;
			this.isSecure = isSecure;
		}

		public HttpCookie HttpCookie
		{
			get
			{
				if (this.httpCookie == null)
				{
					this.httpCookie = new HttpCookie(this.CookieName, this.CookieValue);
					this.httpCookie.HttpOnly = true;
					this.httpCookie.Secure = this.isSecure;
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
				string text = UserContextCookie2.UserContextCookiePrefix;
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
						this.cookieValue = this.cookieValue + "&" + this.ValidTokenBase64Encode(bytes);
					}
				}
				return this.cookieValue;
			}
		}

		public override string ToString()
		{
			return this.CookieName + "=" + this.CookieValue;
		}

		internal static UserContextCookie2 Create(string cookieId, string userContextId, string mailboxUniqueKey, bool isSecure)
		{
			return new UserContextCookie2(cookieId, userContextId, mailboxUniqueKey, isSecure);
		}

		private string ValidTokenBase64Encode(byte[] byteArray)
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

		internal const int UserContextIdLength = 32;

		public static readonly string UserContextCookiePrefix = "UC";

		private readonly bool isSecure;

		private readonly string userContextId;

		private readonly string mailboxUniqueKey;

		private readonly string cookieId;

		private HttpCookie httpCookie;

		private Cookie netCookie;

		private string cookieValue;
	}
}
