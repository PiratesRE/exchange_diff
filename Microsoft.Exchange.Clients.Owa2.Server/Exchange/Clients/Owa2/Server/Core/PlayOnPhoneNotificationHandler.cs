using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class PlayOnPhoneNotificationHandler : DisposeTrackableBase
	{
		public PlayOnPhoneNotificationHandler(UserContext userContext)
		{
			this.notifier = this.CreateNotifier(userContext);
			this.stateProvider = this.CreateStateProvider(userContext);
		}

		public void Subscribe(string subscriptionId, string callId)
		{
			lock (this)
			{
				this.subscriptionId = subscriptionId;
				this.callId = callId;
				if (this.timer == null)
				{
					this.timer = new Timer(new TimerCallback(this.TimerCallback), null, 0, 2000);
				}
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			lock (this)
			{
				if (this.stateProvider != null)
				{
					this.stateProvider.Dispose();
					this.stateProvider = null;
				}
				if (this.timer != null)
				{
					this.timer.Dispose();
					this.timer = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PlayOnPhoneNotificationHandler>(this);
		}

		protected virtual PlayOnPhoneNotifier CreateNotifier(UserContext userContext)
		{
			PlayOnPhoneNotifier playOnPhoneNotifier = new PlayOnPhoneNotifier(userContext);
			playOnPhoneNotifier.RegisterWithPendingRequestNotifier();
			return playOnPhoneNotifier;
		}

		protected virtual IPlayOnPhoneStateProvider CreateStateProvider(UserContext userContext)
		{
			return new PlayOnPhoneStateProvider(userContext);
		}

		private void TimerCallback(object state)
		{
			if (this.timer == null || this.processingTimer)
			{
				return;
			}
			lock (this)
			{
				this.processingTimer = true;
				try
				{
					UMCallState callState = this.stateProvider.GetCallState(this.callId);
					if (this.previousState == null || this.previousState.Value != callState)
					{
						PlayOnPhoneNotificationPayload playOnPhoneNotificationPayload = new PlayOnPhoneNotificationPayload(callState.ToString());
						playOnPhoneNotificationPayload.SubscriptionId = this.subscriptionId;
						playOnPhoneNotificationPayload.Source = new TypeLocation(base.GetType());
						this.notifier.NotifyStateChange(playOnPhoneNotificationPayload);
						this.previousState = new UMCallState?(callState);
					}
					if (callState == UMCallState.Disconnected)
					{
						this.timer.Dispose();
						this.timer = null;
						this.subscriptionId = null;
						this.callId = null;
						this.previousState = null;
					}
				}
				finally
				{
					this.processingTimer = false;
				}
			}
		}

		private PlayOnPhoneNotifier notifier;

		private IPlayOnPhoneStateProvider stateProvider;

		private UMCallState? previousState;

		private bool processingTimer;

		private Timer timer;

		private string subscriptionId;

		private string callId;
	}
}
