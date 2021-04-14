using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NavigationNodeSchema : StoreObjectSchema
	{
		public new static NavigationNodeSchema Instance
		{
			get
			{
				if (NavigationNodeSchema.instance == null)
				{
					NavigationNodeSchema.instance = new NavigationNodeSchema();
				}
				return NavigationNodeSchema.instance;
			}
		}

		public static readonly StorePropertyDefinition GroupClassId = InternalSchema.NavigationNodeGroupClassId;

		public static readonly StorePropertyDefinition StoreEntryId = InternalSchema.NavigationNodeStoreEntryId;

		public static readonly StorePropertyDefinition NodeRecordKey = InternalSchema.NavigationNodeRecordKey;

		public static readonly StorePropertyDefinition ParentGroupClassId = InternalSchema.NavigationNodeParentGroupClassId;

		public static readonly StorePropertyDefinition GroupName = InternalSchema.NavigationNodeGroupName;

		public static readonly StorePropertyDefinition CalendarColor = InternalSchema.NavigationNodeCalendarColor;

		public static readonly StorePropertyDefinition NodeEntryId = InternalSchema.NavigationNodeEntryId;

		public static readonly StorePropertyDefinition Type = InternalSchema.NavigationNodeType;

		public static readonly StorePropertyDefinition OutlookTagId = InternalSchema.NavigationNodeOutlookTagId;

		public static readonly StorePropertyDefinition Flags = InternalSchema.NavigationNodeFlags;

		public static readonly StorePropertyDefinition Ordinal = InternalSchema.NavigationNodeOrdinal;

		public static readonly StorePropertyDefinition ClassId = InternalSchema.NavigationNodeClassId;

		public static readonly StorePropertyDefinition GroupSection = InternalSchema.NavigationNodeGroupSection;

		public static readonly StorePropertyDefinition AddressBookEntryId = InternalSchema.NavigationNodeAddressBookEntryId;

		public static readonly StorePropertyDefinition AddressBookStoreEntryId = InternalSchema.NavigationNodeAddressBookStoreEntryId;

		public static readonly StorePropertyDefinition CalendarTypeFromOlderExchange = InternalSchema.NavigationNodeCalendarTypeFromOlderExchange;

		private static NavigationNodeSchema instance = null;
	}
}
