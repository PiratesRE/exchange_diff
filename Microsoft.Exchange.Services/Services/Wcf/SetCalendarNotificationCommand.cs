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
	internal sealed class SetCalendarNotificationCommand : SingleCmdletCommandBase<SetCalendarNotificationRequest, OptionsResponseBase, SetCalendarNotification, CalendarNotification>
	{
		public SetCalendarNotificationCommand(CallContext callContext, SetCalendarNotificationRequest request) : base(callContext, request, "Set-CalendarNotification", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			PSLocalTask<SetCalendarNotification, CalendarNotification> taskWrapper = this.cmdletRunner.TaskWrapper;
			this.cmdletRunner.SetTaskParameter("Identity", taskWrapper.Task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			CalendarNotification taskParameters = (CalendarNotification)taskWrapper.Task.GetDynamicParameters();
			this.cmdletRunner.SetTaskParameterIfModified("DailyAgendaNotificationSendTime", this.request.Options, taskParameters, new TimeSpan(0, this.request.Options.DailyAgendaNotificationSendTime, 0));
			this.cmdletRunner.SetRemainingModifiedTaskParameters(this.request.Options, taskParameters);
		}

		protected override PSLocalTask<SetCalendarNotification, CalendarNotification> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSetCalendarNotificationTask(base.CallContext.AccessingPrincipal);
		}
	}
}
