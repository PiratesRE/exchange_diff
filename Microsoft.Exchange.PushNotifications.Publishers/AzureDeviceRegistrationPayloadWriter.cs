using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureDeviceRegistrationPayloadWriter
	{
		public void AddDeviceId(string deviceId)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("deviceId", deviceId);
			this.deviceId = deviceId;
		}

		public void AddAzureTag(string tag)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("tag", tag);
			this.azureTag = tag;
		}

		public void AddRegistrationTemplate(string registrationTemplate)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("registrationTemplate", registrationTemplate);
			this.registrationTemplate = registrationTemplate;
		}

		public override string ToString()
		{
			if (!string.IsNullOrWhiteSpace(this.registrationTemplate))
			{
				return string.Format(this.registrationTemplate, this.azureTag, this.deviceId);
			}
			return string.Empty;
		}

		private const string RequestBodyTemplate = "{{\"Channel\":\"{0}\",\"ApplicationPlatform\":\"{1}\",\"DeviceChallenge\":\"{2}\"}}";

		private string azureTag;

		private string deviceId;

		private string registrationTemplate;
	}
}
