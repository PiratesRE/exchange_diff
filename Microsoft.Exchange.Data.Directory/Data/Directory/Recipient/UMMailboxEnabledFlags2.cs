using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	public enum UMMailboxEnabledFlags2
	{
		None = 0,
		PlayOnPhoneEnabled = 1,
		PersonalAutoAttendantEnabled = 4,
		Default = -1
	}
}
