using System;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DelegateSessionManager : IDisposeTrackable, IDisposable
	{
		internal DelegateSessionManager(MailboxSession masterMailboxSession)
		{
			this.masterMailboxSession = masterMailboxSession;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<DelegateSessionManager>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal void SetTimeZone(ExTimeZone exTimeZone)
		{
			foreach (DelegateSessionEntry delegateSessionEntry in this.DelegateSessionCacheInstance)
			{
				delegateSessionEntry.MailboxSession.ExTimeZone = exTimeZone;
			}
		}

		internal DelegateSessionEntry GetDelegateSessionEntry(IExchangePrincipal principal, OpenBy openBy)
		{
			MailboxSession mailboxSession = null;
			DelegateSessionEntry delegateSessionEntry = null;
			bool flag = false;
			try
			{
				if (!this.DelegateSessionCacheInstance.TryGet(principal, openBy, out delegateSessionEntry))
				{
					mailboxSession = MailboxSession.InternalOpenDelegateAccess(this.masterMailboxSession, principal);
					delegateSessionEntry = this.DelegateSessionCacheInstance.Add(new DelegateSessionEntry(mailboxSession, openBy));
				}
				if (!delegateSessionEntry.IsConnected)
				{
					delegateSessionEntry.Connect();
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					if (delegateSessionEntry != null)
					{
						this.DelegateSessionCacheInstance.RemoveEntry(delegateSessionEntry);
					}
					else if (mailboxSession != null)
					{
						mailboxSession.CanDispose = true;
						mailboxSession.Dispose();
						mailboxSession = null;
					}
				}
			}
			return delegateSessionEntry;
		}

		internal void DisconnectAll()
		{
			if (this.delegateSessionCache != null)
			{
				foreach (DelegateSessionEntry delegateSessionEntry in this.delegateSessionCache)
				{
					if (!delegateSessionEntry.MailboxSession.IsDisposed && delegateSessionEntry.IsConnected)
					{
						delegateSessionEntry.Disconnect();
					}
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		private void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.delegateSessionCache != null)
				{
					this.delegateSessionCache.Clear();
				}
				this.delegateSessionCache = null;
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		private DelegateSessionCache DelegateSessionCacheInstance
		{
			get
			{
				if (this.delegateSessionCache == null)
				{
					this.delegateSessionCache = new DelegateSessionCache(6);
				}
				return this.delegateSessionCache;
			}
		}

		private bool isDisposed;

		private readonly MailboxSession masterMailboxSession;

		private DelegateSessionCache delegateSessionCache;

		private readonly DisposeTracker disposeTracker;
	}
}
