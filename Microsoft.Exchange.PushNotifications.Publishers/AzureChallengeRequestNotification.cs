using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureChallengeRequestNotification : PushNotification
	{
		public AzureChallengeRequestNotification(string appId, string targetAppId, IAzureSasTokenProvider sasTokenProvider, AzureUriTemplate uriTemplate, string deviceId, AzureChallengeRequestPayload payload, string hubName) : base(appId, OrganizationId.ForestWideOrgId)
		{
			this.TargetAppId = targetAppId;
			this.AzureSasTokenProvider = sasTokenProvider;
			this.UriTemplate = uriTemplate;
			this.deviceId = deviceId;
			this.Payload = payload;
			this.HubName = hubName;
		}

		public override string RecipientId
		{
			get
			{
				return this.deviceId;
			}
		}

		public AzureChallengeRequestPayload Payload { get; private set; }

		public string TargetAppId { get; private set; }

		public IAzureSasTokenProvider AzureSasTokenProvider { get; private set; }

		public AzureUriTemplate UriTemplate { get; private set; }

		public string SerializedPaylod { get; private set; }

		public string HubName { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			if (this.AzureSasTokenProvider == null)
			{
				errors.Add(Strings.AzureChallengeEmptySasKey);
			}
			if (this.UriTemplate == null)
			{
				errors.Add(Strings.AzureChallengeEmptyUriTemplate);
			}
			if (string.IsNullOrWhiteSpace(this.deviceId))
			{
				errors.Add(Strings.AzureChallengeInvalidDeviceId(this.RecipientId));
			}
			if (this.Payload == null)
			{
				errors.Add(Strings.AzureChallengeMissingPayload);
			}
			if (!this.Payload.TargetPlatform.SupportsIssueRegistrationSecret())
			{
				errors.Add(Strings.AzureChallengeInvalidPlatformOnPayload(this.Payload.TargetPlatform.ToString()));
			}
			this.SerializedPaylod = this.ToAzureHubCreationFormat();
		}

		protected override string InternalToFullString()
		{
			return string.Format("{0}; targetAppId:{1}; sasKey:{2}; uriTemplate:{3}; deviceId:{4}; payload:{5}; hubName:{6}", new object[]
			{
				base.InternalToFullString(),
				this.TargetAppId,
				this.AzureSasTokenProvider,
				this.UriTemplate,
				this.RecipientId,
				this.Payload.ToString(),
				this.HubName
			});
		}

		private string ToAzureHubCreationFormat()
		{
			AzureChallengeRequestPayloadWriter azureChallengeRequestPayloadWriter = new AzureChallengeRequestPayloadWriter();
			azureChallengeRequestPayloadWriter.AddDeviceId(this.RecipientId);
			if (this.Payload != null)
			{
				this.Payload.WriteAzureSecretRequestPayload(azureChallengeRequestPayloadWriter);
			}
			return azureChallengeRequestPayloadWriter.ToString();
		}

		private readonly string deviceId;
	}
}
