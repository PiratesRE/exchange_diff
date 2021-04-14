using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PushNotificationPublishingContext
	{
		public PushNotificationPublishingContext(string source, OrganizationId orgId, bool requiresRegistration = false, string hubName = null)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("source", source);
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			this.Source = source;
			this.OrgId = orgId;
			this.ReceivedTime = ExDateTime.UtcNow;
			this.RequireDeviceRegistration = requiresRegistration;
			this.HubName = hubName;
		}

		public string Source { get; private set; }

		public ExDateTime ReceivedTime { get; private set; }

		public OrganizationId OrgId { get; private set; }

		public bool RequireDeviceRegistration { get; private set; }

		public string HubName { get; private set; }
	}
}
