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
	internal sealed class GetCalendarNotificationCommand : SingleCmdletCommandBase<object, GetCalendarNotificationResponse, GetCalendarNotification, CalendarNotification>
	{
		public GetCalendarNotificationCommand(CallContext callContext) : base(callContext, null, "Get-CalendarNotification", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateTaskParameters()
		{
			PSLocalTask<GetCalendarNotification, CalendarNotification> taskWrapper = this.cmdletRunner.TaskWrapper;
			this.cmdletRunner.SetTaskParameter("Identity", taskWrapper.Task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
		}

		protected override void PopulateResponseData(GetCalendarNotificationResponse response)
		{
			PSLocalTask<GetCalendarNotification, CalendarNotification> taskWrapper = this.cmdletRunner.TaskWrapper;
			response.Options = new CalendarNotificationOptions
			{
				CalendarUpdateNotification = taskWrapper.Result.CalendarUpdateNotification,
				NextDays = taskWrapper.Result.NextDays,
				CalendarUpdateSendDuringWorkHour = taskWrapper.Result.CalendarUpdateSendDuringWorkHour,
				MeetingReminderNotification = taskWrapper.Result.MeetingReminderNotification,
				MeetingReminderSendDuringWorkHour = taskWrapper.Result.MeetingReminderSendDuringWorkHour,
				DailyAgendaNotification = taskWrapper.Result.DailyAgendaNotification,
				DailyAgendaNotificationSendTime = (int)taskWrapper.Result.DailyAgendaNotificationSendTime.TotalMinutes
			};
		}

		protected override PSLocalTask<GetCalendarNotification, CalendarNotification> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetCalendarNotificationTask(base.CallContext.AccessingPrincipal);
		}
	}
}
