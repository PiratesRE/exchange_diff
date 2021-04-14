using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Publishers;
using Microsoft.Exchange.PushNotifications.Server.Services;

namespace Microsoft.Exchange.PushNotifications.Server.Commands
{
	internal class PublishProxyNotifications : PublishNotificationsBase<MailboxNotificationBatch>
	{
		public PublishProxyNotifications(MailboxNotificationBatch notifications, PushNotificationPublisherManager publisherManager, AsyncCallback asyncCallback, object asyncState) : base(notifications, publisherManager, asyncCallback, asyncState)
		{
		}

		public OrganizationId AuthenticatedTenantId { get; private set; }

		protected override void InternalInitialize(IBudget budget)
		{
			base.InternalInitialize(budget);
			TenantBudgetKey tenantBudgetKey = budget.Owner as TenantBudgetKey;
			if (tenantBudgetKey != null && tenantBudgetKey.OrganizationId != null)
			{
				this.AuthenticatedTenantId = tenantBudgetKey.OrganizationId;
			}
			if (this.AuthenticatedTenantId == null)
			{
				this.AuthenticatedTenantId = OrganizationId.ForestWideOrgId;
				string text = budget.Owner.ToString();
				if (base.RequestInstance == null || !base.RequestInstance.IsMonitoring() || !object.ReferenceEquals(budget.Owner, PushNotificationService.ServiceBudgetKey))
				{
					PushNotificationsCrimsonEvents.CannotResolveAuthenticatedTenantId.LogPeriodic<string>(text, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, text);
					ExTraceGlobals.PushNotificationServiceTracer.TraceError((long)this.GetHashCode(), string.Format("Failed to resolve the authenticated Organization for the current request: '{0}'.", text));
				}
			}
		}

		protected override void Publish()
		{
			foreach (MailboxNotification notification in base.RequestInstance.Notifications)
			{
				PushNotificationPublishingContext context = new PushNotificationPublishingContext(base.NotificationSource, this.AuthenticatedTenantId, true, null);
				base.PublisherManager.Publish(notification, context);
			}
		}
	}
}
