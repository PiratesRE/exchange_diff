using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class CalendarReminder : CalendarBase, ICalendarReminder, ICalendarBase<CalendarReminderConfiguration, SetCalendarReminderConfiguration>, IEditObjectService<CalendarReminderConfiguration, SetCalendarReminderConfiguration>, IGetObjectService<CalendarReminderConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxCalendarConfiguration?Identity@R:Self")]
		public PowerShellResults<CalendarReminderConfiguration> GetObject(Identity identity)
		{
			return base.GetObject<CalendarReminderConfiguration>(identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxCalendarConfiguration?Identity@R:Self+Set-MailboxCalendarConfiguration?Identity@W:Self")]
		public PowerShellResults<CalendarReminderConfiguration> SetObject(Identity identity, SetCalendarReminderConfiguration properties)
		{
			return base.SetObject<CalendarReminderConfiguration, SetCalendarReminderConfiguration>(identity, properties);
		}
	}
}
