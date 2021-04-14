using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class CalendarNotificationSettingsService : DataSourceService, ICalendarNotificationSettingsService, IEditObjectService<CalendarNotificationSettings, SetCalendarNotificationSettings>, IGetObjectService<CalendarNotificationSettings>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TextMessagingAccount?Identity@R:Self")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-CalendarNotification?Identity@R:Self")]
		public PowerShellResults<CalendarNotificationSettings> GetObject(Identity identity)
		{
			identity = Identity.FromExecutingUserId();
			PowerShellResults<CalendarNotificationSettings> @object = base.GetObject<CalendarNotificationSettings>("Get-CalendarNotification", identity);
			if (!@object.Failed && @object.HasValue)
			{
				PowerShellResults<SmsOptions> powerShellResults = @object.MergeErrors<SmsOptions>(base.GetObject<SmsOptions>("Get-TextMessagingAccount", identity));
				if (powerShellResults.HasValue)
				{
					@object.Value.NotificationEnabled = powerShellResults.Value.NotificationEnabled;
					@object.Value.EasEnabled = powerShellResults.Value.EasEnabled;
				}
			}
			return @object;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-CalendarNotification?Identity@R:Self+Set-CalendarNotification?Identity@W:Self")]
		public PowerShellResults<CalendarNotificationSettings> SetObject(Identity identity, SetCalendarNotificationSettings properties)
		{
			identity = Identity.FromExecutingUserId();
			return base.SetObject<CalendarNotificationSettings, SetCalendarNotificationSettings>("Set-CalendarNotification", identity, properties);
		}

		internal const string GetCmdlet = "Get-CalendarNotification";

		internal const string SetCmdlet = "Set-CalendarNotification";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		private const string GetObjectRole = "Get-CalendarNotification?Identity@R:Self";

		private const string SetObjectRole = "Get-CalendarNotification?Identity@R:Self+Set-CalendarNotification?Identity@W:Self";
	}
}
