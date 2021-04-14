using System;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class MessagingBase : DataSourceService
	{
		public PowerShellResults<O> GetObject<O>(Identity identity) where O : MessagingConfigurationBase
		{
			identity = Identity.FromExecutingUserId();
			return base.GetObject<O>("Get-MailboxMessageConfiguration", identity);
		}

		public PowerShellResults<O> SetObject<O, U>(Identity identity, U properties) where O : MessagingConfigurationBase where U : SetMessagingConfigurationBase
		{
			identity = Identity.FromExecutingUserId();
			PowerShellResults<O> powerShellResults;
			lock (RbacPrincipal.Current.OwaOptionsLock)
			{
				powerShellResults = base.SetObject<O, U>("Set-MailboxMessageConfiguration", identity, properties);
				if (powerShellResults != null && powerShellResults.Succeeded)
				{
					Util.NotifyOWAUserSettingsChanged(UserSettings.Mail);
				}
			}
			return powerShellResults;
		}

		internal const string GetCmdlet = "Get-MailboxMessageConfiguration";

		internal const string SetCmdlet = "Set-MailboxMessageConfiguration";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		internal const string GetObjectRole = "Get-MailboxMessageConfiguration?Identity@R:Self";

		internal const string SetObjectRole = "Get-MailboxMessageConfiguration?Identity@R:Self+Set-MailboxMessageConfiguration?Identity@W:Self";
	}
}
