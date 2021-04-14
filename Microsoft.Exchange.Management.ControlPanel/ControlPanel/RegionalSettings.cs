using System;
using System.Globalization;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using System.Web;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class RegionalSettings : DataSourceService, IRegionalSettings, IEditObjectService<RegionalSettingsConfiguration, SetRegionalSettingsConfiguration>, IGetObjectService<RegionalSettingsConfiguration>
	{
		internal PowerShellResults<RegionalSettingsConfiguration> GetSettings(Identity identity, bool verifyFolderNameLanguage)
		{
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand("Get-MailboxRegionalConfiguration");
			if (verifyFolderNameLanguage && RbacPrincipal.Current.IsInRole("Get-MailboxRegionalConfiguration?Identity&VerifyDefaultFolderNameLanguage@R:Self"))
			{
				pscommand.AddParameter("VerifyDefaultFolderNameLanguage");
			}
			return base.GetObject<RegionalSettingsConfiguration>(pscommand, identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxRegionalConfiguration?Identity@R:Self+Get-MailboxCalendarConfiguration?Identity@R:Self")]
		public PowerShellResults<RegionalSettingsConfiguration> GetObject(Identity identity)
		{
			identity = Identity.FromExecutingUserId();
			PowerShellResults<RegionalSettingsConfiguration> settings = this.GetSettings(identity, true);
			if (settings.SucceededWithValue)
			{
				if (settings.Value.UserCulture == null)
				{
					settings.Value.MailboxRegionalConfiguration.Language = Culture.GetDefaultCulture(HttpContext.Current);
				}
				PowerShellResults<MailboxCalendarConfiguration> powerShellResults = settings.MergeErrors<MailboxCalendarConfiguration>(base.GetObject<MailboxCalendarConfiguration>("Get-MailboxCalendarConfiguration", identity));
				if (powerShellResults.SucceededWithValue)
				{
					settings.Value.MailboxCalendarConfiguration = powerShellResults.Value;
				}
			}
			return settings;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxRegionalConfiguration?Identity@R:Self+Get-MailboxCalendarConfiguration?Identity@R:Self+Set-MailboxRegionalConfiguration?Identity@W:Self")]
		public PowerShellResults<RegionalSettingsConfiguration> SetObject(Identity identity, SetRegionalSettingsConfiguration properties)
		{
			properties.FaultIfNull();
			identity = Identity.FromExecutingUserId();
			PowerShellResults<RegionalSettingsConfiguration> powerShellResults = null;
			bool flag = CultureInfo.CurrentCulture.LCID != properties.Language;
			properties.ReturnObjectType = ReturnObjectTypes.Full;
			lock (RbacPrincipal.Current.OwaOptionsLock)
			{
				powerShellResults = base.SetObject<RegionalSettingsConfiguration, SetRegionalSettingsConfiguration>("Set-MailboxRegionalConfiguration", identity, properties);
			}
			if (powerShellResults != null && powerShellResults.SucceededWithValue)
			{
				LocalSession.Current.AddRegionalSettingsToCache(powerShellResults.Value);
				if (HttpContext.Current.IsExplicitSignOn())
				{
					LocalSession.Current.UpdateUserTimeZone(powerShellResults.Value.TimeZone);
				}
				else
				{
					LocalSession.Current.UpdateRegionalSettings(powerShellResults.Value);
					if (flag)
					{
						LocalSession.Current.RbacConfiguration.ExecutingUserLanguagesChanged = true;
						HttpContext.Current.Response.Cookies.Add(new HttpCookie("mkt", powerShellResults.Value.UserCulture.Name)
						{
							HttpOnly = false
						});
					}
				}
				UserSettings userSettings = flag ? UserSettings.RegionAndLanguage : UserSettings.RegionWithoutLanguage;
				Util.NotifyOWAUserSettingsChanged(userSettings);
			}
			return powerShellResults;
		}

		internal const string GetCmdlet = "Get-MailboxRegionalConfiguration";

		internal const string SetCmdlet = "Set-MailboxRegionalConfiguration";

		internal const string GetCalendarCmdlet = "Get-MailboxCalendarConfiguration";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		internal const string GetSettingsRole = "Get-MailboxRegionalConfiguration?Identity@R:Self";

		private const string GetSettingsAndVerifyFolderLanguageRole = "Get-MailboxRegionalConfiguration?Identity&VerifyDefaultFolderNameLanguage@R:Self";

		private const string VerifyFolderLanguageParam = "VerifyDefaultFolderNameLanguage";

		private const string GetObjectRole = "Get-MailboxRegionalConfiguration?Identity@R:Self+Get-MailboxCalendarConfiguration?Identity@R:Self";

		private const string SetObjectRole = "Get-MailboxRegionalConfiguration?Identity@R:Self+Get-MailboxCalendarConfiguration?Identity@R:Self+Set-MailboxRegionalConfiguration?Identity@W:Self";
	}
}
