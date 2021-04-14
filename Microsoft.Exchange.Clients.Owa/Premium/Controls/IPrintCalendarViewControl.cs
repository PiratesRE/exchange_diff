using System;
using System.IO;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public interface IPrintCalendarViewControl
	{
		void RenderView(TextWriter writer);

		string DateDescription { get; }

		string CalendarName { get; }

		ExDateTime[] GetEffectiveDates();
	}
}
