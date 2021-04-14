using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.BirthdayCalendar
{
	public interface IBirthdayEvent : IStorageEntity, IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned
	{
		string Attribution { get; set; }

		ExDateTime Birthday { get; set; }

		string Subject { get; set; }

		bool IsBirthYearKnown { get; }

		bool IsWritable { get; set; }
	}
}
