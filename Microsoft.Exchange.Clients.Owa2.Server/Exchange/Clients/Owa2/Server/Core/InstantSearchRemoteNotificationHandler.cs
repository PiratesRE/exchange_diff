using System;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class InstantSearchRemoteNotificationHandler : IInstantSearchNotificationHandler, IDisposable
	{
		public InstantSearchRemoteNotificationHandler(UserContext userContext)
		{
			this.userContext = userContext;
		}

		public void RegisterNotifier(string subscriptionId)
		{
			lock (this.syncRoot)
			{
				this.Notifier = new InstantSearchNotifier(subscriptionId, this.userContext);
			}
		}

		public void DeliverInstantSearchPayload(InstantSearchPayloadType instantSearchPayloadType)
		{
			lock (this.syncRoot)
			{
				if (this.Notifier != null)
				{
					this.Notifier.AddPayload(new InstantSearchNotificationPayload(this.notifier.SubscriptionId, instantSearchPayloadType));
				}
			}
		}

		private InstantSearchNotifier Notifier
		{
			get
			{
				return this.notifier;
			}
			set
			{
				if (this.notifier == value)
				{
					return;
				}
				if (this.notifier != null)
				{
					this.notifier.UnregisterWithPendingRequestNotifier();
				}
				this.notifier = value;
				if (value != null)
				{
					value.RegisterWithPendingRequestNotifier();
				}
			}
		}

		public void Dispose()
		{
			lock (this.syncRoot)
			{
				this.Notifier = null;
			}
		}

		private readonly UserContext userContext;

		private readonly object syncRoot = new object();

		private InstantSearchNotifier notifier;
	}
}
