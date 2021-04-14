using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FreeBusyItemSchema : ItemSchema
	{
		public new static FreeBusyItemSchema Instance
		{
			get
			{
				if (FreeBusyItemSchema.instance == null)
				{
					FreeBusyItemSchema.instance = new FreeBusyItemSchema();
				}
				return FreeBusyItemSchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition FreeBusyEntryIds = InternalSchema.FreeBusyEntryIds;

		[Autoload]
		public static readonly StorePropertyDefinition ScheduleInfoMonthsTentative = InternalSchema.ScheduleInfoMonthsTentative;

		[Autoload]
		public static readonly StorePropertyDefinition ScheduleInfoFreeBusyTentative = InternalSchema.ScheduleInfoFreeBusyTentative;

		[Autoload]
		public static readonly StorePropertyDefinition ScheduleInfoMonthsBusy = InternalSchema.ScheduleInfoMonthsBusy;

		[Autoload]
		public static readonly StorePropertyDefinition ScheduleInfoFreeBusyBusy = InternalSchema.ScheduleInfoFreeBusyBusy;

		[Autoload]
		public static readonly StorePropertyDefinition ScheduleInfoMonthsOof = InternalSchema.ScheduleInfoMonthsOof;

		[Autoload]
		public static readonly StorePropertyDefinition ScheduleInfoFreeBusyOof = InternalSchema.ScheduleInfoFreeBusyOof;

		[Autoload]
		public static readonly StorePropertyDefinition ScheduleInfoMonthsMerged = InternalSchema.ScheduleInfoMonthsMerged;

		[Autoload]
		public static readonly StorePropertyDefinition ScheduleInfoFreeBusyMerged = InternalSchema.ScheduleInfoFreeBusyMerged;

		[Autoload]
		public static readonly StorePropertyDefinition ScheduleInfoRecipientLegacyDn = InternalSchema.ScheduleInfoRecipientLegacyDn;

		[Autoload]
		public static readonly StorePropertyDefinition OutlookFreeBusyMonthCount = InternalSchema.OutlookFreeBusyMonthCount;

		[Autoload]
		public static readonly StorePropertyDefinition PublicFolderFreeBusy = InternalSchema.PublicFolderFreeBusy;

		private static FreeBusyItemSchema instance = null;
	}
}
