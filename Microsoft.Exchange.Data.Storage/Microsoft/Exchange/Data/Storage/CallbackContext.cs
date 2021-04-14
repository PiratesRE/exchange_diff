using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CallbackContext : DisposableObject
	{
		public CallbackContext(StoreSession session)
		{
			if (session != null && session is MailboxSession)
			{
				this.session = (MailboxSession)session;
			}
		}

		internal IDictionary<StoreObjectId, FolderAuditInfo> FolderAuditInfo
		{
			get
			{
				this.CheckDisposed(null);
				if (this.folderAuditInfo == null)
				{
					this.folderAuditInfo = new Dictionary<StoreObjectId, FolderAuditInfo>();
				}
				return this.folderAuditInfo;
			}
		}

		internal IDictionary<StoreObjectId, ItemAuditInfo> ItemAuditInfo
		{
			get
			{
				this.CheckDisposed(null);
				if (this.itemAuditInfo == null)
				{
					this.itemAuditInfo = new Dictionary<StoreObjectId, ItemAuditInfo>();
				}
				return this.itemAuditInfo;
			}
		}

		internal ItemAuditInfo ItemOperationAuditInfo { get; set; }

		internal bool? AuditSkippedOnBefore { get; set; }

		internal MailboxAuditOperations SubmitAuditOperation { get; set; }

		internal MailboxSession SubmitEffectiveMailboxSession { get; set; }

		internal ExchangePrincipal SubmitEffectiveMailboxOwner { get; set; }

		internal ContactLinkingProcessingState ContactLinkingProcessingState { get; set; }

		internal COWProcessorState SiteMailboxMessageDedupState { get; set; }

		internal COWProcessorState COWGroupMessageEscalationState { get; set; }

		internal COWProcessorState COWGroupMessageWSPublishingState { get; set; }

		public MailboxSession SessionWithBestAccess
		{
			get
			{
				this.CheckDisposed(null);
				if (this.sessionWithBestAccess != null)
				{
					return this.sessionWithBestAccess;
				}
				if (this.session != null && COWSession.IsDelegateSession(this.session))
				{
					this.sessionWithBestAccess = COWSettings.GetAdminMailboxSession(this.session);
					this.sessionWithBestAccess.SetClientIPEndpoints(this.session.ClientIPAddress, this.session.ServerIPAddress);
					COWSession.PerfCounters.DumpsterDelegateSessionsActive.Increment();
					return this.sessionWithBestAccess;
				}
				return this.session;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CallbackContext>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.SubmitEffectiveMailboxSession != null)
				{
					this.SubmitEffectiveMailboxSession.Dispose();
					this.SubmitEffectiveMailboxSession = null;
				}
				if (this.sessionWithBestAccess != null)
				{
					COWSettings.ReturnAdminMailboxSession(this.sessionWithBestAccess);
					this.sessionWithBestAccess = null;
					COWSession.PerfCounters.DumpsterDelegateSessionsActive.Decrement();
				}
			}
			base.InternalDispose(disposing);
		}

		private Dictionary<StoreObjectId, FolderAuditInfo> folderAuditInfo = new Dictionary<StoreObjectId, FolderAuditInfo>();

		private Dictionary<StoreObjectId, ItemAuditInfo> itemAuditInfo = new Dictionary<StoreObjectId, ItemAuditInfo>();

		private MailboxSession session;

		private MailboxSession sessionWithBestAccess;
	}
}
