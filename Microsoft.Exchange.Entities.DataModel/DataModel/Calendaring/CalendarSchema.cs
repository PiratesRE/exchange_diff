using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public sealed class CalendarSchema : StorageEntitySchema
	{
		public CalendarSchema()
		{
			base.RegisterPropertyDefinition(CalendarSchema.StaticColorProperty);
			base.RegisterPropertyDefinition(CalendarSchema.StaticNameProperty);
			base.RegisterPropertyDefinition(CalendarSchema.StaticCalendarFolderStoreIdProperty);
			base.RegisterPropertyDefinition(CalendarSchema.StaticRecordKeyProperty);
		}

		public TypedPropertyDefinition<CalendarColor> ColorProperty
		{
			get
			{
				return CalendarSchema.StaticColorProperty;
			}
		}

		public TypedPropertyDefinition<string> NameProperty
		{
			get
			{
				return CalendarSchema.StaticNameProperty;
			}
		}

		internal TypedPropertyDefinition<StoreId> CalendarFolderStoreIdProperty
		{
			get
			{
				return CalendarSchema.StaticCalendarFolderStoreIdProperty;
			}
		}

		internal TypedPropertyDefinition<byte[]> RecordKeyProperty
		{
			get
			{
				return CalendarSchema.StaticRecordKeyProperty;
			}
		}

		private static readonly TypedPropertyDefinition<StoreId> StaticCalendarFolderStoreIdProperty = new TypedPropertyDefinition<StoreId>("Calendar.InternalCalendarFolderStoreId", null, true);

		private static readonly TypedPropertyDefinition<CalendarColor> StaticColorProperty = new TypedPropertyDefinition<CalendarColor>("Calendar.Color", CalendarColor.Auto, true);

		private static readonly TypedPropertyDefinition<string> StaticNameProperty = new TypedPropertyDefinition<string>("Calendar.Name", null, true);

		private static readonly TypedPropertyDefinition<byte[]> StaticRecordKeyProperty = new TypedPropertyDefinition<byte[]>("Calendar.RecordKey", null, true);
	}
}
