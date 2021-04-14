using System;

namespace Microsoft.Exchange.Data.Storage.Clutter
{
	internal enum ClutterNotificationType
	{
		None,
		Invitation,
		OptedIn,
		FirstReminder,
		SecondReminder,
		ThirdReminder,
		AutoEnablementNotice
	}
}
