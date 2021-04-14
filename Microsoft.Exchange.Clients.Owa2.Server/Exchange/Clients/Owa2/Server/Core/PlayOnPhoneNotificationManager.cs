using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class PlayOnPhoneNotificationManager : DisposeTrackableBase
	{
		internal PlayOnPhoneNotificationManager(UserContext userContext)
		{
			this.userContext = userContext;
		}

		public void SubscribeToPlayOnPhoneNotification(string subscriptionId, SubscriptionParameters parameters)
		{
			if (this.popHandler == null)
			{
				this.popHandler = new PlayOnPhoneNotificationHandler(this.userContext);
			}
			string callId = parameters.CallId;
			this.popHandler.Subscribe(subscriptionId, callId);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool>((long)this.GetHashCode(), "PlayOnPhoneNotificationManager.Dispose. IsDisposing: {0}", isDisposing);
			if (isDisposing && this.popHandler != null)
			{
				this.popHandler.Dispose();
				this.popHandler = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PlayOnPhoneNotificationManager>(this);
		}

		private const string PopCallId = "callid";

		private UserContext userContext;

		private PlayOnPhoneNotificationHandler popHandler;
	}
}
