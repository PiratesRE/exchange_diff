using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class UserContextKey
	{
		public Canary Canary { get; private set; }

		private UserContextKey(Guid userContextId, string logonUniqueKey, string mailboxUniqueKey)
		{
			this.Canary = new Canary(userContextId, logonUniqueKey);
			this.mailboxUniqueKey = mailboxUniqueKey;
		}

		private UserContextKey(Canary canary, string mailboxUniqueKey)
		{
			this.Canary = canary;
			this.mailboxUniqueKey = mailboxUniqueKey;
		}

		internal UserContextKey CloneWithRenewedCanary()
		{
			return new UserContextKey(this.Canary.CloneRenewed(), this.mailboxUniqueKey);
		}

		internal static UserContextKey Create(Guid userContextIdGuid, string logonUniqueKey, string mailboxUniqueKey)
		{
			return new UserContextKey(userContextIdGuid, logonUniqueKey, mailboxUniqueKey);
		}

		internal static UserContextKey CreateFromCookie(UserContextCookie userContextCookie)
		{
			return new UserContextKey(userContextCookie.ContextCanary, userContextCookie.MailboxUniqueKey);
		}

		internal static UserContextKey CreateNew(OwaContext owaContext)
		{
			string uniqueId = owaContext.LogonIdentity.UniqueId;
			string text = owaContext.IsDifferentMailbox ? owaContext.MailboxIdentity.UniqueId : null;
			return UserContextKey.Create(Guid.NewGuid(), uniqueId, text);
		}

		public override string ToString()
		{
			if (this.keyString == null)
			{
				this.keyString = this.UserContextId + ":" + this.Canary.LogonUniqueKey;
				if (this.mailboxUniqueKey != null)
				{
					this.keyString = this.keyString + ":" + this.mailboxUniqueKey;
				}
			}
			return this.keyString;
		}

		internal string UserContextId
		{
			get
			{
				return this.Canary.UserContextId;
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
			}
		}

		private string mailboxUniqueKey;

		private string keyString;
	}
}
