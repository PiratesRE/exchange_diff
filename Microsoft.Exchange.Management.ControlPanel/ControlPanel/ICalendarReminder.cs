using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "CalendarReminder")]
	public interface ICalendarReminder : ICalendarBase<CalendarReminderConfiguration, SetCalendarReminderConfiguration>, IEditObjectService<CalendarReminderConfiguration, SetCalendarReminderConfiguration>, IGetObjectService<CalendarReminderConfiguration>
	{
	}
}
