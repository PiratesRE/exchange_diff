using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class PendingGetPublisher : PushNotificationPublisher<PendingGetNotification, PendingGetChannel>
	{
		public PendingGetPublisher(PushNotificationPublisherSettings publisherSettings, IPendingGetConnectionCache connectionCache, ITracer tracer = null, IThrottlingManager throttlingManager = null, List<IPushNotificationMapping<PendingGetNotification>> mappings = null) : base(publisherSettings, tracer ?? ExTraceGlobals.PendingGetPublisherTracer, throttlingManager, mappings, null, null)
		{
			this.connectionCache = connectionCache;
		}

		protected override PendingGetChannel CreateNotificationChannel()
		{
			return new PendingGetChannel(base.AppId, this.connectionCache, base.Tracer);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PendingGetPublisher>(this);
		}

		private IPendingGetConnectionCache connectionCache;
	}
}
