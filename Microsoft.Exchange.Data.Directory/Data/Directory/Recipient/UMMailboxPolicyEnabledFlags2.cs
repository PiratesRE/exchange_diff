using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	public enum UMMailboxPolicyEnabledFlags2
	{
		None = 0,
		AllowFax = 1,
		AllowTUIAccessToCalendar = 2,
		AllowTUIAccessToEmail = 4,
		AllowSubscriberAccess = 8,
		AllowTUIAccessToDirectory = 16,
		AllowASR = 32,
		AllowPlayOnPhone = 64,
		AllowVoiceMailPreview = 128,
		AllowPersonalAutoAttendant = 256,
		AllowMessageWaitingIndicator = 512,
		AllowTUIAccessToPersonalContacts = 1024,
		AllowSMSNotification = 2048,
		AllowVoiceResponseToOtherMessageTypes = 4096,
		InformCallerOfVoiceMailAnalysis = 8192,
		AllowVoiceNotification = 16384,
		Default = 65534
	}
}
