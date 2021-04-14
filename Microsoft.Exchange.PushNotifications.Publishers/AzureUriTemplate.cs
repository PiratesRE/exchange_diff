using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class AzureUriTemplate
	{
		protected AzureUriTemplate(string uriTemplate)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("uriTemplate", uriTemplate);
			this.ValidateUriTemplate(uriTemplate);
			this.UriTemplate = uriTemplate;
		}

		public string UriTemplate { get; private set; }

		public static AzureUriTemplate CreateUriTemplate(string uriTemplate, string additionalParameters = null)
		{
			if (uriTemplate != null)
			{
				if (uriTemplate.Contains("{3}"))
				{
					return new AzurePartitionUriTemplate(uriTemplate, additionalParameters);
				}
				if (uriTemplate.Contains("{2}"))
				{
					return new AzureVariableHubUriTemplate(uriTemplate);
				}
			}
			return new AzureFixedHubUriTemplate(uriTemplate);
		}

		public abstract string CreateSendNotificationStringUri(AzureNotification notification);

		public abstract string CreateReadRegistrationStringUri(AzureNotification notification);

		public abstract string CreateReadRegistrationStringUri(AzureDeviceRegistrationNotification notification);

		public abstract string CreateNewRegistrationStringUri(string appId, string hubName);

		public abstract string CreateNewRegistrationStringUri(AzureNotification notification);

		public abstract string CreateOrUpdateRegistrationStringUri(AzureDeviceRegistrationNotification notification, string registrationId);

		public abstract string CreateNewRegistrationIdStringUri(AzureDeviceRegistrationNotification notification);

		public virtual string CreateIssueRegistrationSecretStringUri(string targetAppId, string hubName)
		{
			return string.Format(this.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(targetAppId), hubName, "issueregistrationsecret");
		}

		public virtual string CreateTargetHubCreationStringUri(AzureHubCreationNotification notification)
		{
			return string.Format(this.UriTemplate, AzureUriTemplate.ConvertAppIdToValidNamespace(notification.AppId), notification.HubName);
		}

		public abstract string CreateOnPremResourceStringUri(string appId, string hubName);

		public override string ToString()
		{
			return this.UriTemplate;
		}

		internal static string ConvertAppIdToValidNamespace(string appId)
		{
			if (appId.Contains("."))
			{
				return appId.Replace('.', '-');
			}
			return appId;
		}

		protected virtual void ValidateUriTemplate(string uriTemplate)
		{
			ArgumentValidator.ThrowIfInvalidValue<string>("uriTemplate-Format", uriTemplate, (string x) => x.Contains("{0}") && x.Contains("{1}"));
		}

		protected string CreateReadRegistrationAction(string azureTag)
		{
			return string.Format("tags/{0}/registrations", azureTag);
		}

		protected const string SendingAction = "messages";

		protected const string ReadRegisteringActionTemplate = "tags/{0}/registrations";

		protected const string NewRegisteringActionTemplate = "registrations";

		protected const string NewRegistrationIdActionTemplate = "registrationIDs";

		protected const string IssueRegistrationSecretTemplate = "issueregistrationsecret";
	}
}
