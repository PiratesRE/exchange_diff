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
	internal sealed class SetCalendarProcessingCommand : SingleCmdletCommandBase<SetCalendarProcessingRequest, OptionsResponseBase, SetCalendarProcessing, CalendarConfiguration>
	{
		public SetCalendarProcessingCommand(CallContext callContext, SetCalendarProcessingRequest request) : base(callContext, request, "Set-CalendarProcessing", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			PSLocalTask<SetCalendarProcessing, CalendarConfiguration> taskWrapper = this.cmdletRunner.TaskWrapper;
			this.cmdletRunner.SetTaskParameter("Identity", taskWrapper.Task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			CalendarConfiguration taskParameters = (CalendarConfiguration)taskWrapper.Task.GetDynamicParameters();
			this.cmdletRunner.SetRemainingModifiedTaskParameters(this.request.Options, taskParameters);
		}

		protected override PSLocalTask<SetCalendarProcessing, CalendarConfiguration> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSetCalendarProcessingTask(base.CallContext.AccessingPrincipal);
		}
	}
}
