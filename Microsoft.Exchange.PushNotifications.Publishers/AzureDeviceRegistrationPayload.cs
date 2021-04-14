using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureDeviceRegistrationPayload
	{
		public AzureDeviceRegistrationPayload(string deviceId, string registrationTemplate, string tag)
		{
			this.DeviceId = deviceId;
			this.RegistrationTemplate = registrationTemplate;
			this.AzureTag = tag;
		}

		public string DeviceId { get; private set; }

		public string RegistrationTemplate { get; private set; }

		public string AzureTag { get; private set; }

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = string.Format("{{deviceId:{0}; tag:{1}; template:{2}}}", this.DeviceId, this.AzureTag, this.RegistrationTemplate);
			}
			return this.toString;
		}

		internal void WriteAzureDeviceRegistrationPayload(AzureDeviceRegistrationPayloadWriter apw)
		{
			ArgumentValidator.ThrowIfNull("apw", apw);
			apw.AddRegistrationTemplate(this.RegistrationTemplate);
			apw.AddAzureTag(this.AzureTag);
			apw.AddDeviceId(this.DeviceId);
		}

		private string toString;
	}
}
