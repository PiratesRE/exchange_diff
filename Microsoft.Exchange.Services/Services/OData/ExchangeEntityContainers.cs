using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Services.OData
{
	internal class ExchangeEntityContainers : IExchangeEntityContainers
	{
		public ExchangeEntityContainers(StoreSession storeSession)
		{
			this.Calendaring = new CalendaringContainer(storeSession, null);
		}

		public ICalendaringContainer Calendaring { get; private set; }
	}
}
