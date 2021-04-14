using System;
using System.Security.Principal;
using System.Web.Caching;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMailboxContext : IDisposable
	{
		UserContextState State { get; set; }

		UserContextKey Key { get; }

		ExchangePrincipal ExchangePrincipal { get; set; }

		MailboxSession MailboxSession { get; }

		string UserPrincipalName { get; }

		SmtpAddress PrimarySmtpAddress { get; }

		OwaIdentity LogonIdentity { get; }

		OwaIdentity MailboxIdentity { get; }

		INotificationManager NotificationManager { get; }

		PendingRequestManager PendingRequestManager { get; }

		CacheItemRemovedReason AbandonedReason { get; set; }

		UserContextTerminationStatus TerminationStatus { get; set; }

		bool IsExplicitLogon { get; }

		SessionDataCache SessionDataCache { get; }

		void Load(OwaIdentity logonIdentity, OwaIdentity mailboxIdentity, UserContextStatistics stats);

		void ValidateLogonPermissionIfNecessary();

		void LogBreadcrumb(string message);

		string DumpBreadcrumbs();

		bool LockAndReconnectMailboxSession(int timeout);

		void UnlockAndDisconnectMailboxSession();

		bool MailboxSessionLockedByCurrentThread();

		void DisconnectMailboxSession();

		MailboxSession CloneMailboxSession(string mailboxKey, ExchangePrincipal exchangePrincipal, IADOrgPerson person, ClientSecurityContext clientSecurityContext, GenericIdentity genericIdentity, bool unifiedLogon);
	}
}
