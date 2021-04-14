using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class FolderTypeMap
	{
		static FolderTypeMap()
		{
			FolderTypeMap.displayNameToStoreObjectTypeMap.Add("Folder", StoreObjectType.Folder);
			FolderTypeMap.displayNameToStoreObjectTypeMap.Add("CalendarFolder", StoreObjectType.CalendarFolder);
			FolderTypeMap.displayNameToStoreObjectTypeMap.Add("ContactsFolder", StoreObjectType.ContactsFolder);
			FolderTypeMap.displayNameToStoreObjectTypeMap.Add("SearchFolder", StoreObjectType.SearchFolder);
			FolderTypeMap.displayNameToStoreObjectTypeMap.Add("TasksFolder", StoreObjectType.TasksFolder);
		}

		public static StoreObjectType FolderTypeToStoreObjectType(string folderElementName)
		{
			StoreObjectType result = StoreObjectType.Unknown;
			if (!FolderTypeMap.displayNameToStoreObjectTypeMap.TryGetValue(folderElementName, out result))
			{
				return StoreObjectType.Folder;
			}
			return result;
		}

		private static Dictionary<string, StoreObjectType> displayNameToStoreObjectTypeMap = new Dictionary<string, StoreObjectType>();
	}
}
