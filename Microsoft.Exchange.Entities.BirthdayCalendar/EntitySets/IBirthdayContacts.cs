using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.BirthdayCalendar.DataProviders;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets
{
	internal interface IBirthdayContacts : IEntitySet<IBirthdayContact>, IStorageEntitySetScope<IStoreSession>
	{
		BirthdayContactDataProvider BirthdayContactDataProvider { get; set; }

		IEnumerable<IBirthdayContact> GetLinkedContacts(PersonId personId);
	}
}
