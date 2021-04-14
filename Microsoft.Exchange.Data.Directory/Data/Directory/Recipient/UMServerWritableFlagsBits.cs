using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	internal enum UMServerWritableFlagsBits
	{
		None = 0,
		MissedCallNotificationEnabled = 1,
		SMSVoiceMailNotificationEnabled = 2,
		SMSMissedCallNotificationEnabled = 4,
		PinlessAccessToVoiceMailEnabled = 8
	}
}
