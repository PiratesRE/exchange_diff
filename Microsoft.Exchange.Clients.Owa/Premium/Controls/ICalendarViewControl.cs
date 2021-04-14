using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal interface ICalendarViewControl
	{
		OwaStoreObjectId SelectedItemId { get; set; }

		int Count { get; }

		CalendarAdapterBase CalendarAdapter { get; }
	}
}
