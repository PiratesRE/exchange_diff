using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Flags]
	public enum NewItemNotification
	{
		None = 0,
		Sound = 1,
		EMailToast = 2,
		VoiceMailToast = 4,
		FaxToast = 8,
		All = 15
	}
}
