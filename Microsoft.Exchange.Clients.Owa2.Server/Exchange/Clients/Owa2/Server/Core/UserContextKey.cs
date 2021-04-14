using System;
using System.Security.Principal;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class UserContextKey
	{
		private UserContextKey(string userContextId, string logonUniqueKey, string mailboxUniqueKey)
		{
			this.userContextId = userContextId;
			this.logonUniqueKey = logonUniqueKey;
			this.mailboxUniqueKey = mailboxUniqueKey;
		}

		internal string UserContextId
		{
			get
			{
				return this.userContextId;
			}
		}

		internal string LogonUniqueKey
		{
			get
			{
				return this.logonUniqueKey;
			}
			set
			{
				this.logonUniqueKey = value;
				this.keyString = null;
			}
		}

		internal string MailboxUniqueKey
		{
			get
			{
				return this.mailboxUniqueKey;
			}
			set
			{
				this.mailboxUniqueKey = value;
				this.keyString = null;
			}
		}

		public override string ToString()
		{
			if (this.keyString == null)
			{
				this.keyString = this.userContextId + ":" + this.logonUniqueKey;
				if (this.mailboxUniqueKey != null)
				{
					this.keyString = this.keyString + ":" + this.mailboxUniqueKey;
				}
			}
			return this.keyString;
		}

		internal static bool TryParse(string keyString, out UserContextKey userContextKey)
		{
			ArgumentValidator.ThrowIfNull("keyString", keyString);
			userContextKey = null;
			string[] array = keyString.Split(new char[]
			{
				':'
			});
			if (array.Length < 2 || array.Length > 3)
			{
				return false;
			}
			string text = array[0];
			string text2 = array[1];
			string text3 = null;
			if (array.Length == 3)
			{
				text3 = array[2];
			}
			userContextKey = new UserContextKey(text, text2, text3);
			return true;
		}

		internal static UserContextKey Parse(string keyString)
		{
			ArgumentValidator.ThrowIfNull("keyString", keyString);
			UserContextKey result;
			if (!UserContextKey.TryParse(keyString, out result))
			{
				throw new ArgumentException(string.Format("Invalid UserContextKey string - '{0}'", keyString), "keyString");
			}
			return result;
		}

		internal static UserContextKey Create(string userContextId, string logonUniqueKey, string mailboxUniqueKey)
		{
			return new UserContextKey(userContextId, logonUniqueKey, mailboxUniqueKey);
		}

		internal static UserContextKey CreateFromCookie(UserContextCookie userContextCookie, HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			if (httpContext.User == null)
			{
				throw new ArgumentNullException("httpContext.User");
			}
			if (httpContext.User.Identity == null)
			{
				throw new ArgumentNullException("httpContext.User.Identity");
			}
			SecurityIdentifier securityIdentifier = httpContext.User.Identity.GetSecurityIdentifier();
			if (securityIdentifier == null)
			{
				ExTraceGlobals.UserContextCallTracer.TraceDebug(0L, "UserContextKey.CreateFromCookie: current user has no security identifier.");
				return null;
			}
			string text = securityIdentifier.ToString();
			if (userContextCookie == null)
			{
				throw new ArgumentNullException("userContextCookie");
			}
			return new UserContextKey(userContextCookie.UserContextId, text, userContextCookie.MailboxUniqueKey);
		}

		internal static UserContextKey CreateFromCookie(UserContextCookie userContextCookie, SecurityIdentifier sid)
		{
			string text = sid.ToString();
			return new UserContextKey(userContextCookie.UserContextId, text, userContextCookie.MailboxUniqueKey);
		}

		internal static UserContextKey CreateNew(OwaIdentity logonIdentity, OwaIdentity mailboxIdentity, HttpContext httpContext)
		{
			if (logonIdentity == null)
			{
				throw new ArgumentNullException("logonIdentity");
			}
			string uniqueId = logonIdentity.UniqueId;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("userContextLogonIdentityName=<PII>{0}</PII>", logonIdentity.SafeGetRenderableName());
			if (logonIdentity.UserSid != null)
			{
				stringBuilder.AppendFormat("userContextLogonIdentitySid=<PII>{0}</PII>", logonIdentity.UserSid.ToString());
			}
			string text = null;
			if (mailboxIdentity != null)
			{
				text = mailboxIdentity.UniqueId;
				stringBuilder.AppendFormat("userContextMbIdentityName=<PII>{0}</PII>", mailboxIdentity.SafeGetRenderableName());
				if (mailboxIdentity.UserSid != null)
				{
					stringBuilder.AppendFormat("userContextMbIdentitySid=<PII>{0}</PII>", mailboxIdentity.UserSid.ToString());
				}
			}
			try
			{
				string text2 = stringBuilder.ToString();
				if (LiveIdAuthenticationModule.IdentityTracingEnabled && !string.IsNullOrWhiteSpace(text2))
				{
					httpContext.Response.AppendToLog(text2);
				}
			}
			catch (Exception)
			{
			}
			return UserContextKey.Create(UserContextUtilities.GetNewGuid(), uniqueId, text);
		}

		internal static UserContextKey CreateNew(SecurityIdentifier sid)
		{
			string text = null;
			string text2 = sid.ToString();
			return UserContextKey.Create(UserContextUtilities.GetNewGuid(), text2, text);
		}

		private string userContextId;

		private string logonUniqueKey;

		private string mailboxUniqueKey;

		private string keyString;
	}
}
