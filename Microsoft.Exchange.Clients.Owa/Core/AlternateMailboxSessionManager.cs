using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class AlternateMailboxSessionManager : DisposeTrackableBase
	{
		internal AlternateMailboxSessionManager(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			this.userContext = userContext;
			this.alternateMailboxSessions = new Dictionary<Guid, MailboxSession>(5);
			this.exchangePrincipals = new Dictionary<string, ExchangePrincipal>(StringComparer.OrdinalIgnoreCase);
		}

		internal ExchangePrincipal GetExchangePrincipal(string mailboxOwnerLegacyDN)
		{
			if (string.IsNullOrEmpty(mailboxOwnerLegacyDN))
			{
				throw new ArgumentNullException("mailboxOwnerLegacyDN");
			}
			if (!Utilities.IsValidLegacyDN(mailboxOwnerLegacyDN))
			{
				throw new ArgumentException("mailboxOwnerLegacyDN is not a valid LegacyDN");
			}
			ExchangePrincipal exchangePrincipal = null;
			ADSessionSettings adSettings = Utilities.CreateScopedADSessionSettings(this.userContext.LogonIdentity.DomainName);
			if (!this.exchangePrincipals.TryGetValue(mailboxOwnerLegacyDN, out exchangePrincipal))
			{
				exchangePrincipal = ExchangePrincipal.FromLegacyDN(adSettings, mailboxOwnerLegacyDN, RemotingOptions.AllowCrossSite | RemotingOptions.AllowCrossPremise);
				if (!exchangePrincipal.MailboxInfo.IsArchive && !exchangePrincipal.MailboxInfo.IsAggregated)
				{
					throw new ArgumentException(string.Format("mailboxOwnerLegacyDN: {0} is not for archive or aggregated mailbox", mailboxOwnerLegacyDN));
				}
				this.exchangePrincipals.Add(mailboxOwnerLegacyDN, exchangePrincipal);
			}
			return exchangePrincipal;
		}

		internal MailboxSession GetMailboxSession(IExchangePrincipal principal)
		{
			if (principal == null)
			{
				throw new ArgumentNullException("principal");
			}
			MailboxSession mailboxSession = null;
			if (principal.MailboxInfo.IsArchive || principal.MailboxInfo.IsAggregated)
			{
				this.alternateMailboxSessions.TryGetValue(principal.MailboxInfo.MailboxGuid, out mailboxSession);
				if (mailboxSession == null)
				{
					if (this.alternateMailboxSessions.Count == 5)
					{
						Guid key = Guid.Empty;
						using (Dictionary<Guid, MailboxSession>.Enumerator enumerator = this.alternateMailboxSessions.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								KeyValuePair<Guid, MailboxSession> keyValuePair = enumerator.Current;
								key = keyValuePair.Key;
							}
						}
						this.alternateMailboxSessions.Remove(key);
					}
					mailboxSession = this.CreateMailboxSession(principal);
					this.alternateMailboxSessions.Add(principal.MailboxInfo.MailboxGuid, mailboxSession);
				}
				Utilities.ReconnectStoreSession(mailboxSession, this.userContext);
				return mailboxSession;
			}
			throw new ArgumentException("principal is not for archive or alternate mailbox");
		}

		internal void UpdateTimeZoneOnAllSessions(ExTimeZone timeZone)
		{
			foreach (MailboxSession mailboxSession in this.alternateMailboxSessions.Values)
			{
				Utilities.ReconnectStoreSession(mailboxSession, this.userContext);
				mailboxSession.ExTimeZone = timeZone;
			}
		}

		internal void DisconnectAllSessions()
		{
			foreach (MailboxSession session in this.alternateMailboxSessions.Values)
			{
				Utilities.DisconnectStoreSession(session);
			}
		}

		internal void ClearAllSearchFolders()
		{
			foreach (MailboxSession mailboxSession in this.alternateMailboxSessions.Values)
			{
				Utilities.ReconnectStoreSession(mailboxSession, this.userContext);
				FolderSearch.ClearSearchFolders(mailboxSession);
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this.ReleaseAllSessions();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AlternateMailboxSessionManager>(this);
		}

		private MailboxSession CreateMailboxSession(IExchangePrincipal principal)
		{
			if (principal.MailboxInfo.IsArchive && !principal.IsCrossSiteAccessAllowed)
			{
				StackTrace stackTrace = new StackTrace();
				Exception exception = new OwaInvalidOperationException(string.Format("Archive Sessions should be allowed cross site, stack trace {0}", stackTrace.ToString()));
				ExWatson.SendReport(exception, ReportOptions.None, null);
			}
			MailboxSession mailboxSession = this.userContext.LogonIdentity.CreateMailboxSession(principal, Thread.CurrentThread.CurrentCulture, HttpContext.Current.Request);
			mailboxSession.ExTimeZone = this.userContext.MailboxSession.ExTimeZone;
			return mailboxSession;
		}

		private void ReleaseAllSessions()
		{
			foreach (MailboxSession mailboxSession in this.alternateMailboxSessions.Values)
			{
				Utilities.DisconnectStoreSession(mailboxSession);
				mailboxSession.Dispose();
			}
			this.alternateMailboxSessions.Clear();
		}

		private const int AlternateSessionCountMaximum = 5;

		private Dictionary<Guid, MailboxSession> alternateMailboxSessions;

		private Dictionary<string, ExchangePrincipal> exchangePrincipals;

		private UserContext userContext;
	}
}
