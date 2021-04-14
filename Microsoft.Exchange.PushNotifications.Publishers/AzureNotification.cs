using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Security.Compliance;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureNotification : PushNotification
	{
		public AzureNotification(string appId, string recipient, OrganizationId tenantId, AzurePayload payload, bool enableDeviceRegistration = false) : base(appId, tenantId)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("recipient", recipient);
			ArgumentValidator.ThrowIfNull("payload", payload);
			if (AzureNotification.IsLegacyRecipientId(recipient))
			{
				this.recipientId = AzureNotification.ComputeHashTag(recipient);
			}
			else
			{
				this.recipientId = recipient;
			}
			this.DeviceId = recipient;
			this.Payload = payload;
			this.IsRegistrationEnabled = enableDeviceRegistration;
			if (payload != null)
			{
				payload.NotificationId = base.Identifier;
				base.IsBackgroundSyncAvailable = payload.IsBackground;
			}
		}

		public AzureNotification(string appId, string recipient, string hubName, AzurePayload payload, bool enableDeviceRegistration = false) : base(appId, OrganizationId.ForestWideOrgId)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("recipient", recipient);
			ArgumentValidator.ThrowIfNull("payload", payload);
			if (AzureNotification.IsLegacyRecipientId(recipient))
			{
				this.recipientId = AzureNotification.ComputeHashTag(recipient);
			}
			else
			{
				this.recipientId = recipient;
			}
			this.DeviceId = recipient;
			this.HubName = hubName;
			this.Payload = payload;
			this.IsRegistrationEnabled = enableDeviceRegistration;
			if (payload != null)
			{
				payload.NotificationId = base.Identifier;
				base.IsBackgroundSyncAvailable = payload.IsBackground;
			}
		}

		public override string RecipientId
		{
			get
			{
				return this.recipientId;
			}
		}

		public AzurePayload Payload { get; private set; }

		public string HubName { get; private set; }

		public string SerializedPaylod { get; private set; }

		public bool IsRegistrationEnabled { get; private set; }

		public string DeviceId { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			try
			{
				if (this.HubName == null)
				{
					this.HubName = base.TenantId.ToExternalDirectoryOrganizationId();
				}
			}
			catch (CannotResolveExternalDirectoryOrganizationIdException exception)
			{
				errors.Add(Strings.AzureCannotResolveExternalOrgId(exception.ToTraceString()));
			}
			if (string.IsNullOrWhiteSpace(this.recipientId))
			{
				errors.Add(Strings.AzureInvalidRecipientId(this.recipientId));
			}
			this.SerializedPaylod = this.ToAzureTemplateFormat();
			if (Encoding.UTF8.GetByteCount(this.SerializedPaylod) > 8192)
			{
				errors.Add(Strings.InvalidAzurePayloadLength(8192, this.SerializedPaylod));
			}
		}

		protected override string InternalToFullString()
		{
			return string.Format("{0}; recipientId:{1}; hub:{2}; payload:{3};", new object[]
			{
				base.InternalToFullString(),
				this.RecipientId,
				this.HubName,
				this.Payload.ToString()
			});
		}

		private static string ComputeHashTag(string recipientId)
		{
			string result;
			using (HMACSHA256Cng hmacsha256Cng = new HMACSHA256Cng(Encoding.UTF8.GetBytes("O7sfRRXL7dbltiobjozqaSO6qVSIm94OUJrlC5fsGGG=")))
			{
				result = HexConverter.ByteArrayToHexString(hmacsha256Cng.ComputeHash(Encoding.UTF8.GetBytes(recipientId)));
			}
			return result;
		}

		private static bool IsLegacyRecipientId(string recipientId)
		{
			return recipientId.Length >= 64;
		}

		private string ToAzureTemplateFormat()
		{
			AzurePayloadWriter azurePayloadWriter = new AzurePayloadWriter();
			if (this.Payload != null)
			{
				this.Payload.WriteAzurePayload(azurePayloadWriter);
			}
			return azurePayloadWriter.ToString();
		}

		private const int MaxPayloadSize = 8192;

		private const int ApnsDeviceTokenSize = 64;

		private const string HashingKey = "O7sfRRXL7dbltiobjozqaSO6qVSIm94OUJrlC5fsGGG=";

		private readonly string recipientId;
	}
}
