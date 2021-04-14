using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	public enum UMEnabledFlags
	{
		None = 0,
		UMEnabled = 1,
		FaxEnabled = 2,
		TUIAccessToCalendarEnabled = 4,
		TUIAccessToEmailEnabled = 8,
		SubscriberAccessEnabled = 16,
		TUIAccessToAddressBookEnabled = 32,
		AnonymousCallersCanLeaveMessages = 256,
		ASREnabled = 512,
		VoiceMailAnalysisEnabled = 1024,
		Default = 830
	}
}
