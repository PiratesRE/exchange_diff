using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AzureVariableHubUriTemplate : AzureUriTemplate
	{
		public AzureVariableHubUriTemplate(string uriTemplate) : base(uriTemplate)
		{
		}

		public override string CreateSendNotificationStringUri(AzureNotification notification)
		{
			return string.Format(base.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(notification.AppId), notification.HubName, "messages");
		}

		public override string CreateReadRegistrationStringUri(AzureNotification notification)
		{
			return string.Format(base.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(notification.AppId), notification.HubName, base.CreateReadRegistrationAction(notification.RecipientId));
		}

		public override string CreateReadRegistrationStringUri(AzureDeviceRegistrationNotification notification)
		{
			return string.Format(base.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(notification.TargetAppId), notification.HubName, base.CreateReadRegistrationAction(notification.RecipientId));
		}

		public override string CreateNewRegistrationStringUri(string appId, string hubName)
		{
			return string.Format(base.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(appId), hubName, "registrations");
		}

		public override string CreateNewRegistrationStringUri(AzureNotification notification)
		{
			return this.CreateNewRegistrationStringUri(notification.AppId, notification.HubName);
		}

		public override string CreateNewRegistrationIdStringUri(AzureDeviceRegistrationNotification notification)
		{
			return string.Format(base.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(notification.TargetAppId), notification.HubName, "registrationIDs");
		}

		public override string CreateOrUpdateRegistrationStringUri(AzureDeviceRegistrationNotification notification, string registrationId)
		{
			return string.Format(base.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(notification.TargetAppId), notification.HubName, string.Format("{0}/{1}", "registrations", registrationId));
		}

		public override string CreateOnPremResourceStringUri(string appId, string hubName)
		{
			return string.Format(base.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(appId), hubName, string.Empty);
		}

		protected override void ValidateUriTemplate(string uriTemplate)
		{
			base.ValidateUriTemplate(uriTemplate);
			string formatedString = string.Format(uriTemplate, "ns", "hb", "ac");
			ArgumentValidator.ThrowIfInvalidValue<string>("uriTemplate-Uri", uriTemplate, (string x) => Uri.IsWellFormedUriString(formatedString, UriKind.Absolute));
			ArgumentValidator.ThrowIfInvalidValue<string>("uriTemplate-Order", uriTemplate, (string x) => formatedString.IndexOf("ns") < formatedString.IndexOf("hb") && formatedString.IndexOf("hb") < formatedString.IndexOf("ac"));
		}
	}
}
