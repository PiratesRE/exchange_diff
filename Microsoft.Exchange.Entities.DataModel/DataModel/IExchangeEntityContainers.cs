using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.DataModel
{
	public interface IExchangeEntityContainers
	{
		ICalendaringContainer Calendaring { get; }
	}
}
