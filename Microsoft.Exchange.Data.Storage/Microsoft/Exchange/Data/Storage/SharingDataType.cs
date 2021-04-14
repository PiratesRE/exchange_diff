using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SharingDataType
	{
		public string ContainerClass { get; private set; }

		public string ExternalName { get; private set; }

		public bool IsExternallySharable { get; private set; }

		public string PublishName { get; private set; }

		public string PublishResourceName { get; private set; }

		public LocalizedString DisplayName { get; private set; }

		internal DefaultFolderType DefaultFolderType { get; private set; }

		internal StoreObjectType StoreObjectType { get; private set; }

		public static SharingDataType FromContainerClass(string containerClass)
		{
			if (!string.IsNullOrEmpty(containerClass))
			{
				foreach (SharingDataType sharingDataType in SharingDataType.dataTypes)
				{
					if (ObjectClass.IsOfClass(containerClass, sharingDataType.ContainerClass))
					{
						return sharingDataType;
					}
				}
			}
			return null;
		}

		public static SharingDataType FromExternalName(string externalName)
		{
			if (!string.IsNullOrEmpty(externalName))
			{
				foreach (SharingDataType sharingDataType in SharingDataType.dataTypes)
				{
					if (StringComparer.InvariantCultureIgnoreCase.Equals(externalName, sharingDataType.ExternalName))
					{
						return sharingDataType;
					}
				}
			}
			return null;
		}

		public static SharingDataType FromPublishName(string publishName)
		{
			if (!string.IsNullOrEmpty(publishName))
			{
				foreach (SharingDataType sharingDataType in SharingDataType.dataTypes)
				{
					if (StringComparer.InvariantCultureIgnoreCase.Equals(publishName, sharingDataType.PublishName))
					{
						return sharingDataType;
					}
				}
			}
			return null;
		}

		public static SharingDataType FromPublishResourceName(string publishResourceName)
		{
			if (!string.IsNullOrEmpty(publishResourceName))
			{
				foreach (SharingDataType sharingDataType in SharingDataType.publishDataTypes)
				{
					if (StringComparer.InvariantCultureIgnoreCase.Equals(publishResourceName, sharingDataType.PublishResourceName))
					{
						return sharingDataType;
					}
				}
			}
			return null;
		}

		public override string ToString()
		{
			return this.ExternalName ?? this.ContainerClass;
		}

		private SharingDataType(string containerClass, string externalName, bool isExternallySharable, string publishName, string publishResourceName, LocalizedString displayName, DefaultFolderType defaultFolderType, StoreObjectType storeObjectType)
		{
			Util.ThrowOnNullOrEmptyArgument(containerClass, "containerClass");
			Util.ThrowOnNullOrEmptyArgument(externalName, "externalName");
			this.PublishResourceName = publishResourceName;
			this.ContainerClass = containerClass;
			this.ExternalName = externalName;
			this.IsExternallySharable = isExternallySharable;
			this.PublishName = publishName;
			this.DisplayName = displayName;
			this.DefaultFolderType = defaultFolderType;
			this.StoreObjectType = storeObjectType;
		}

		public static readonly SharingDataType Calendar = new SharingDataType("IPF.Appointment", "calendar", true, "text/calendar", "calendar", ClientStrings.Calendar, DefaultFolderType.Calendar, StoreObjectType.CalendarFolder);

		public static readonly SharingDataType ReachCalendar = new SharingDataType("IPF.Appointment", "calendar", true, "text/calendar", "reachcalendar", ClientStrings.Calendar, DefaultFolderType.Calendar, StoreObjectType.CalendarFolder);

		public static readonly SharingDataType Contacts = new SharingDataType("IPF.Contact", "contacts", true, null, null, ClientStrings.Contacts, DefaultFolderType.Contacts, StoreObjectType.ContactsFolder);

		public static readonly SharingDataType Tasks = new SharingDataType("IPF.Task", "tasks", false, null, null, ClientStrings.Tasks, DefaultFolderType.Tasks, StoreObjectType.TasksFolder);

		public static readonly SharingDataType Journal = new SharingDataType("IPF.Journal", "journals", false, null, null, ClientStrings.Journal, DefaultFolderType.Journal, StoreObjectType.JournalFolder);

		public static readonly SharingDataType Notes = new SharingDataType("IPF.StickyNote", "notes", false, null, null, ClientStrings.Notes, DefaultFolderType.Notes, StoreObjectType.NotesFolder);

		private static SharingDataType[] dataTypes = new SharingDataType[]
		{
			SharingDataType.Calendar,
			SharingDataType.Contacts,
			SharingDataType.Tasks,
			SharingDataType.Journal,
			SharingDataType.Notes
		};

		private static SharingDataType[] publishDataTypes = new SharingDataType[]
		{
			SharingDataType.Calendar,
			SharingDataType.ReachCalendar
		};
	}
}
