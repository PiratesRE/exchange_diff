using System;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class CalendarVDirApplication : OwaApplicationBase
	{
		internal override OWAVDirType OwaVDirType
		{
			get
			{
				return OWAVDirType.Calendar;
			}
		}

		protected override void ExecuteApplicationSpecificStart()
		{
			ExWatson.Register("E12IIS");
			OwaEventRegistry.RegisterHandler(typeof(ProxyEventHandler));
			CalendarViewEventHandler.Register();
			DatePickerEventHandler.Register();
			PrintCalendarEventHandler.Register();
		}
	}
}
