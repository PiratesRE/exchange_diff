using System;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PublicFolderMailboxSynchronizer : DisposeTrackableBase
	{
		static PublicFolderMailboxSynchronizer()
		{
			PublicFolderMailboxSynchronizer.SynchronizeHierarchyAfterSuccessInterval = TimeSpan.FromMilliseconds((double)StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\PublicFolder", "MailboxSynchronizerSuccessInterval", 900000, (int x) => x >= 60000));
			PublicFolderMailboxSynchronizer.SynchronizeHierarchyAfterFailureInterval = TimeSpan.FromMilliseconds((double)StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\PublicFolder", "MailboxSynchronizerFailureInterval", 60000, (int x) => x >= 60000));
		}

		public PublicFolderMailboxSynchronizer(OrganizationId organizationId, Guid mailboxGuid, string serverFqdn, bool onlyRefCounting)
		{
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			ArgumentValidator.ThrowIfEmpty("mailboxGuid", mailboxGuid);
			ArgumentValidator.ThrowIfNull("server", serverFqdn);
			this.tenantPartitionHint = TenantPartitionHint.FromOrganizationId(organizationId);
			this.mailboxGuid = mailboxGuid;
			this.serverFqdn = serverFqdn;
			if (!onlyRefCounting)
			{
				this.ScheduleSynchronizeHierarchy(TimeSpan.FromMilliseconds(0.0));
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				base.CheckDisposed();
				return this.mailboxGuid;
			}
		}

		public PublicFolderSyncJobState SyncJobState
		{
			get
			{
				base.CheckDisposed();
				return this.syncJobState;
			}
		}

		private void OnSynchronizeHierarchy(object state)
		{
			lock (this.objectDisposedLock)
			{
				if (this.isInternalDisposed)
				{
					return;
				}
				if (this.synchronizeHierarchyTimer != null)
				{
					this.synchronizeHierarchyTimer.Dispose();
					this.synchronizeHierarchyTimer = null;
				}
			}
			try
			{
				OrganizationId organizationId = this.tenantPartitionHint.GetOrganizationId();
				PublicFolderSyncJobState publicFolderSyncJobState = PublicFolderSyncJobRpc.QueryStatusSyncHierarchy(organizationId, this.mailboxGuid, this.serverFqdn);
				this.syncJobState = publicFolderSyncJobState;
				if (publicFolderSyncJobState.JobStatus == PublicFolderSyncJobState.Status.Queued)
				{
					this.ScheduleSynchonizeHierarchyCheck(PublicFolderMailboxSynchronizer.QueryStatusSynchronizeHierarchyInterval);
				}
				else
				{
					publicFolderSyncJobState = PublicFolderSyncJobRpc.StartSyncHierarchy(organizationId, this.mailboxGuid, this.serverFqdn, false);
					this.syncJobState = publicFolderSyncJobState;
					if (publicFolderSyncJobState.JobStatus == PublicFolderSyncJobState.Status.Queued)
					{
						this.ScheduleSynchonizeHierarchyCheck(PublicFolderMailboxSynchronizer.QueryStatusSynchronizeHierarchyInterval);
					}
					else
					{
						this.ScheduleSynchronizeHierarchy(PublicFolderMailboxSynchronizer.SynchronizeHierarchyAfterFailureInterval);
					}
				}
			}
			catch (PublicFolderSyncTransientException)
			{
				this.ScheduleSynchronizeHierarchy(PublicFolderMailboxSynchronizer.SynchronizeHierarchyAfterFailureInterval);
			}
			catch (PublicFolderSyncPermanentException)
			{
				this.ScheduleSynchronizeHierarchy(PublicFolderMailboxSynchronizer.SynchronizeHierarchyAfterFailureInterval);
			}
		}

		private void OnSynchronizeHierarchyCheck(object sender)
		{
			lock (this.objectDisposedLock)
			{
				if (this.isInternalDisposed)
				{
					return;
				}
				if (this.synchronizeHierarchyCheckTimer != null)
				{
					this.synchronizeHierarchyCheckTimer.Dispose();
					this.synchronizeHierarchyCheckTimer = null;
				}
			}
			try
			{
				OrganizationId organizationId = this.tenantPartitionHint.GetOrganizationId();
				PublicFolderSyncJobState publicFolderSyncJobState = PublicFolderSyncJobRpc.QueryStatusSyncHierarchy(organizationId, this.mailboxGuid, this.serverFqdn);
				this.syncJobState = publicFolderSyncJobState;
				if (publicFolderSyncJobState.JobStatus == PublicFolderSyncJobState.Status.Queued)
				{
					this.ScheduleSynchonizeHierarchyCheck(PublicFolderMailboxSynchronizer.QueryStatusSynchronizeHierarchyInterval);
				}
				else if (publicFolderSyncJobState.JobStatus == PublicFolderSyncJobState.Status.None)
				{
					this.ScheduleSynchronizeHierarchy(TimeSpan.FromMilliseconds(0.0));
				}
				else if (publicFolderSyncJobState.JobStatus == PublicFolderSyncJobState.Status.Completed)
				{
					if (publicFolderSyncJobState.LastError != null)
					{
						this.ScheduleSynchronizeHierarchy(PublicFolderMailboxSynchronizer.SynchronizeHierarchyAfterFailureInterval);
					}
					else
					{
						this.ScheduleSynchronizeHierarchy(PublicFolderMailboxSynchronizer.SynchronizeHierarchyAfterSuccessInterval);
					}
				}
			}
			catch (PublicFolderSyncTransientException)
			{
				this.ScheduleSynchonizeHierarchyCheck(PublicFolderMailboxSynchronizer.QueryStatusSynchronizeHierarchyInterval);
			}
			catch (PublicFolderSyncPermanentException)
			{
				this.ScheduleSynchonizeHierarchyCheck(PublicFolderMailboxSynchronizer.QueryStatusSynchronizeHierarchyInterval);
			}
		}

		private void ScheduleSynchronizeHierarchy(TimeSpan dueTime)
		{
			lock (this.objectDisposedLock)
			{
				if (!this.isInternalDisposed)
				{
					if (this.synchronizeHierarchyTimer == null)
					{
						this.synchronizeHierarchyTimer = new Timer(new TimerCallback(this.OnSynchronizeHierarchy), null, dueTime, TimeSpan.FromMilliseconds(-1.0));
					}
				}
			}
		}

		private void ScheduleSynchonizeHierarchyCheck(TimeSpan dueTime)
		{
			lock (this.objectDisposedLock)
			{
				if (!this.isInternalDisposed)
				{
					if (this.synchronizeHierarchyCheckTimer == null)
					{
						this.synchronizeHierarchyCheckTimer = new Timer(new TimerCallback(this.OnSynchronizeHierarchyCheck), null, dueTime, TimeSpan.FromMilliseconds(-1.0));
					}
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PublicFolderMailboxSynchronizer>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			lock (this.objectDisposedLock)
			{
				if (this.synchronizeHierarchyTimer != null)
				{
					this.synchronizeHierarchyTimer.Dispose();
					this.synchronizeHierarchyTimer = null;
				}
				if (this.synchronizeHierarchyCheckTimer != null)
				{
					this.synchronizeHierarchyCheckTimer.Dispose();
					this.synchronizeHierarchyCheckTimer = null;
				}
				this.isInternalDisposed = true;
			}
		}

		private const string RegKeyPublicFolder = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\PublicFolder";

		private const string RegValueMailboxSynchronizerSuccessInterval = "MailboxSynchronizerSuccessInterval";

		private const string RegValueMailboxSynchronizerFailureInterval = "MailboxSynchronizerFailureInterval";

		private const string RegValueMailboxSynchronizerQueryInterval = "MailboxSynchronizerQueryInterval";

		internal static TimeSpan SynchronizeHierarchyAfterSuccessInterval;

		internal static TimeSpan SynchronizeHierarchyAfterFailureInterval;

		internal static TimeSpan QueryStatusSynchronizeHierarchyInterval = TimeSpan.FromMilliseconds((double)StoreSession.GetConfigFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\PublicFolder", "MailboxSynchronizerQueryInterval", 60000, (int x) => x >= 60000));

		private readonly TenantPartitionHint tenantPartitionHint;

		private readonly Guid mailboxGuid;

		private readonly string serverFqdn;

		private Timer synchronizeHierarchyTimer;

		private Timer synchronizeHierarchyCheckTimer;

		protected PublicFolderSyncJobState syncJobState;

		private object objectDisposedLock = new object();

		private bool isInternalDisposed;
	}
}
