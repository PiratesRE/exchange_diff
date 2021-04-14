using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FolderCreateInfo
	{
		private FolderCreateInfo(StoreObjectType folderType, string containerClass, FolderSchema schema, FolderCreateInfo.FolderCreator creator)
		{
			this.FolderType = folderType;
			this.ContainerClass = containerClass;
			this.Schema = schema;
			this.Creator = creator;
		}

		private static SearchFolder SearchFolderCreator(CoreFolder coreFolder)
		{
			return new SearchFolder(coreFolder);
		}

		private static OutlookSearchFolder OutlookSearchFolderCreator(CoreFolder coreFolder)
		{
			return new OutlookSearchFolder(coreFolder);
		}

		private static Folder GenericFolderCreator(CoreFolder coreFolder)
		{
			return new Folder(coreFolder);
		}

		private static ContactsFolder ContactsFolderCreator(CoreFolder coreFolder)
		{
			return new ContactsFolder(coreFolder);
		}

		private static CalendarFolder CalendarFolderCreator(CoreFolder coreFolder)
		{
			return new CalendarFolder(coreFolder);
		}

		internal static FolderCreateInfo GetFolderCreateInfo(StoreObjectType folderType)
		{
			return FolderCreateInfo.folderCreateInfoDictionary[folderType];
		}

		private static Dictionary<StoreObjectType, FolderCreateInfo> CreateFolderCreateInfoDictionary()
		{
			FolderCreateInfo[] array = new FolderCreateInfo[]
			{
				FolderCreateInfo.SearchFolderInfo,
				FolderCreateInfo.OutlookSearchFolderInfo,
				FolderCreateInfo.CalendarFolderInfo,
				FolderCreateInfo.ContactsFolderInfo,
				FolderCreateInfo.TasksFolderInfo,
				FolderCreateInfo.JournalFolderInfo,
				FolderCreateInfo.NotesFolderInfo,
				FolderCreateInfo.ShortcutFolderInfo,
				FolderCreateInfo.GenericFolderInfo
			};
			Dictionary<StoreObjectType, FolderCreateInfo> dictionary = new Dictionary<StoreObjectType, FolderCreateInfo>(new StoreObjectTypeComparer());
			foreach (FolderCreateInfo folderCreateInfo in array)
			{
				dictionary.Add(folderCreateInfo.FolderType, folderCreateInfo);
			}
			return dictionary;
		}

		internal readonly string ContainerClass;

		internal readonly FolderSchema Schema;

		internal readonly FolderCreateInfo.FolderCreator Creator;

		internal readonly StoreObjectType FolderType;

		private static readonly FolderCreateInfo OutlookSearchFolderInfo = new FolderCreateInfo(StoreObjectType.OutlookSearchFolder, "IPF.Note", FolderSchema.Instance, new FolderCreateInfo.FolderCreator(FolderCreateInfo.OutlookSearchFolderCreator));

		private static readonly FolderCreateInfo CalendarFolderInfo = new FolderCreateInfo(StoreObjectType.CalendarFolder, "IPF.Appointment", CalendarFolderSchema.Instance, new FolderCreateInfo.FolderCreator(FolderCreateInfo.CalendarFolderCreator));

		private static readonly FolderCreateInfo ContactsFolderInfo = new FolderCreateInfo(StoreObjectType.ContactsFolder, "IPF.Contact", FolderSchema.Instance, new FolderCreateInfo.FolderCreator(FolderCreateInfo.ContactsFolderCreator));

		private static readonly FolderCreateInfo TasksFolderInfo = new FolderCreateInfo(StoreObjectType.TasksFolder, "IPF.Task", FolderSchema.Instance, new FolderCreateInfo.FolderCreator(FolderCreateInfo.GenericFolderCreator));

		private static readonly FolderCreateInfo JournalFolderInfo = new FolderCreateInfo(StoreObjectType.JournalFolder, "IPF.Journal", FolderSchema.Instance, new FolderCreateInfo.FolderCreator(FolderCreateInfo.GenericFolderCreator));

		private static readonly FolderCreateInfo NotesFolderInfo = new FolderCreateInfo(StoreObjectType.NotesFolder, "IPF.StickyNote", FolderSchema.Instance, new FolderCreateInfo.FolderCreator(FolderCreateInfo.GenericFolderCreator));

		private static readonly FolderCreateInfo ShortcutFolderInfo = new FolderCreateInfo(StoreObjectType.ShortcutFolder, "IPF.ShortcutFolder", FolderSchema.Instance, new FolderCreateInfo.FolderCreator(FolderCreateInfo.GenericFolderCreator));

		internal static readonly FolderCreateInfo SearchFolderInfo = new FolderCreateInfo(StoreObjectType.SearchFolder, "IPF.Note", FolderSchema.Instance, new FolderCreateInfo.FolderCreator(FolderCreateInfo.SearchFolderCreator));

		internal static readonly FolderCreateInfo GenericFolderInfo = new FolderCreateInfo(StoreObjectType.Folder, null, FolderSchema.Instance, new FolderCreateInfo.FolderCreator(FolderCreateInfo.GenericFolderCreator));

		private static readonly Dictionary<StoreObjectType, FolderCreateInfo> folderCreateInfoDictionary = FolderCreateInfo.CreateFolderCreateInfoDictionary();

		internal delegate Folder FolderCreator(CoreFolder coreFolder);
	}
}
