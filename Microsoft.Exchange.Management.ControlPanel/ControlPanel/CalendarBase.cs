using System;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class CalendarBase : DataSourceService
	{
		public PowerShellResults<O> GetObject<O>(Identity identity) where O : CalendarConfigurationBase
		{
			identity = Identity.FromExecutingUserId();
			return base.GetObject<O>("Get-MailboxCalendarConfiguration", identity);
		}

		public PowerShellResults<O> SetObject<O, U>(Identity identity, U properties) where O : CalendarConfigurationBase where U : SetCalendarConfigurationBase
		{
			identity = Identity.FromExecutingUserId();
			PowerShellResults<O> powerShellResults;
			lock (RbacPrincipal.Current.OwaOptionsLock)
			{
				powerShellResults = base.SetObject<O, U>("Set-MailboxCalendarConfiguration", identity, properties);
				if (powerShellResults != null && powerShellResults.Succeeded)
				{
					Util.NotifyOWAUserSettingsChanged(UserSettings.Calendar);
				}
			}
			return powerShellResults;
		}

		internal const string GetCmdlet = "Get-MailboxCalendarConfiguration";

		internal const string SetCmdlet = "Set-MailboxCalendarConfiguration";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		internal const string GetObjectRole = "Get-MailboxCalendarConfiguration?Identity@R:Self";

		internal const string SetObjectRole = "Get-MailboxCalendarConfiguration?Identity@R:Self+Set-MailboxCalendarConfiguration?Identity@W:Self";
	}
}
