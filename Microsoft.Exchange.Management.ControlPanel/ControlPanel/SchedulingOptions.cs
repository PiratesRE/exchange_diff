using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class SchedulingOptions : ResourceBase, ISchedulingOptions, IResourceBase<SchedulingOptionsConfiguration, SetSchedulingOptionsConfiguration>, IEditObjectService<SchedulingOptionsConfiguration, SetSchedulingOptionsConfiguration>, IGetObjectService<SchedulingOptionsConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Resource+Get-CalendarProcessing?Identity@R:Self+Get-MailboxCalendarConfiguration?Identity@R:Self")]
		public PowerShellResults<SchedulingOptionsConfiguration> GetObject(Identity identity)
		{
			PowerShellResults<SchedulingOptionsConfiguration> @object = base.GetObject<SchedulingOptionsConfiguration>(identity);
			if (@object.SucceededWithValue)
			{
				PowerShellResults<MailboxCalendarConfiguration> powerShellResults = @object.MergeErrors<MailboxCalendarConfiguration>(base.GetObject<MailboxCalendarConfiguration>("Get-MailboxCalendarConfiguration", Identity.FromExecutingUserId()));
				if (powerShellResults.SucceededWithValue)
				{
					@object.Value.MailboxCalendarConfiguration = powerShellResults.Value;
				}
			}
			return @object;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Resource+Get-CalendarProcessing?Identity@R:Self+Get-MailboxCalendarConfiguration?Identity@R:Self+Set-MailboxCalendarConfiguration?Identity@R:Self")]
		public PowerShellResults<SchedulingOptionsConfiguration> SetObject(Identity identity, SetSchedulingOptionsConfiguration properties)
		{
			properties.FaultIfNull();
			PowerShellResults<SchedulingOptionsConfiguration> powerShellResults = new PowerShellResults<SchedulingOptionsConfiguration>();
			powerShellResults.MergeErrors<CalendarConfiguration>(base.SetObject<CalendarConfiguration, SetMailboxCalendarConfiguration>("Set-MailboxCalendarConfiguration", Identity.FromExecutingUserId(), properties.SetMailboxCalendarConfiguration));
			if (powerShellResults.Failed)
			{
				return powerShellResults;
			}
			powerShellResults.MergeAll(base.SetObject<SchedulingOptionsConfiguration, SetSchedulingOptionsConfiguration>(identity, properties));
			return powerShellResults;
		}

		internal const string GetCalendarCmdlet = "Get-MailboxCalendarConfiguration";

		internal const string SetCalendarCmdlet = "Set-MailboxCalendarConfiguration";

		private const string GetSchedulingOptionsRole = "Resource+Get-CalendarProcessing?Identity@R:Self+Get-MailboxCalendarConfiguration?Identity@R:Self";

		private const string SetSchedulingOptionsRole = "Resource+Get-CalendarProcessing?Identity@R:Self+Get-MailboxCalendarConfiguration?Identity@R:Self+Set-MailboxCalendarConfiguration?Identity@R:Self";
	}
}
