using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Worker.Throttling;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncMailboxSession : DisposeTrackableBase
	{
		public SyncMailboxSession(SyncLogSession syncLogSession)
		{
			this.storeWasRestartedAtLeastOnce = false;
			this.isMailboxSessionSet = false;
			this.syncLogSession = syncLogSession;
		}

		internal SyncMailboxSession(MailboxSession mailboxSession, SyncLogSession syncLogSession) : this(syncLogSession)
		{
			SyncUtilities.ThrowIfArgumentNull("mailboxSession", mailboxSession);
			this.mailboxSession = mailboxSession;
			this.isMailboxSessionSet = true;
		}

		public MailboxSession MailboxSession
		{
			get
			{
				base.CheckDisposed();
				return this.mailboxSession;
			}
		}

		public bool WasMailboxSessionOpened
		{
			get
			{
				base.CheckDisposed();
				return this.isMailboxSessionSet;
			}
		}

		public bool TryOpen(string legacyDN, Guid mailboxGuid, Guid databaseGuid, string mailboxServer, out OrganizationId organizationId, out ISyncException exception, out bool invalidState)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("legacyDN", legacyDN);
			SyncUtilities.ThrowIfGuidEmpty("mailboxGuid", mailboxGuid);
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("mailboxServer", mailboxServer);
			if (this.MailboxSession != null)
			{
				organizationId = this.organizationId;
				exception = null;
				invalidState = false;
				return true;
			}
			Exception ex = null;
			try
			{
				exception = null;
				invalidState = false;
				MailboxSession mailboxSession = SyncUtilities.OpenMailboxSessionAndHaveCompleteExchangePrincipal(mailboxGuid, databaseGuid, (IExchangePrincipal exchangePrincipal) => SubscriptionManager.OpenMailbox(exchangePrincipal, ExchangeMailboxOpenType.AsTransport, SyncUtilities.WorkerClientInfoString));
				SyncStoreLoadManager.Instance.EnableLoadTrackingOnSession(mailboxSession);
				this.organizationId = mailboxSession.MailboxOwner.MailboxInfo.OrganizationId;
				this.SetMailboxSession(mailboxSession);
				organizationId = this.organizationId;
				return true;
			}
			catch (ObjectNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (MailboxUnavailableException ex3)
			{
				ex = ex3;
			}
			this.syncLogSession.LogError((TSLID)306UL, "Did not find mailbox when trying to open it. Error:{0}", new object[]
			{
				ex
			});
			exception = SyncPermanentException.CreateOperationLevelException(DetailedAggregationStatus.CommunicationError, new SubscriptionSyncException(null, ex));
			organizationId = null;
			invalidState = true;
			return false;
		}

		public void SetMailboxSession(MailboxSession toWrap)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("toWrap", toWrap);
			this.mailboxSession = toWrap;
			this.isMailboxSessionSet = true;
		}

		public void SetOrganizationId(OrganizationId organizationId)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("organizationId", organizationId);
			this.organizationId = organizationId;
		}

		public bool EnsureConnectWithStatus()
		{
			base.CheckDisposed();
			this.EnsureMailboxSessionSet();
			if (this.mailboxSession.IsConnected)
			{
				this.syncLogSession.LogDebugging((TSLID)6UL, "EnsureConnectWithStatus::Mailbox session is already connected, storeWasRestartedAtLeastOnce:{0}", new object[]
				{
					this.storeWasRestartedAtLeastOnce
				});
			}
			else
			{
				bool flag = this.mailboxSession.ConnectWithStatus();
				if (flag)
				{
					this.storeWasRestartedAtLeastOnce = true;
				}
				this.syncLogSession.LogDebugging((TSLID)7UL, "EnsureConnectWithStatus::Just reconnected mailbox session, storeWasRestartedInLatestReconnect:{0}, storeWasRestartedAtLeastOnce: {1}", new object[]
				{
					flag,
					this.storeWasRestartedAtLeastOnce
				});
			}
			return this.storeWasRestartedAtLeastOnce;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.mailboxSession != null)
			{
				this.mailboxSession.Dispose();
				this.mailboxSession = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SyncMailboxSession>(this);
		}

		private void EnsureMailboxSessionSet()
		{
		}

		private readonly SyncLogSession syncLogSession;

		private MailboxSession mailboxSession;

		private OrganizationId organizationId;

		private bool storeWasRestartedAtLeastOnce;

		private bool isMailboxSessionSet;
	}
}
