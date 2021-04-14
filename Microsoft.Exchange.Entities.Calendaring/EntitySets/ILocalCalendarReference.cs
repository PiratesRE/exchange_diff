using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets
{
	internal interface ILocalCalendarReference : ICalendarReference, IEntityReference<Calendar>
	{
		StoreId GetCalendarFolderId();
	}
}
