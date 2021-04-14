using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Publishers;

namespace Microsoft.Exchange.PushNotifications.Server.Commands
{
	internal class IssueRegistrationChallenge : PublishNotificationsBase<AzureChallengeRequestInfo>
	{
		public IssueRegistrationChallenge(AzureChallengeRequestInfo issueRegistrationInfo, PushNotificationPublisherManager publisherManager, AsyncCallback asyncCallback, object asyncState) : base(issueRegistrationInfo, publisherManager, asyncCallback, asyncState)
		{
		}

		public IssueRegistrationChallenge(AzureChallengeRequestInfo issueRegistrationInfo, PushNotificationPublisherManager publisherManager, string hubName, AsyncCallback asyncCallback, object asyncState) : base(issueRegistrationInfo, publisherManager, asyncCallback, asyncState)
		{
			this.HubName = hubName;
		}

		protected string HubName { get; set; }

		protected override void Publish()
		{
			base.PublisherManager.Publish(base.RequestInstance, new PushNotificationPublishingContext(base.NotificationSource, OrganizationId.ForestWideOrgId, false, this.HubName));
		}
	}
}
