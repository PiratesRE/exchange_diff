using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public sealed class Calendar : StorageEntity<CalendarSchema>, ICalendar, IStorageEntity, IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned
	{
		public CalendarColor Color
		{
			get
			{
				return base.GetPropertyValueOrDefault<CalendarColor>(base.Schema.ColorProperty);
			}
			set
			{
				base.SetPropertyValue<CalendarColor>(base.Schema.ColorProperty, value);
			}
		}

		public IEnumerable<Event> Events { get; set; }

		public string Name
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.NameProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.NameProperty, value);
			}
		}

		internal StoreId CalendarFolderStoreId
		{
			get
			{
				return base.GetPropertyValueOrDefault<StoreId>(base.Schema.CalendarFolderStoreIdProperty);
			}
			set
			{
				base.SetPropertyValue<StoreId>(base.Schema.CalendarFolderStoreIdProperty, value);
			}
		}

		internal byte[] RecordKey
		{
			get
			{
				return base.GetPropertyValueOrDefault<byte[]>(base.Schema.RecordKeyProperty);
			}
			set
			{
				base.SetPropertyValue<byte[]>(base.Schema.RecordKeyProperty, value);
			}
		}

		public new static class Accessors
		{
			public static readonly EntityPropertyAccessor<Calendar, CalendarColor> Color = new EntityPropertyAccessor<Calendar, CalendarColor>(SchematizedObject<CalendarSchema>.SchemaInstance.ColorProperty, (Calendar calendar) => calendar.Color, delegate(Calendar calendar, CalendarColor color)
			{
				calendar.Color = color;
			});

			public static readonly EntityPropertyAccessor<Calendar, string> Name = new EntityPropertyAccessor<Calendar, string>(SchematizedObject<CalendarSchema>.SchemaInstance.NameProperty, (Calendar calendar) => calendar.Name, delegate(Calendar calendar, string name)
			{
				calendar.Name = name;
			});

			internal static readonly EntityPropertyAccessor<Calendar, StoreId> CalendarFolderStoreId = new EntityPropertyAccessor<Calendar, StoreId>(SchematizedObject<CalendarSchema>.SchemaInstance.CalendarFolderStoreIdProperty, (Calendar calendar) => calendar.CalendarFolderStoreId, delegate(Calendar calendar, StoreId id)
			{
				calendar.CalendarFolderStoreId = id;
			});

			internal static readonly EntityPropertyAccessor<Calendar, byte[]> RecordKey = new EntityPropertyAccessor<Calendar, byte[]>(SchematizedObject<CalendarSchema>.SchemaInstance.RecordKeyProperty, (Calendar calendar) => calendar.RecordKey, delegate(Calendar calendar, byte[] recordKey)
			{
				calendar.RecordKey = recordKey;
			});
		}
	}
}
