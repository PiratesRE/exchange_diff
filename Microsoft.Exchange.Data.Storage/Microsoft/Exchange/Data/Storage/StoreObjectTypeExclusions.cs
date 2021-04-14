using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class StoreObjectTypeExclusions
	{
		public static bool E12KnownObjectType(StoreObjectType storeObjectType)
		{
			switch (storeObjectType)
			{
			case StoreObjectType.Unknown:
			case StoreObjectType.Folder:
			case StoreObjectType.CalendarFolder:
			case StoreObjectType.ContactsFolder:
			case StoreObjectType.TasksFolder:
			case StoreObjectType.NotesFolder:
			case StoreObjectType.JournalFolder:
			case StoreObjectType.SearchFolder:
			case StoreObjectType.OutlookSearchFolder:
			case StoreObjectType.Message:
			case StoreObjectType.MeetingMessage:
			case StoreObjectType.MeetingRequest:
			case StoreObjectType.MeetingResponse:
			case StoreObjectType.MeetingCancellation:
			case StoreObjectType.ConflictMessage:
			case StoreObjectType.CalendarItem:
			case StoreObjectType.CalendarItemOccurrence:
			case StoreObjectType.Contact:
			case StoreObjectType.DistributionList:
			case StoreObjectType.Task:
			case StoreObjectType.TaskRequest:
			case StoreObjectType.Note:
			case StoreObjectType.Post:
			case StoreObjectType.Report:
			case StoreObjectType.MeetingForwardNotification:
				break;
			default:
				if (storeObjectType != StoreObjectType.Mailbox)
				{
					return false;
				}
				break;
			}
			return true;
		}
	}
}
