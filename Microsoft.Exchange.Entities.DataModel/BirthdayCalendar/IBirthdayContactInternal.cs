using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.BirthdayCalendar
{
	internal interface IBirthdayContactInternal : IBirthdayContact, IStorageEntity, IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned
	{
		PersonId PersonId { get; set; }

		StoreId StoreId { get; set; }
	}
}
