using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class SharedContext : MailboxContextBase
	{
		public override AuthZClientInfo CallerClientInfo
		{
			get
			{
				return CallContextUtilities.GetCallerClientInfo();
			}
		}

		internal SharedContext(UserContextKey key, string userAgent) : base(key, userAgent)
		{
		}

		public override void ValidateLogonPermissionIfNecessary()
		{
		}

		protected override MailboxSession CreateMailboxSession()
		{
			if (base.ExchangePrincipal == null)
			{
				throw new OwaInvalidOperationException("SharedContext::CreateMailboxSession cannot open mailbox session when ExchangePrincipal is null");
			}
			MailboxSession result;
			try
			{
				MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(base.ExchangePrincipal, CultureInfo.InvariantCulture, "Client=OWA");
				if (mailboxSession == null)
				{
					throw new OwaInvalidOperationException("SharedContext::CreateMailboxSession cannot create a mailbox session");
				}
				result = mailboxSession;
			}
			catch (AccessDeniedException innerException)
			{
				throw new OwaExplicitLogonException("user has no access rights to the mailbox", "errorexplicitlogonaccessdenied", innerException);
			}
			return result;
		}
	}
}
