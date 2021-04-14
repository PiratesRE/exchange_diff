using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CalendarGroup : Entity
	{
		public string Name
		{
			get
			{
				return (string)base[CalendarGroupSchema.Name];
			}
			set
			{
				base[CalendarGroupSchema.Name] = value;
			}
		}

		public string ChangeKey
		{
			get
			{
				return (string)base[CalendarGroupSchema.ChangeKey];
			}
			set
			{
				base[CalendarGroupSchema.ChangeKey] = value;
			}
		}

		public Guid ClassId
		{
			get
			{
				return (Guid)base[CalendarGroupSchema.ClassId];
			}
			set
			{
				base[CalendarGroupSchema.ClassId] = value;
			}
		}

		public IEnumerable<Calendar> Calendars
		{
			get
			{
				return (IEnumerable<Calendar>)base[CalendarGroupSchema.Calendars];
			}
			set
			{
				base[CalendarGroupSchema.Calendars] = value;
			}
		}

		internal override EntitySchema Schema
		{
			get
			{
				return CalendarGroupSchema.SchemaInstance;
			}
		}

		internal new static readonly EdmEntityType EdmEntityType = new EdmEntityType(typeof(CalendarGroup).Namespace, typeof(CalendarGroup).Name, Entity.EdmEntityType);
	}
}
