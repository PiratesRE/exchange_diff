using System;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class UserContextCookie
	{
		private UserContextCookie(string cookieId, string userContextId, string mailboxUniqueKey, bool isSecure)
		{
			this.cookieId = cookieId;
			this.userContextId = userContextId;
			this.mailboxUniqueKey = mailboxUniqueKey;
			this.isSecure = isSecure;
		}

		internal HttpCookie HttpCookie
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
				string text = UserContextCookie.UserContextCookiePrefix;
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
						this.cookieValue = this.cookieValue + "&" + UserContextUtilities.ValidTokenBase64Encode(bytes);
					}
				}
				return this.cookieValue;
			}
		}

		public override string ToString()
		{
			return this.CookieName + "=" + this.CookieValue;
		}

		internal static UserContextCookie Create(string cookieId, string userContextId, string mailboxUniqueKey, bool isSecure)
		{
			return new UserContextCookie(cookieId, userContextId, mailboxUniqueKey, isSecure);
		}

		internal static UserContextCookie CreateFromKey(string cookieId, UserContextKey userContextKey, bool isSecure)
		{
			return UserContextCookie.Create(cookieId, userContextKey.UserContextId, userContextKey.MailboxUniqueKey, isSecure);
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
			return UserContextCookie.Create(text3, text, text2, cookie.Secure);
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
					bytes = UserContextUtilities.ValidTokenBase64Decode(tokenValidBase64String);
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
			if (!cookieName.StartsWith(UserContextCookie.UserContextCookiePrefix, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			int length = UserContextCookie.UserContextCookiePrefix.Length;
			if (cookieName.Length == length)
			{
				return true;
			}
			cookieId = cookieName.Substring(length + 1, cookieName.Length - length - 1);
			return UserContextUtilities.IsValidGuid(cookieId);
		}

		internal static UserContextCookie GetUserContextCookie(HttpContext httpContext)
		{
			HttpRequest request = httpContext.Request;
			for (int i = 0; i < request.Cookies.Count; i++)
			{
				HttpCookie httpCookie = request.Cookies[i];
				if (httpCookie.Name != null && httpCookie.Name.StartsWith(UserContextCookie.UserContextCookiePrefix, StringComparison.OrdinalIgnoreCase))
				{
					UserContextCookie userContextCookie = UserContextCookie.TryCreateFromHttpCookie(httpCookie);
					if (userContextCookie == null)
					{
						ExTraceGlobals.UserContextTracer.TraceDebug<string, string, string>(0L, "Invalid user context cookie received. Name={0}, Value={1}, httpContext.Request.RawUrl={2}", httpCookie.Name, httpCookie.Value, request.RawUrl);
						return null;
					}
					if (userContextCookie.MailboxUniqueKey == null)
					{
						if (!UserContextUtilities.IsDifferentMailbox(httpContext))
						{
							return userContextCookie;
						}
					}
					else
					{
						string explicitLogonUser = UserContextUtilities.GetExplicitLogonUser(httpContext);
						if (!string.IsNullOrEmpty(explicitLogonUser))
						{
							using (OwaIdentity owaIdentity = OwaIdentity.CreateOwaIdentityFromExplicitLogonAddress(explicitLogonUser))
							{
								if (string.Equals(userContextCookie.MailboxUniqueKey, owaIdentity.UniqueId, StringComparison.Ordinal))
								{
									return userContextCookie;
								}
							}
						}
					}
				}
			}
			return null;
		}

		private static bool IsValidUserContextId(string userContextId)
		{
			return UserContextUtilities.IsValidGuid(userContextId);
		}

		internal const int UserContextIdLength = 32;

		public static readonly string UserContextCookiePrefix = "UC";

		private readonly bool isSecure;

		private string userContextId;

		private string mailboxUniqueKey;

		private string cookieId;

		private HttpCookie httpCookie;

		private Cookie netCookie;

		private string cookieValue;
	}
}
