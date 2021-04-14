using System;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class UserContextCookie
	{
		internal Canary ContextCanary { get; private set; }

		private UserContextCookie(string cookieId, Canary canary, string mailboxUniqueKey)
		{
			this.cookieId = cookieId;
			this.mailboxUniqueKey = mailboxUniqueKey;
			this.ContextCanary = canary;
			this.cookieValue = null;
		}

		internal UserContextCookie(string cookieId, string userContextId, string logonUniqueKey, string mailboxUniqueKey)
		{
			Guid userContextId2;
			if (!Guid.TryParse(userContextId, out userContextId2))
			{
				userContextId2 = Guid.NewGuid();
			}
			this.cookieId = cookieId;
			this.mailboxUniqueKey = mailboxUniqueKey;
			this.ContextCanary = new Canary(userContextId2, logonUniqueKey);
			this.cookieValue = null;
		}

		internal static UserContextCookie CreateFromKey(string cookieId, UserContextKey userContextKey)
		{
			return new UserContextCookie(cookieId, userContextKey.Canary, userContextKey.MailboxUniqueKey);
		}

		internal static UserContextCookie Create(string cookieId, Canary canary, string mailboxUniqueKey)
		{
			if (canary == null)
			{
				ExTraceGlobals.UserContextTracer.TraceDebug<string, string>(20L, "Canary == null, cookieId={0}, mailboxUniqueKey={1}", cookieId, mailboxUniqueKey);
				return null;
			}
			return new UserContextCookie(cookieId, canary, mailboxUniqueKey);
		}

		internal static UserContextCookie TryCreateFromHttpCookie(HttpCookie cookie, string logonUniqueKey)
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			Canary canary = null;
			if (string.IsNullOrEmpty(cookie.Value))
			{
				ExTraceGlobals.UserContextTracer.TraceDebug<string, string, string>(21L, "Http cookie value is null, Name={0}, Domain={1}, Path={2}", cookie.Name, cookie.Domain, cookie.Path);
			}
			else if (!UserContextCookie.TryParseCookieValue(cookie.Value, out text, out text2))
			{
				ExTraceGlobals.UserContextTracer.TraceDebug(21L, "TryParseCookeValue failed, Name={0}, Domain={1}, Path={2}, Value={3}", new object[]
				{
					cookie.Name,
					cookie.Domain,
					cookie.Path,
					cookie.Value
				});
			}
			else
			{
				if (!UserContextCookie.TryParseCookieName(cookie.Name, out text3))
				{
					ExTraceGlobals.UserContextTracer.TraceDebug(21L, "TryParseCookieName failed, Name={0}, Domain={1}, Path={2}, vVlue={3}, canaryString={4}, mailboxUniqueKey={5}", new object[]
					{
						cookie.Name,
						cookie.Domain,
						cookie.Path,
						cookie.Value,
						text,
						text2
					});
				}
				canary = Canary.RestoreCanary(text, logonUniqueKey);
			}
			if (canary == null)
			{
				ExTraceGlobals.UserContextTracer.TraceDebug(21L, "restoredCanary==null, Name={0}, Domain={1}, Path={2}, Value={3}, canaryString={4}, mailboxUniqueKey={5}, logonUniqueKey={6}", new object[]
				{
					cookie.Name,
					cookie.Domain,
					cookie.Path,
					cookie.Value,
					text,
					text2,
					logonUniqueKey
				});
				canary = new Canary(Guid.NewGuid(), logonUniqueKey);
				ExTraceGlobals.UserContextTracer.TraceDebug<string, string, string>(21L, "Canary is recreated, userContextId={0}, logonUniqueKey={1}, canaryString={2}", canary.UserContextId, canary.LogonUniqueKey, canary.ToString());
			}
			return UserContextCookie.Create(text3, canary, text2);
		}

		internal HttpCookie HttpCookie
		{
			get
			{
				if (this.httpCookie == null)
				{
					this.httpCookie = new HttpCookie(this.CookieName, this.CookieValue);
					this.httpCookie.Secure = true;
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
					this.netCookie.Secure = true;
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

		internal string CookieValue
		{
			get
			{
				if (this.cookieValue == null)
				{
					this.cookieValue = this.ContextCanary.ToString();
					if (this.mailboxUniqueKey != null)
					{
						UTF8Encoding utf8Encoding = new UTF8Encoding();
						byte[] bytes = utf8Encoding.GetBytes(this.mailboxUniqueKey);
						this.cookieValue = this.cookieValue + "&" + Utilities.ValidTokenBase64Encode(bytes);
					}
				}
				return this.cookieValue;
			}
		}

		public override string ToString()
		{
			return this.CookieName + "=" + this.CookieValue;
		}

		internal string MailboxUniqueKey
		{
			get
			{
				return this.mailboxUniqueKey;
			}
		}

		internal static bool TryParseCookieValue(string cookieValue, out string canaryString, out string mailboxUniqueKey)
		{
			canaryString = null;
			mailboxUniqueKey = null;
			if (cookieValue.Length == 76)
			{
				canaryString = cookieValue;
			}
			else
			{
				if (cookieValue.Length < 78)
				{
					ExTraceGlobals.UserContextTracer.TraceDebug<string, int>(22L, "Invalid format cookie={0}, cookieValue.Length={1}", cookieValue, cookieValue.Length);
					return false;
				}
				int num = cookieValue.IndexOf('&');
				if (num != 76)
				{
					ExTraceGlobals.UserContextTracer.TraceDebug<int, int>(22L, "keyIndex={0}!= UserContextCookie.UserContextCanaryLength={1}", num, 76);
					return false;
				}
				num++;
				canaryString = cookieValue.Substring(0, num - 1);
				string text = cookieValue.Substring(num, cookieValue.Length - num);
				byte[] bytes = null;
				try
				{
					bytes = Utilities.ValidTokenBase64Decode(text);
				}
				catch (FormatException ex)
				{
					if (ExTraceGlobals.UserContextTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.UserContextTracer.TraceDebug<string, string>(22L, "FormatException:{0}, encodedMailboxUniqueKey={1}", ex.ToString(), text);
					}
					return false;
				}
				UTF8Encoding utf8Encoding = new UTF8Encoding();
				mailboxUniqueKey = utf8Encoding.GetString(bytes);
				return true;
			}
			return true;
		}

		internal static bool TryParseCookieName(string cookieName, out string cookieId)
		{
			cookieId = null;
			if (!cookieName.StartsWith("UserContext", StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.UserContextTracer.TraceDebug<string>(23L, "cookieName={0}", cookieName);
				return false;
			}
			int length = "UserContext".Length;
			if (cookieName.Length == length)
			{
				return true;
			}
			cookieId = cookieName.Substring(length + 1, cookieName.Length - length - 1);
			if (!Utilities.IsValidGuid(cookieId))
			{
				ExTraceGlobals.UserContextTracer.TraceDebug<string>(23L, "invalid cookieId={0}", cookieId);
				return false;
			}
			return true;
		}

		private static bool IsValidUserContextId(string userContextId)
		{
			return Utilities.IsValidGuid(userContextId);
		}

		internal static UserContextCookie GetUserContextCookie(OwaContext owaContext)
		{
			HttpRequest request = owaContext.HttpContext.Request;
			for (int i = 0; i < request.Cookies.Count; i++)
			{
				HttpCookie httpCookie = request.Cookies[i];
				if (httpCookie.Name != null && httpCookie.Name.StartsWith("UserContext", StringComparison.OrdinalIgnoreCase))
				{
					UserContextCookie userContextCookie = UserContextCookie.TryCreateFromHttpCookie(httpCookie, owaContext.LogonIdentity.UniqueId);
					if (userContextCookie == null)
					{
						ExTraceGlobals.UserContextTracer.TraceDebug(24L, "Invalid user context cookie received. Cookie value={0}, logonIdentity={1}, owaContext.MailboxIdentity.UniqueId={2}, owaContext.IsDifferentMailbox={3}", new object[]
						{
							httpCookie.Value,
							owaContext.LogonIdentity.UniqueId,
							owaContext.MailboxIdentity.UniqueId,
							owaContext.IsDifferentMailbox
						});
						throw new OwaInvalidRequestException("Invalid user context cookie received. Cookie value:" + httpCookie.Value + " logonIdentity:" + owaContext.LogonIdentity.UniqueId);
					}
					if (userContextCookie.MailboxUniqueKey == null)
					{
						if (!owaContext.IsDifferentMailbox)
						{
							return userContextCookie;
						}
					}
					else if (string.Equals(userContextCookie.MailboxUniqueKey, owaContext.MailboxIdentity.UniqueId, StringComparison.Ordinal))
					{
						return userContextCookie;
					}
					ExTraceGlobals.UserContextTracer.TraceDebug<string, string, bool>(24L, "currentCookie.MailboxUniqueKey={0}, owaContext.MailboxIdentity.UniqueId={1}, owaContext.IsDifferentMailbox={2}", userContextCookie.MailboxUniqueKey, owaContext.MailboxIdentity.UniqueId, owaContext.IsDifferentMailbox);
				}
			}
			return null;
		}

		internal UserContextCookie CloneWithNewCanary()
		{
			return new UserContextCookie(this.cookieId, this.ContextCanary.UserContextId, this.ContextCanary.LogonUniqueKey, this.mailboxUniqueKey);
		}

		internal const int UserContextCanaryLength = 76;

		public const string UserContextCookiePrefix = "UserContext";

		private string mailboxUniqueKey;

		private string cookieId;

		private HttpCookie httpCookie;

		private Cookie netCookie;

		private string cookieValue;
	}
}
