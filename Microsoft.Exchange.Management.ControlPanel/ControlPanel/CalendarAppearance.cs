using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class CalendarAppearance : CalendarBase, ICalendarAppearance, ICalendarBase<CalendarAppearanceConfiguration, SetCalendarAppearanceConfiguration>, IEditObjectService<CalendarAppearanceConfiguration, SetCalendarAppearanceConfiguration>, IGetObjectService<CalendarAppearanceConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxCalendarConfiguration?Identity@R:Self")]
		public PowerShellResults<CalendarAppearanceConfiguration> GetObject(Identity identity)
		{
			return base.GetObject<CalendarAppearanceConfiguration>(identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxCalendarConfiguration?Identity@R:Self+Set-MailboxCalendarConfiguration?Identity@W:Self")]
		public PowerShellResults<CalendarAppearanceConfiguration> SetObject(Identity identity, SetCalendarAppearanceConfiguration properties)
		{
			return base.SetObject<CalendarAppearanceConfiguration, SetCalendarAppearanceConfiguration>(identity, properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxCalendarConfiguration?Identity@R:Self+Set-MailboxCalendarConfiguration?Identity@W:Self")]
		public PowerShellResults<CalendarAppearanceConfiguration> UpdateObject(Identity identity)
		{
			identity = Identity.FromExecutingUserId();
			SetCalendarAppearanceConfiguration setCalendarAppearanceConfiguration = new SetCalendarAppearanceConfiguration();
			setCalendarAppearanceConfiguration.WorkingHoursTimeZone = ((RbacPrincipal.Current.UserTimeZone == null) ? ExTimeZone.CurrentTimeZone.Id : RbacPrincipal.Current.UserTimeZone.Id);
			PowerShellResults<CalendarAppearanceConfiguration> powerShellResults;
			lock (RbacPrincipal.Current.OwaOptionsLock)
			{
				powerShellResults = base.SetObject<CalendarAppearanceConfiguration, SetCalendarAppearanceConfiguration>("Set-MailboxCalendarConfiguration", identity, setCalendarAppearanceConfiguration);
			}
			if (powerShellResults != null && powerShellResults.Succeeded)
			{
				Util.NotifyOWAUserSettingsChanged(UserSettings.Calendar);
			}
			return powerShellResults;
		}
	}
}
