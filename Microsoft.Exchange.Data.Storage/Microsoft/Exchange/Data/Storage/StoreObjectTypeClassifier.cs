using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class StoreObjectTypeClassifier
	{
		public static bool IsFolderObjectType(StoreObjectType storeObjectType)
		{
			switch (storeObjectType)
			{
			case StoreObjectType.Folder:
			case StoreObjectType.CalendarFolder:
			case StoreObjectType.ContactsFolder:
			case StoreObjectType.TasksFolder:
			case StoreObjectType.NotesFolder:
			case StoreObjectType.JournalFolder:
			case StoreObjectType.SearchFolder:
			case StoreObjectType.OutlookSearchFolder:
				break;
			default:
				if (storeObjectType != StoreObjectType.ShortcutFolder)
				{
					return false;
				}
				break;
			}
			return true;
		}

		public static bool AlwaysReportRealType(StoreObjectType storeObjectType)
		{
			switch (storeObjectType)
			{
			case StoreObjectType.OofMessage:
			case StoreObjectType.ExternalOofMessage:
				break;
			default:
				if (storeObjectType != StoreObjectType.CalendarItemSeries)
				{
					return false;
				}
				break;
			}
			return true;
		}
	}
}
