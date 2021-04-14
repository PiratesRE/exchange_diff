using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.BirthdayCalendar
{
	internal interface IBirthdayEventInternal : IBirthdayEvent, IStorageEntity, IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned
	{
		StoreObjectId ContactId { get; set; }

		PersonId PersonId { get; set; }

		StoreId StoreId { get; set; }
	}
}
