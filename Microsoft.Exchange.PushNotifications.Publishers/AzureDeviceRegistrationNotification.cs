using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureDeviceRegistrationNotification : PushNotification
	{
		public AzureDeviceRegistrationNotification(string appId, string targetAppId, IAzureSasTokenProvider sasTokenProvider, AzureUriTemplate uriTemplate, AzureDeviceRegistrationPayload payload, string hubName, string serverChallenge = null) : base(appId, OrganizationId.ForestWideOrgId)
		{
			this.TargetAppId = targetAppId;
			this.AzureSasTokenProvider = sasTokenProvider;
			this.UriTemplate = uriTemplate;
			this.Payload = payload;
			this.HubName = hubName;
			this.ServerChallenge = serverChallenge;
		}

		public override string RecipientId
		{
			get
			{
				return this.azureTag;
			}
		}

		public string TargetAppId { get; private set; }

		public IAzureSasTokenProvider AzureSasTokenProvider { get; private set; }

		public AzureUriTemplate UriTemplate { get; private set; }

		public string HubName { get; private set; }

		public string ServerChallenge { get; private set; }

		public AzureDeviceRegistrationPayload Payload { get; private set; }

		public string SerializedPaylod { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			if (this.AzureSasTokenProvider == null)
			{
				errors.Add(Strings.AzureDeviceEmptySasKey);
			}
			if (this.UriTemplate == null)
			{
				errors.Add(Strings.AzureDeviceEmptyUriTemplate);
			}
			if (this.Payload == null)
			{
				errors.Add(Strings.AzureDeviceMissingPayload);
			}
			else
			{
				if (string.IsNullOrWhiteSpace(this.Payload.DeviceId))
				{
					errors.Add(Strings.AzureDeviceInvalidDeviceId(this.Payload.DeviceId));
				}
				if (string.IsNullOrWhiteSpace(this.Payload.AzureTag))
				{
					errors.Add(Strings.AzureDeviceInvalidTag(this.Payload.AzureTag));
				}
				this.azureTag = this.Payload.AzureTag;
			}
			this.SerializedPaylod = this.ToAzureDeviceRegistrationFormat();
		}

		protected override string InternalToFullString()
		{
			return string.Format("{0}; targetAppId:{1}; sasKey:{2}; uriTemplate:{3}; deviceId:{4}; payload:{5}; hubName:{6}; challenge:{7}", new object[]
			{
				base.InternalToFullString(),
				this.TargetAppId,
				this.AzureSasTokenProvider,
				this.UriTemplate,
				this.RecipientId,
				this.Payload.ToString(),
				this.HubName,
				this.ServerChallenge
			});
		}

		private string ToAzureDeviceRegistrationFormat()
		{
			AzureDeviceRegistrationPayloadWriter azureDeviceRegistrationPayloadWriter = new AzureDeviceRegistrationPayloadWriter();
			if (this.Payload != null)
			{
				this.Payload.WriteAzureDeviceRegistrationPayload(azureDeviceRegistrationPayloadWriter);
			}
			return azureDeviceRegistrationPayloadWriter.ToString();
		}

		private string azureTag;
	}
}
