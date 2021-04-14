using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureHubCreationNotification : PushNotification
	{
		public AzureHubCreationNotification(string appId, string hubName, string partition, AzureHubPayload payload) : base(appId, OrganizationId.ForestWideOrgId)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("hubName", hubName);
			ArgumentValidator.ThrowIfNull("payload", payload);
			this.AzureNamespace = appId;
			this.HubName = hubName;
			this.Partition = partition;
			this.Payload = payload;
		}

		public override string RecipientId
		{
			get
			{
				return this.HubName;
			}
		}

		public string AzureNamespace { get; private set; }

		public AzureHubPayload Payload { get; private set; }

		public string HubName { get; private set; }

		public string Partition { get; private set; }

		public string SerializedPaylod { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			this.SerializedPaylod = this.ToAzureHubCreationFormat();
			if (Encoding.UTF8.GetByteCount(this.SerializedPaylod) > 8192)
			{
				errors.Add(Strings.InvalidAzurePayloadLength(8192, this.SerializedPaylod));
			}
		}

		protected override string InternalToFullString()
		{
			return string.Format("{0}; hub:{1}; partition: {2}; payload:{3}", new object[]
			{
				base.InternalToFullString(),
				this.HubName,
				this.Partition,
				this.Payload.ToString()
			});
		}

		private string ToAzureHubCreationFormat()
		{
			AzureHubPayloadWriter azureHubPayloadWriter = new AzureHubPayloadWriter();
			if (this.Payload != null)
			{
				this.Payload.WriteAzureHubPayload(azureHubPayloadWriter);
			}
			return azureHubPayloadWriter.ToString();
		}

		private const int MaxPayloadSize = 8192;
	}
}
