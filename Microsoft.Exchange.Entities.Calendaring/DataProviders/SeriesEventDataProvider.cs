using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.Calendaring.DataProviders
{
	internal class SeriesEventDataProvider : EventDataProvider
	{
		public SeriesEventDataProvider(IStorageEntitySetScope<IStoreSession> scope, StoreId calendarFolderId) : base(scope, calendarFolderId)
		{
		}

		protected override ICalendarItemBase CreateNewStoreObject()
		{
			return base.XsoFactory.CreateCalendarItemSeries(base.Session, base.ContainerFolderId);
		}
	}
}
