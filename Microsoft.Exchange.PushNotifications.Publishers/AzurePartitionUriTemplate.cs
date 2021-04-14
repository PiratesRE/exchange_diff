using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AzurePartitionUriTemplate : AzureUriTemplate
	{
		public AzurePartitionUriTemplate(string uriTemplate, string partitionName) : base(uriTemplate)
		{
			this.partitionName = partitionName;
		}

		public override string CreateSendNotificationStringUri(AzureNotification notification)
		{
			return string.Format(base.UriTemplate, new object[]
			{
				AzureUriTemplate.ConvertAppIdToValidNamespace(notification.AppId),
				this.partitionName,
				notification.HubName,
				"messages"
			});
		}

		public override string CreateReadRegistrationStringUri(AzureNotification notification)
		{
			return string.Format(base.UriTemplate, new object[]
			{
				AzureUriTemplate.ConvertAppIdToValidNamespace(notification.AppId),
				this.partitionName,
				notification.HubName,
				base.CreateReadRegistrationAction(notification.RecipientId)
			});
		}

		public override string CreateReadRegistrationStringUri(AzureDeviceRegistrationNotification notification)
		{
			return string.Format(base.UriTemplate, new object[]
			{
				AzureUriTemplate.ConvertAppIdToValidNamespace(notification.TargetAppId),
				this.partitionName,
				notification.HubName,
				base.CreateReadRegistrationAction(notification.RecipientId)
			});
		}

		public override string CreateNewRegistrationStringUri(string appId, string hubName)
		{
			return string.Format(base.UriTemplate, new object[]
			{
				AzureUriTemplate.ConvertAppIdToValidNamespace(appId),
				this.partitionName,
				hubName,
				"registrations"
			});
		}

		public override string CreateNewRegistrationStringUri(AzureNotification notification)
		{
			return this.CreateNewRegistrationStringUri(notification.AppId, notification.HubName);
		}

		public override string CreateNewRegistrationIdStringUri(AzureDeviceRegistrationNotification notification)
		{
			return string.Format(base.UriTemplate, new object[]
			{
				AzureUriTemplate.ConvertAppIdToValidNamespace(notification.TargetAppId),
				this.partitionName,
				notification.HubName,
				"registrationIDs"
			});
		}

		public override string CreateOrUpdateRegistrationStringUri(AzureDeviceRegistrationNotification notification, string registrationId)
		{
			return string.Format(base.UriTemplate, new object[]
			{
				AzureUriTemplate.ConvertAppIdToValidNamespace(notification.TargetAppId),
				this.partitionName,
				notification.HubName,
				string.Format("{0}/{1}", "registrations", registrationId)
			});
		}

		public override string CreateTargetHubCreationStringUri(AzureHubCreationNotification notification)
		{
			return string.Format(base.UriTemplate, new object[]
			{
				AzureUriTemplate.ConvertAppIdToValidNamespace(notification.AppId),
				notification.Partition,
				notification.HubName,
				string.Empty
			});
		}

		public override string CreateIssueRegistrationSecretStringUri(string targetAppId, string hubName)
		{
			return string.Format(base.UriTemplate, new object[]
			{
				AzureUriTemplate.ConvertAppIdToValidNamespace(targetAppId),
				this.partitionName,
				hubName,
				"issueregistrationsecret"
			});
		}

		public override string CreateOnPremResourceStringUri(string appId, string hubName)
		{
			return string.Format(base.UriTemplate, new object[]
			{
				AzureUriTemplate.ConvertAppIdToValidNamespace(appId),
				this.partitionName,
				hubName,
				string.Empty
			});
		}

		protected override void ValidateUriTemplate(string uriTemplate)
		{
			base.ValidateUriTemplate(uriTemplate);
			string formatedString = string.Format(uriTemplate, new object[]
			{
				"ns",
				"pn",
				"hb",
				"ac"
			});
			ArgumentValidator.ThrowIfInvalidValue<string>("uriTemplate-Uri", uriTemplate, (string x) => Uri.IsWellFormedUriString(formatedString, UriKind.Absolute));
			ArgumentValidator.ThrowIfInvalidValue<string>("uriTemplate-Order", uriTemplate, (string x) => formatedString.IndexOf("ns") < formatedString.IndexOf("pn") && formatedString.IndexOf("pn") < formatedString.IndexOf("hb") && formatedString.IndexOf("hb") < formatedString.IndexOf("ac"));
		}

		private readonly string partitionName;
	}
}
