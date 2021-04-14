using System;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal abstract class ADConfigurationLoader<ADConfigType, StateType> : DisposeTrackableBase where ADConfigType : ADConfigurationObject, new()
	{
		protected abstract void LogFailure(ADConfigurationLoader<ADConfigType, StateType>.FailureLocation failureLocation, Exception exception);

		protected abstract void PreAdOperation(ref StateType state);

		protected abstract void AdOperation(ref StateType state);

		protected abstract void PostAdOperation(StateType state, bool wasSuccessful);

		protected abstract void OnServerChangeCallback(ADNotificationEventArgs args);

		protected ADConfigurationLoader() : this(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(15.0))
		{
		}

		protected ADConfigurationLoader(TimeSpan failureRetryInterval, TimeSpan periodicReadInterval)
		{
			this.failureRetryInterval = failureRetryInterval;
			this.periodicReadInterval = periodicReadInterval;
			this.periodicTimer = new GuardedTimer(new TimerCallback(this.ReadConfiguration));
		}

		internal void ReadConfiguration()
		{
			StateType state = default(StateType);
			this.PreAdOperation(ref state);
			base.CheckDisposed();
			this.TryRegisterForADNotifications();
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				this.AdOperation(ref state);
			}, 2);
			if (adoperationResult.Succeeded)
			{
				if (this.adNotificationCookie != null)
				{
					this.UpdateTimer(this.periodicReadInterval);
				}
			}
			else
			{
				this.LogFailure(ADConfigurationLoader<ADConfigType, StateType>.FailureLocation.ADConfigurationLoading, adoperationResult.Exception);
				if (this.adNotificationCookie != null)
				{
					this.UpdateTimer(this.failureRetryInterval);
				}
			}
			this.PostAdOperation(state, adoperationResult.Succeeded);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ADConfigurationLoader<ADConfigType, StateType>>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.periodicTimer != null)
				{
					this.periodicTimer.Dispose(true);
					lock (this.periodicTimerLock)
					{
						this.periodicTimer = null;
					}
				}
				lock (this.notificationLock)
				{
					if (this.adNotificationCookie != null)
					{
						ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
						{
							ADNotificationAdapter.UnregisterChangeNotification(this.adNotificationCookie);
						}, 2);
						if (!adoperationResult.Succeeded)
						{
							this.LogFailure(ADConfigurationLoader<ADConfigType, StateType>.FailureLocation.ADNotificationRegistration, adoperationResult.Exception);
						}
						this.adNotificationCookie = null;
					}
					this.hasUnregisteredNotification = true;
				}
			}
		}

		private void ReadConfiguration(object obj)
		{
			this.ReadConfiguration();
		}

		private void TryRegisterForADNotifications()
		{
			if (this.adNotificationCookie == null)
			{
				lock (this.notificationLock)
				{
					if (this.adNotificationCookie == null && !this.hasUnregisteredNotification)
					{
						ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
						{
							this.adNotificationCookie = ADNotificationAdapter.RegisterChangeNotification<ADConfigType>(null, new ADNotificationCallback(this.ServerChangeCallback));
						}, 2);
						if (!adoperationResult.Succeeded)
						{
							this.LogFailure(ADConfigurationLoader<ADConfigType, StateType>.FailureLocation.ADNotificationRegistration, adoperationResult.Exception);
							this.UpdateTimer(this.failureRetryInterval);
						}
					}
				}
			}
		}

		private void ServerChangeCallback(ADNotificationEventArgs args)
		{
			lock (this.notificationLock)
			{
				if (!this.hasUnregisteredNotification)
				{
					this.OnServerChangeCallback(args);
				}
			}
		}

		private void UpdateTimer(TimeSpan interval)
		{
			lock (this.periodicTimerLock)
			{
				if (this.periodicTimer != null)
				{
					this.periodicTimer.Continue(interval, interval);
				}
			}
		}

		private const int DefaultFailureRetryMinutes = 1;

		private const int DefaultPeriodicReadMinutes = 15;

		private ADNotificationRequestCookie adNotificationCookie;

		private object notificationLock = new object();

		private GuardedTimer periodicTimer;

		private object periodicTimerLock = new object();

		private TimeSpan failureRetryInterval;

		private TimeSpan periodicReadInterval;

		private bool hasUnregisteredNotification;

		protected enum FailureLocation
		{
			ADNotificationRegistration,
			ADConfigurationLoading
		}
	}
}
