using System;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ConnectedAccountsNotificationManagerBase : DisposeTrackableBase, IConnectedAccountsNotificationManager, IDisposable
	{
		protected ConnectedAccountsNotificationManagerBase(Guid userMailboxGuid, Guid userMdbGuid, string userMailboxServerFQDN, IConnectedAccountsConfiguration configuration, ISyncNowNotificationClient notificationClient) : this(userMailboxGuid, userMdbGuid, userMailboxServerFQDN, configuration, notificationClient, new Func<TimerCallback, object, TimeSpan, TimeSpan, IGuardedTimer>(ConnectedAccountsNotificationManagerBase.CreateGuardedTimer))
		{
		}

		protected ConnectedAccountsNotificationManagerBase(Guid userMailboxGuid, Guid userMdbGuid, string userMailboxServerFQDN, IConnectedAccountsConfiguration configuration, ISyncNowNotificationClient notificationClient, Func<TimerCallback, object, TimeSpan, TimeSpan, IGuardedTimer> createGuardedTimer)
		{
			SyncUtilities.ThrowIfGuidEmpty("userMailboxGuid", userMailboxGuid);
			SyncUtilities.ThrowIfGuidEmpty("userMdbGuid", userMdbGuid);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("userMailboxServerFQDN", userMailboxServerFQDN);
			SyncUtilities.ThrowIfArgumentNull("configuration", configuration);
			SyncUtilities.ThrowIfArgumentNull("notificationClient", notificationClient);
			SyncUtilities.ThrowIfArgumentNull("createGuardedTimer", createGuardedTimer);
			this.configuration = configuration;
			this.notificationClient = notificationClient;
			this.userMailboxGuid = userMailboxGuid;
			this.userMdbGuid = userMdbGuid;
			this.userMailboxServerFQDN = userMailboxServerFQDN;
			if (this.configuration.PeriodicSyncNowEnabled)
			{
				ExTraceGlobals.ConnectedAccountsTracer.TraceDebug<Guid, TimeSpan>((long)this.GetHashCode(), "ConnectedAccountsNotificationManager::Setting up periodicSyncNowTimer for User:{0}, PeriodicSyncNowInterval:{1}", this.userMdbGuid, this.configuration.PeriodicSyncNowInterval);
				this.periodicSyncNowTimer = createGuardedTimer(new TimerCallback(this.SendPeriodicSyncNowRequest), null, this.configuration.PeriodicSyncNowInterval, this.configuration.PeriodicSyncNowInterval);
			}
		}

		protected Guid UserMailboxGuid
		{
			get
			{
				return this.userMailboxGuid;
			}
		}

		protected Guid UserMdbGuid
		{
			get
			{
				return this.userMdbGuid;
			}
		}

		protected string UserMailboxServerFQDN
		{
			get
			{
				return this.userMailboxServerFQDN;
			}
		}

		protected IGuardedTimer PeriodicSyncNowTimer
		{
			get
			{
				return this.periodicSyncNowTimer;
			}
		}

		protected ISyncNowNotificationClient NotificationClient
		{
			get
			{
				return this.notificationClient;
			}
		}

		protected IConnectedAccountsConfiguration Configuration
		{
			get
			{
				return this.configuration;
			}
		}

		void IConnectedAccountsNotificationManager.SendLogonTriggeredSyncNowRequest()
		{
			ExTraceGlobals.ConnectedAccountsTracer.TraceDebug((long)this.GetHashCode(), "SendLogonTriggeredSyncNowRequest::UserMailboxGuid:{0}, UserMdbGuid:{1}, userMailboxServerFQDN:{2}, LogonTriggeredSyncNowEnabled:{3}.", new object[]
			{
				this.userMailboxGuid,
				this.userMdbGuid,
				this.userMailboxServerFQDN,
				this.configuration.LogonTriggeredSyncNowEnabled
			});
			if (this.configuration.LogonTriggeredSyncNowEnabled)
			{
				this.SendSyncNowNotification(new Action<Guid, Guid, string>(this.notificationClient.NotifyOWALogonTriggeredSyncNowNeeded));
			}
		}

		void IConnectedAccountsNotificationManager.SendRefreshButtonTriggeredSyncNowRequest()
		{
			ExTraceGlobals.ConnectedAccountsTracer.TraceDebug((long)this.GetHashCode(), "SendRefreshButtonTriggeredSyncNowRequest::UserMailboxGuid:{0}, UserMdbGuid:{1}, userMailboxServerFQDN:{2}, RefreshButtonTriggeredSyncNowEnabled:{3}.", new object[]
			{
				this.userMailboxGuid,
				this.userMdbGuid,
				this.userMailboxServerFQDN,
				this.configuration.RefreshButtonTriggeredSyncNowEnabled
			});
			if (this.configuration.RefreshButtonTriggeredSyncNowEnabled)
			{
				ExDateTime currentTime = this.GetCurrentTime();
				TimeSpan timeSpan = currentTime - this.lastRefreshButtonTriggeredSyncNowRequest;
				if (timeSpan < this.configuration.RefreshButtonTriggeredSyncNowSuppressThreshold)
				{
					ExTraceGlobals.ConnectedAccountsTracer.TraceDebug((long)this.GetHashCode(), "SendRefreshButtonTriggeredSyncNowRequest:: Suppress request for this User (MailboxGuid:{0}, MdbGuid:{1}), timeSinceLastRequest:{2}, SuppressThreshold:{3}.", new object[]
					{
						this.userMailboxGuid,
						this.userMdbGuid,
						timeSpan,
						this.configuration.RefreshButtonTriggeredSyncNowSuppressThreshold
					});
					return;
				}
				this.SendSyncNowNotification(new Action<Guid, Guid, string>(this.notificationClient.NotifyOWARefreshButtonTriggeredSyncNowNeeded));
				this.lastRefreshButtonTriggeredSyncNowRequest = currentTime;
			}
		}

		public static bool ShouldSetupNotificationManagerForUser(MailboxSession mailboxSession, UserContext userContext)
		{
			if (mailboxSession != null && mailboxSession.MailboxOwner != null && !mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid.Equals(Guid.Empty) && !mailboxSession.MailboxOwner.MailboxInfo.MailboxDatabase.IsNullOrEmpty() && !string.IsNullOrEmpty(mailboxSession.MailboxOwner.MailboxInfo.Location.ServerFqdn))
			{
				return true;
			}
			ExTraceGlobals.ConnectedAccountsTracer.TraceDebug((long)userContext.GetHashCode(), "UserContext.InvokeConnectedAccountsSync::RequiredMailBoxSessionPropertiesNotSet, skip setting up the ConnectedAccountsNotificationManager.");
			return false;
		}

		private void SendSyncNowNotification(Action<Guid, Guid, string> notificationMethod)
		{
			this.ExecuteOperationOnThreadPoolThreadWithUnhandledExceptionHandler(delegate
			{
				notificationMethod(this.userMailboxGuid, this.userMdbGuid, this.userMailboxServerFQDN);
			});
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				ExTraceGlobals.ConnectedAccountsTracer.TraceDebug<Guid, Guid, string>((long)this.GetHashCode(), "ConnectedAccountsNotificationManager.InternalDispose called for User (MailboxGuid:{0},MdbGuid:{1},MailboxServerFQDN:{2}).", this.userMailboxGuid, this.userMdbGuid, this.userMailboxServerFQDN);
				if (this.periodicSyncNowTimer != null)
				{
					this.periodicSyncNowTimer.Dispose(false);
					this.periodicSyncNowTimer = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ConnectedAccountsNotificationManagerBase>(this);
		}

		protected virtual ExDateTime GetCurrentTime()
		{
			return ExDateTime.UtcNow;
		}

		protected virtual void ExecuteOperationOnThreadPoolThreadWithUnhandledExceptionHandler(Action userOperation)
		{
			ThreadPool.QueueUserWorkItem(delegate(object state)
			{
				try
				{
					userOperation();
				}
				catch (Exception ex)
				{
					ExTraceGlobals.ConnectedAccountsTracer.TraceError<Exception>((long)this.GetHashCode(), "Unhandled exception caught during SendRPCNotificationToMailboxServer call: {0}", ex);
					if (Globals.SendWatsonReports)
					{
						ExTraceGlobals.ConnectedAccountsTracer.TraceError((long)this.GetHashCode(), "Sending watson report.");
						ExWatson.SendReport(ex, ReportOptions.None, null);
					}
				}
			});
		}

		private static IGuardedTimer CreateGuardedTimer(TimerCallback timerCallback, object state, TimeSpan dueTime, TimeSpan period)
		{
			return new GuardedTimer(timerCallback, state, dueTime, period);
		}

		private void SendPeriodicSyncNowRequest(object state)
		{
			ExTraceGlobals.ConnectedAccountsTracer.TraceDebug<Guid, Guid, string>((long)this.GetHashCode(), "SendPeriodicSyncNowRequest::UserMailboxGuid:{0}, UserMdbGuid:{1}, userMailboxServerFQDN:{2}.", this.userMailboxGuid, this.userMdbGuid, this.userMailboxServerFQDN);
			this.SendSyncNowNotification(new Action<Guid, Guid, string>(this.notificationClient.NotifyOWAActivityTriggeredSyncNowNeeded));
		}

		private readonly IConnectedAccountsConfiguration configuration;

		private readonly ISyncNowNotificationClient notificationClient;

		private readonly Guid userMailboxGuid;

		private readonly Guid userMdbGuid;

		private readonly string userMailboxServerFQDN;

		private IGuardedTimer periodicSyncNowTimer;

		private ExDateTime lastRefreshButtonTriggeredSyncNowRequest = ExDateTime.MinValue;
	}
}
