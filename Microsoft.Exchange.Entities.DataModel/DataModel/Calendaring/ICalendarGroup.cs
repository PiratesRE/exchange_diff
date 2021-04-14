using System;
using System.Collections.Generic;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public interface ICalendarGroup : IStorageEntity, IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned
	{
		IEnumerable<Calendar> Calendars { get; set; }

		Guid ClassId { get; set; }

		string Name { get; set; }
	}
}
