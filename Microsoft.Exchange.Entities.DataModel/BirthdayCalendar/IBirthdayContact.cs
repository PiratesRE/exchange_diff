using System;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.BirthdayCalendar
{
	public interface IBirthdayContact : IStorageEntity, IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned
	{
		ExDateTime? Birthday { get; set; }

		string DisplayName { get; set; }

		string Attribution { get; set; }

		bool ShouldHideBirthday { get; set; }

		bool IsWritable { get; set; }
	}
}
