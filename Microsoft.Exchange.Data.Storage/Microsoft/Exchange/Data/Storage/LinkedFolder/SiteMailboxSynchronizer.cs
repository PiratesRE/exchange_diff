using System;
using System.Threading;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SiteMailboxSynchronizer : DisposeTrackableBase
	{
		public SiteMailboxSynchronizer(IExchangePrincipal siteMailboxPrincipal, string client)
		{
			if (!SiteMailboxSynchronizer.initialized)
			{
				lock (SiteMailboxSynchronizer.initSyncObject)
				{
					if (!SiteMailboxSynchronizer.initialized)
					{
						SiteMailboxSynchronizer.SyncInterval = TimeSpan.FromSeconds((double)StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\SiteMailbox", "SyncInterval", 900, (int x) => x > 0));
						SiteMailboxSynchronizer.initialized = true;
					}
				}
			}
			this.clientString = client + "_SiteMailboxSychronizer";
			this.siteMailboxPrincipal = siteMailboxPrincipal;
			this.ScheduleSynchronization(TimeSpan.FromMilliseconds(0.0));
		}

		public bool TryToSyncNow()
		{
			return this.TryThreadSafeCall(delegate
			{
				this.ScheduleSynchronization(TimeSpan.FromMilliseconds(0.0));
				return true;
			});
		}

		public bool TryToDispose()
		{
			return this.TryThreadSafeCall(delegate
			{
				if (this.hasBeenSyncedOnce)
				{
					this.Dispose();
					return true;
				}
				return false;
			});
		}

		public Guid MailboxGuid
		{
			get
			{
				base.CheckDisposed();
				return this.siteMailboxPrincipal.MailboxInfo.MailboxGuid;
			}
		}

		private void OnSynchronize(object state)
		{
			this.ThreadSafeCall(delegate
			{
				if (base.IsDisposed)
				{
					return;
				}
				this.hasBeenSyncedOnce = true;
				if (this.synchronizerTimer != null)
				{
					this.synchronizerTimer.Dispose();
					this.synchronizerTimer = null;
				}
				try
				{
					if (this.lastMembershipSyncTime + SiteMailboxSynchronizer.SyncInterval < DateTime.UtcNow)
					{
						this.lastMembershipSyncTime = DateTime.UtcNow;
						Utils.TriggerSiteMailboxSync(this.siteMailboxPrincipal, this.clientString, false);
					}
					else
					{
						Utils.TriggerSiteMailboxSync(this.siteMailboxPrincipal, this.clientString, true);
					}
				}
				finally
				{
					this.ScheduleSynchronization(SiteMailboxSynchronizer.SyncInterval);
				}
			});
		}

		private void ScheduleSynchronization(TimeSpan dueTime)
		{
			this.ThreadSafeCall(delegate
			{
				if (this.IsDisposed)
				{
					return;
				}
				if (this.synchronizerTimer == null)
				{
					this.synchronizerTimer = new Timer(new TimerCallback(this.OnSynchronize), null, dueTime, TimeSpan.FromMilliseconds(-1.0));
					return;
				}
				this.synchronizerTimer.Change(dueTime, TimeSpan.FromMilliseconds(-1.0));
			});
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SiteMailboxSynchronizer>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			this.ThreadSafeCall(delegate
			{
				if (this.synchronizerTimer != null)
				{
					this.synchronizerTimer.Dispose();
					this.synchronizerTimer = null;
				}
			});
		}

		private void ThreadSafeCall(Action functionDelegate)
		{
			try
			{
				Monitor.Enter(this.threadSafeLock);
				functionDelegate();
			}
			finally
			{
				if (Monitor.IsEntered(this.threadSafeLock))
				{
					Monitor.Exit(this.threadSafeLock);
				}
			}
		}

		private bool TryThreadSafeCall(Func<bool> functionDelegate)
		{
			try
			{
				if (Monitor.TryEnter(this.threadSafeLock))
				{
					return functionDelegate();
				}
			}
			finally
			{
				if (Monitor.IsEntered(this.threadSafeLock))
				{
					Monitor.Exit(this.threadSafeLock);
				}
			}
			return false;
		}

		private const string RegKeySiteMailbox = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\SiteMailbox";

		private const string RegValueSyncInterval = "SyncInterval";

		private static bool initialized = false;

		private static readonly object initSyncObject = new object();

		private readonly string clientString;

		private static TimeSpan SyncInterval = TimeSpan.MaxValue;

		private readonly IExchangePrincipal siteMailboxPrincipal;

		private Timer synchronizerTimer;

		private readonly object threadSafeLock = new object();

		private DateTime lastMembershipSyncTime = DateTime.MinValue;

		private bool hasBeenSyncedOnce;
	}
}
