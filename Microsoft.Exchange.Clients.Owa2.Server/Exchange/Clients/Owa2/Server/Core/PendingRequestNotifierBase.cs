using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class PendingRequestNotifierBase : IPendingRequestNotifier
	{
		public PendingRequestNotifierBase(IMailboxContext userContext) : this(null, userContext)
		{
		}

		public PendingRequestNotifierBase(string subscriptionId, IMailboxContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			this.SubscriptionId = subscriptionId;
			this.UserContext = userContext;
		}

		public event DataAvailableEventHandler DataAvailable;

		public virtual bool ShouldThrottle
		{
			get
			{
				return true;
			}
		}

		public string SubscriptionId { get; protected set; }

		public string ContextKey
		{
			get
			{
				string result = null;
				if (this.UserContext != null)
				{
					result = this.UserContext.Key.ToString();
				}
				return result;
			}
		}

		protected IMailboxContext UserContext { get; set; }

		public virtual IList<NotificationPayloadBase> ReadDataAndResetState()
		{
			List<NotificationPayloadBase> result = null;
			lock (this)
			{
				this.containsDataToPickup = false;
				result = (List<NotificationPayloadBase>)this.ReadDataAndResetStateInternal();
			}
			return result;
		}

		internal void RegisterWithPendingRequestNotifier()
		{
			if (this.UserContext != null && this.UserContext.PendingRequestManager != null)
			{
				lock (this)
				{
					if (this.hasNotifierBeenUnregistered)
					{
						throw new InvalidOperationException("A notifier should not be reused after being unregistered");
					}
				}
				this.UserContext.PendingRequestManager.AddPendingRequestNotifier(this);
			}
		}

		internal void UnregisterWithPendingRequestNotifier()
		{
			if (this.UserContext != null && this.UserContext.PendingRequestManager != null)
			{
				lock (this)
				{
					if (this.hasNotifierBeenUnregistered)
					{
						throw new InvalidOperationException("A notifier should not be unregistered twice.");
					}
					this.hasNotifierBeenUnregistered = true;
				}
				this.UserContext.PendingRequestManager.RemovePendingRequestNotifier(this);
			}
		}

		internal virtual void PickupData()
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<Type>((long)this.GetHashCode(), "PendingRequestNotifierBase.PickupData Begin. type: {0}", base.GetType());
			bool flag = false;
			lock (this)
			{
				flag = (this.IsDataAvailableForPickup() && !this.containsDataToPickup);
				if (flag)
				{
					this.containsDataToPickup = true;
				}
			}
			if (flag)
			{
				this.FireDataAvailableEvent();
				ExTraceGlobals.CoreCallTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "PickupData. FireDataAvailableEvent method. User {0}", this.UserContext.PrimarySmtpAddress);
				return;
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "PickupData. No need to call FireDataAvailableEvent method. User {0}", this.UserContext.PrimarySmtpAddress);
		}

		protected abstract IList<NotificationPayloadBase> ReadDataAndResetStateInternal();

		protected virtual bool IsDataAvailableForPickup()
		{
			return true;
		}

		protected void FireDataAvailableEvent()
		{
			DataAvailableEventHandler dataAvailable = this.DataAvailable;
			if (dataAvailable != null)
			{
				dataAvailable(this, new EventArgs());
				return;
			}
			ExTraceGlobals.CoreCallTracer.TraceError((long)this.GetHashCode(), "FireDataAvailableEvent() cannot fire because DataAvailable is null.");
			if (!this.hasNotifierBeenUnregistered)
			{
				throw new InvalidOperationException("DataAvailable has not been set");
			}
		}

		private bool containsDataToPickup;

		private volatile bool hasNotifierBeenUnregistered;
	}
}
