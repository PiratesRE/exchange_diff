using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	public enum CalendarNotificationType
	{
		[LocDescription(ServerStrings.IDs.CalNotifTypeUninteresting)]
		Uninteresting,
		[LocDescription(ServerStrings.IDs.CalNotifTypeSummary)]
		Summary,
		[LocDescription(ServerStrings.IDs.CalNotifTypeReminder)]
		Reminder,
		[LocDescription(ServerStrings.IDs.CalNotifTypeNewUpdate)]
		NewUpdate,
		[LocDescription(ServerStrings.IDs.CalNotifTypeChangedUpdate)]
		ChangedUpdate,
		[LocDescription(ServerStrings.IDs.CalNotifTypeDeletedUpdate)]
		DeletedUpdate
	}
}
