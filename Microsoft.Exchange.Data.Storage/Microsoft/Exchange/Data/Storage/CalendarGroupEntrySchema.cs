using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarGroupEntrySchema : FolderTreeDataSchema
	{
		public new static CalendarGroupEntrySchema Instance
		{
			get
			{
				if (CalendarGroupEntrySchema.instance == null)
				{
					CalendarGroupEntrySchema.instance = new CalendarGroupEntrySchema();
				}
				return CalendarGroupEntrySchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition StoreEntryId = InternalSchema.NavigationNodeStoreEntryId;

		[Autoload]
		public static readonly StorePropertyDefinition NodeRecordKey = InternalSchema.NavigationNodeRecordKey;

		[Autoload]
		public static readonly StorePropertyDefinition CalendarColor = InternalSchema.NavigationNodeCalendarColor;

		[Autoload]
		public static readonly StorePropertyDefinition CalendarName = InternalSchema.Subject;

		[Autoload]
		public static readonly StorePropertyDefinition NodeEntryId = InternalSchema.NavigationNodeEntryId;

		[Autoload]
		public static readonly StorePropertyDefinition ParentGroupName = InternalSchema.NavigationNodeGroupName;

		[Autoload]
		public static readonly StorePropertyDefinition SharerAddressBookEntryId = InternalSchema.NavigationNodeAddressBookEntryId;

		[Autoload]
		public static readonly StorePropertyDefinition UserAddressBookStoreEntryId = InternalSchema.NavigationNodeAddressBookStoreEntryId;

		private static CalendarGroupEntrySchema instance;
	}
}
