using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.StoreTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetMailboxCalendarConfigurationCommand : SingleCmdletCommandBase<object, GetMailboxCalendarConfigurationResponse, GetMailboxCalendarConfiguration, Microsoft.Exchange.Data.Storage.Management.MailboxCalendarConfiguration>
	{
		public GetMailboxCalendarConfigurationCommand(CallContext callContext) : base(callContext, null, "Get-MailboxCalendarConfiguration", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateTaskParameters()
		{
			GetMailboxCalendarConfiguration task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Identity", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
		}

		protected override void PopulateResponseData(GetMailboxCalendarConfigurationResponse response)
		{
			Microsoft.Exchange.Data.Storage.Management.MailboxCalendarConfiguration result = this.cmdletRunner.TaskWrapper.Result;
			response.Options = new Microsoft.Exchange.Services.Core.Types.MailboxCalendarConfiguration
			{
				CurrentTimeZone = new TimeZoneInformation
				{
					TimeZoneId = EWSSettings.RequestTimeZone.Id,
					DisplayName = EWSSettings.RequestTimeZone.LocalizableDisplayName.ToString(base.CallContext.ClientCulture)
				},
				WorkDays = result.WorkDays,
				WorkingHoursStartTime = (int)result.WorkingHoursStartTime.TotalMinutes,
				WorkingHoursEndTime = (int)result.WorkingHoursEndTime.TotalMinutes,
				WorkingHoursTimeZone = new TimeZoneInformation
				{
					TimeZoneId = result.WorkingHoursTimeZone.ExTimeZone.Id,
					DisplayName = result.WorkingHoursTimeZone.ExTimeZone.LocalizableDisplayName.ToString(base.CallContext.ClientCulture)
				},
				WeekStartDay = result.WeekStartDay,
				ShowWeekNumbers = result.ShowWeekNumbers,
				FirstWeekOfYear = result.FirstWeekOfYear,
				TimeIncrement = result.TimeIncrement,
				RemindersEnabled = result.RemindersEnabled,
				ReminderSoundEnabled = result.ReminderSoundEnabled,
				DefaultReminderTime = (CalendarReminder)result.DefaultReminderTime.TotalMinutes
			};
		}

		protected override PSLocalTask<GetMailboxCalendarConfiguration, Microsoft.Exchange.Data.Storage.Management.MailboxCalendarConfiguration> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetMailboxCalendarConfigurationTask(base.CallContext.AccessingPrincipal);
		}
	}
}
