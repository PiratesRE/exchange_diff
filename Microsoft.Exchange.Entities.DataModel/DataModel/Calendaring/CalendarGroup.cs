using System;
using System.Collections.Generic;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public sealed class CalendarGroup : StorageEntity<CalendarGroupSchema>, ICalendarGroup, IStorageEntity, IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned
	{
		public IEnumerable<Calendar> Calendars { get; set; }

		public Guid ClassId
		{
			get
			{
				return base.GetPropertyValueOrDefault<Guid>(base.Schema.ClassIdProperty);
			}
			set
			{
				base.SetPropertyValue<Guid>(base.Schema.ClassIdProperty, value);
			}
		}

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

		public new static class Accessors
		{
			public static readonly EntityPropertyAccessor<CalendarGroup, Guid> ClassId = new EntityPropertyAccessor<CalendarGroup, Guid>(SchematizedObject<CalendarGroupSchema>.SchemaInstance.ClassIdProperty, (CalendarGroup calendarGroup) => calendarGroup.ClassId, delegate(CalendarGroup calendarGroup, Guid classId)
			{
				calendarGroup.ClassId = classId;
			});

			public static readonly EntityPropertyAccessor<CalendarGroup, string> Name = new EntityPropertyAccessor<CalendarGroup, string>(SchematizedObject<CalendarGroupSchema>.SchemaInstance.NameProperty, (CalendarGroup calendarGroup) => calendarGroup.Name, delegate(CalendarGroup calendarGroup, string name)
			{
				calendarGroup.Name = name;
			});
		}
	}
}
