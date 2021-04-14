using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	public enum NewNotification
	{
		None = 0,
		Sound = 1,
		EMailToast = 2,
		VoiceMailToast = 4,
		FaxToast = 8
	}
}
