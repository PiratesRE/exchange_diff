using System;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class InstantSearchNotificationHandler : IInstantSearchNotificationHandler, IDisposable
	{
		public InstantSearchNotificationHandler(UserContext userContext)
		{
			this.notifier = new InstantSearchNotifier("InstantSearchNotification", userContext);
			this.notifier.RegisterWithPendingRequestNotifier();
		}

		public void DeliverInstantSearchPayload(InstantSearchPayloadType instantSearchPayloadType)
		{
			this.notifier.AddPayload(new InstantSearchNotificationPayload("InstantSearchNotification", instantSearchPayloadType));
		}

		public void Dispose()
		{
			this.notifier.UnregisterWithPendingRequestNotifier();
		}

		public const string InstantSearchSubscriptionId = "InstantSearchNotification";

		private readonly InstantSearchNotifier notifier;
	}
}
