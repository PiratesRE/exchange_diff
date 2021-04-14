using System;
using Microsoft.Exchange.Data.Storage.ReliableActions;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.Interop
{
	internal interface ISeriesActionParser
	{
		ICalendarInteropSeriesAction DeserializeCommand(ActionInfo action, Event contextEntity);
	}
}
