using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.BirthdayCalendar
{
	public sealed class BirthdayCalendar : StorageEntity<BirthdayCalendarSchema>, IBirthdayCalendar, IStorageEntity, IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned
	{
	}
}
