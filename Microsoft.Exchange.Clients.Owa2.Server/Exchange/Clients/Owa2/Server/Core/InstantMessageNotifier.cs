using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class InstantMessageNotifier : PendingRequestNotifierBase
	{
		internal InstantMessageNotifier(IUserContext userContext) : base(userContext)
		{
			this.pendingNotifications = new List<NotificationPayloadBase>();
			base.UserContext.PendingRequestManager.KeepAlive += this.KeepAlive;
		}

		public event EventHandler<EventArgs> ChangeUserPresenceAfterInactivity;

		public new string SubscriptionId
		{
			get
			{
				return base.SubscriptionId;
			}
			set
			{
				base.SubscriptionId = value;
			}
		}

		public override bool ShouldThrottle
		{
			get
			{
				return true;
			}
		}

		public bool IsRegistered
		{
			get
			{
				return !string.IsNullOrEmpty(this.SubscriptionId);
			}
		}

		public int PendingCount
		{
			get
			{
				return this.pendingNotifications.Count;
			}
		}

		internal List<NotificationPayloadBase> PendingNotifications
		{
			get
			{
				return this.pendingNotifications;
			}
		}

		public void Clear()
		{
			this.pendingNotifications.Clear();
		}

		public void Add(InstantMessagePayload payloadItem)
		{
			if (this.pendingNotifications.Count < 10000)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "Queuing IM payload item {0} for user {1}", new object[]
				{
					payloadItem.PayloadType,
					this.GetUriForUser()
				});
				if (!this.isOverMaxSize)
				{
					payloadItem.SubscriptionId = this.SubscriptionId;
					this.pendingNotifications.Add(payloadItem);
					return;
				}
			}
			else
			{
				this.LogPayloadNotPickedEvent();
			}
		}

		internal void KeepAlive(object sender, EventArgs e)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageNotifier.KeepAlive. User: {0}", new object[]
			{
				this.GetUriForUser()
			});
			long num = Globals.ApplicationTime - ((IUserContext)base.UserContext).LastUserRequestTime;
			if (num > (long)Globals.ActivityBasedPresenceDuration)
			{
				EventArgs e2 = new EventArgs();
				EventHandler<EventArgs> changeUserPresenceAfterInactivity = this.ChangeUserPresenceAfterInactivity;
				if (changeUserPresenceAfterInactivity != null)
				{
					changeUserPresenceAfterInactivity(this, e2);
				}
			}
		}

		protected override IList<NotificationPayloadBase> ReadDataAndResetStateInternal()
		{
			IList<NotificationPayloadBase> result = null;
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageNotifier.ReadDataAndResetStateInternal. SIP Uri: {0}", new object[]
			{
				this.GetUriForUser()
			});
			lock (this)
			{
				result = new List<NotificationPayloadBase>(this.pendingNotifications);
				this.Clear();
			}
			return result;
		}

		protected override bool IsDataAvailableForPickup()
		{
			return this.pendingNotifications.Count > 0;
		}

		protected virtual void Cancel()
		{
			this.Clear();
		}

		private void LogPayloadNotPickedEvent()
		{
			if (!this.isOverMaxSize)
			{
				this.isOverMaxSize = true;
				string uriForUser = this.GetUriForUser();
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageNotifier.LogPayloadNotPickedEvent. Payload has grown too large without being picked up. User: {0}", new object[]
				{
					uriForUser
				});
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_PayloadNotBeingPickedup, string.Empty, new object[]
				{
					this.GetUriForUser()
				});
				PendingRequestManager pendingRequestManager = base.UserContext.PendingRequestManager;
				if (pendingRequestManager != null && pendingRequestManager.HasAnyActivePendingGetChannel())
				{
					InstantMessageUtilities.SendWatsonReport("InstantMessageNotifier.LogPayloadNotPickedEvent", (IUserContext)base.UserContext, new OverflowException(string.Format("Payload has grown too large without being picked up. User: {0}", uriForUser)));
				}
				this.Cancel();
				this.Add(new InstantMessagePayload(InstantMessagePayloadType.ServiceUnavailable)
				{
					ServiceError = InstantMessageServiceError.OverMaxPayloadSize
				});
			}
		}

		private string GetUriForUser()
		{
			if (((IUserContext)base.UserContext).InstantMessageType != InstantMessagingTypeOptions.Ocs)
			{
				return string.Empty;
			}
			return ((IUserContext)base.UserContext).SipUri;
		}

		private const int MaxPayloadSize = 10000;

		private volatile bool isOverMaxSize;

		private List<NotificationPayloadBase> pendingNotifications;
	}
}
