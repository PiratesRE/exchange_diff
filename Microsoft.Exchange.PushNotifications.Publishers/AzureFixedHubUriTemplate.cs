using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AzureFixedHubUriTemplate : AzureUriTemplate
	{
		public AzureFixedHubUriTemplate(string uriTemplate) : base(uriTemplate)
		{
		}

		public override string CreateSendNotificationStringUri(AzureNotification notification)
		{
			return string.Format(base.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(notification.AppId), "messages");
		}

		public override string CreateReadRegistrationStringUri(AzureNotification notification)
		{
			return string.Format(base.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(notification.AppId), base.CreateReadRegistrationAction(notification.RecipientId));
		}

		public override string CreateReadRegistrationStringUri(AzureDeviceRegistrationNotification notification)
		{
			return string.Format(base.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(notification.TargetAppId), base.CreateReadRegistrationAction(notification.RecipientId));
		}

		public override string CreateNewRegistrationStringUri(string appId, string hubName)
		{
			return string.Format(base.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(appId), "registrations");
		}

		public override string CreateNewRegistrationStringUri(AzureNotification notification)
		{
			return this.CreateNewRegistrationStringUri(notification.AppId, notification.HubName);
		}

		public override string CreateIssueRegistrationSecretStringUri(string targetAppId, string hubName)
		{
			return string.Format(base.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(targetAppId), "issueregistrationsecret");
		}

		public override string CreateNewRegistrationIdStringUri(AzureDeviceRegistrationNotification notification)
		{
			return string.Format(base.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(notification.TargetAppId), "registrationIDs");
		}

		public override string CreateOrUpdateRegistrationStringUri(AzureDeviceRegistrationNotification notification, string registrationId)
		{
			return string.Format(base.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(notification.TargetAppId), string.Format("{0}/{1}", "registrations", registrationId));
		}

		public override string CreateOnPremResourceStringUri(string appId, string hubName)
		{
			ArgumentValidator.ThrowIfInvalidValue<string>("hubName", hubName, (string x) => base.UriTemplate.Contains(x));
			return string.Format(base.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(appId), string.Empty);
		}

		protected override void ValidateUriTemplate(string uriTemplate)
		{
			base.ValidateUriTemplate(uriTemplate);
			string formatedString = string.Format(uriTemplate, "ns", "ac");
			ArgumentValidator.ThrowIfInvalidValue<string>("uriTemplate-Uri", uriTemplate, (string x) => Uri.IsWellFormedUriString(formatedString, UriKind.Absolute));
			ArgumentValidator.ThrowIfInvalidValue<string>("uriTemplate-Order", uriTemplate, (string x) => formatedString.IndexOf("ns") < formatedString.IndexOf("ac"));
		}
	}
}
