using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal sealed class MailboxSessionManager
	{
		private MailboxSessionManager()
		{
			string instanceName = null;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				instanceName = currentProcess.ProcessName;
			}
			this.mailboxSessionCache = new MailboxSessionCache(instanceName, MailboxSessionManager.MaxNumberOfMailboxSessions, MailboxSessionManager.MaxNumberOfMailboxSessionsPerMailbox, MailboxSessionManager.CacheTimeoutMinutes);
		}

		internal static MailboxSessionCache InnerMailboxSessionCache
		{
			get
			{
				return MailboxSessionManager.instance.mailboxSessionCache;
			}
		}

		internal static MailboxSession GetUserMailboxSessionFromCache(ExchangePrincipal principal)
		{
			MailboxSessionCacheKey cacheKey = new MailboxSessionCacheKey(principal);
			return MailboxSessionManager.instance.mailboxSessionCache.GetMailboxSession(cacheKey, principal);
		}

		internal static void ReturnMailboxSessionToCache(ref MailboxSession mailboxSession, bool discard)
		{
			if (!discard)
			{
				MailboxSessionManager.instance.mailboxSessionCache.ReturnMailboxSession(ref mailboxSession);
				return;
			}
			mailboxSession.Dispose();
			mailboxSession = null;
		}

		internal static MailboxSession CreateMailboxSession(ExchangePrincipal principal)
		{
			if (principal == null)
			{
				throw new ArgumentException("Principal is null");
			}
			return MailboxSession.OpenAsSystemService(principal, CultureInfo.InvariantCulture, "Client=Management;Action=AdminLog");
		}

		private const string ClientInfoString = "Client=Management;Action=AdminLog";

		private static readonly int MaxNumberOfMailboxSessions = AdminAuditSettings.Instance.SessionCacheSize;

		private static readonly int MaxNumberOfMailboxSessionsPerMailbox = AdminAuditSettings.Instance.MaxNumberOfMailboxSessionsPerMailbox;

		private static readonly TimeSpan CacheTimeoutMinutes = AdminAuditSettings.Instance.SessionExpirationTime;

		private static readonly MailboxSessionManager instance = new MailboxSessionManager();

		private readonly MailboxSessionCache mailboxSessionCache;
	}
}
