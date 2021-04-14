using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.StoreTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetCalendarProcessingCommand : SingleCmdletCommandBase<object, GetCalendarProcessingResponse, GetCalendarProcessing, CalendarConfiguration>
	{
		public GetCalendarProcessingCommand(CallContext callContext) : base(callContext, null, "Get-CalendarProcessing", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateTaskParameters()
		{
			PSLocalTask<GetCalendarProcessing, CalendarConfiguration> taskWrapper = this.cmdletRunner.TaskWrapper;
			this.cmdletRunner.SetTaskParameter("Identity", taskWrapper.Task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
		}

		protected override void PopulateResponseData(GetCalendarProcessingResponse response)
		{
			PSLocalTask<GetCalendarProcessing, CalendarConfiguration> taskWrapper = this.cmdletRunner.TaskWrapper;
			CalendarConfiguration result = taskWrapper.Result;
			response.Options = new CalendarProcessingOptions
			{
				RemoveOldMeetingMessages = result.RemoveOldMeetingMessages,
				RemoveForwardedMeetingNotifications = result.RemoveForwardedMeetingNotifications
			};
		}

		protected override PSLocalTask<GetCalendarProcessing, CalendarConfiguration> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetCalendarProcessingTask(base.CallContext.AccessingPrincipal);
		}
	}
}
