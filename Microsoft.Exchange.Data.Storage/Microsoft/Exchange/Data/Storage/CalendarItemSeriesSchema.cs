using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarItemSeriesSchema : CalendarItemBaseSchema
	{
		public new static CalendarItemSeriesSchema Instance
		{
			get
			{
				if (CalendarItemSeriesSchema.instance == null)
				{
					CalendarItemSeriesSchema.instance = new CalendarItemSeriesSchema();
				}
				return CalendarItemSeriesSchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition CalendarInteropActionQueue = InternalSchema.CalendarInteropActionQueue;

		[Autoload]
		public static readonly StorePropertyDefinition CalendarInteropActionQueueHasData = InternalSchema.CalendarInteropActionQueueHasData;

		[Autoload]
		public static readonly StorePropertyDefinition SeriesCreationHash = InternalSchema.SeriesCreationHash;

		[Autoload]
		public static readonly StorePropertyDefinition SeriesReminderIsSet = InternalSchema.SeriesReminderIsSet;

		public static readonly StorePropertyDefinition CalendarInteropActionQueueHasDataInternal = InternalSchema.CalendarInteropActionQueueHasDataInternal;

		public static readonly StorePropertyDefinition CalendarInteropActionQueueInternal = InternalSchema.CalendarInteropActionQueueInternal;

		private static CalendarItemSeriesSchema instance = null;
	}
}
