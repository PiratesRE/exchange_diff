using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "CalendarNotificationSettings")]
	public interface ICalendarNotificationSettingsService : IEditObjectService<CalendarNotificationSettings, SetCalendarNotificationSettings>, IGetObjectService<CalendarNotificationSettings>
	{
	}
}
