using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.UM.ClientAccess
{
	public class UMPropertiesEx : UMProperties
	{
		public bool ReceivedVoiceMailPreviewEnabled { get; set; }

		public bool SentVoiceMailPreviewEnabled { get; set; }

		public bool PlayOnPhoneEnabled { get; set; }

		public bool PinlessAccessToVoicemail { get; set; }

		public bool ReadUnreadVoicemailInFIFOOrder { get; set; }

		public UMSMSNotificationOptions SMSNotificationOption { get; set; }
	}
}
