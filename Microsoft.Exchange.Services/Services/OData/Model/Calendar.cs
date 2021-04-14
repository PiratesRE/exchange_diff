using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class Calendar : Entity
	{
		public string Name
		{
			get
			{
				return (string)base[CalendarSchema.Name];
			}
			set
			{
				base[CalendarSchema.Name] = value;
			}
		}

		public string ChangeKey
		{
			get
			{
				return (string)base[CalendarSchema.ChangeKey];
			}
			set
			{
				base[CalendarSchema.ChangeKey] = value;
			}
		}

		public IEnumerable<Event> Events
		{
			get
			{
				return (IEnumerable<Event>)base[CalendarSchema.Events];
			}
			set
			{
				base[CalendarSchema.Events] = value;
			}
		}

		internal override EntitySchema Schema
		{
			get
			{
				return CalendarSchema.SchemaInstance;
			}
		}

		internal new static readonly EdmEntityType EdmEntityType = new EdmEntityType(typeof(Calendar).Namespace, typeof(Calendar).Name, Entity.EdmEntityType);
	}
}
