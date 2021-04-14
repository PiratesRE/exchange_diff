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
	internal sealed class SetMailboxCalendarConfigurationCommand : SingleCmdletCommandBase<SetMailboxCalendarConfigurationRequest, OptionsResponseBase, SetMailboxCalendarConfiguration, Microsoft.Exchange.Data.Storage.Management.MailboxCalendarConfiguration>
	{
		public SetMailboxCalendarConfigurationCommand(CallContext callContext, SetMailboxCalendarConfigurationRequest request) : base(callContext, request, "Set-MailboxCalendarConfiguration", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			SetMailboxCalendarConfiguration task = this.cmdletRunner.TaskWrapper.Task;
			Microsoft.Exchange.Services.Core.Types.MailboxCalendarConfiguration options = this.request.Options;
			this.cmdletRunner.SetTaskParameter("Identity", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			Microsoft.Exchange.Data.Storage.Management.MailboxCalendarConfiguration taskParameters = (Microsoft.Exchange.Data.Storage.Management.MailboxCalendarConfiguration)task.GetDynamicParameters();
			this.cmdletRunner.SetTaskParameterIfModified("WorkingHoursStartTime", options, taskParameters, new TimeSpan(0, options.WorkingHoursStartTime, 0));
			this.cmdletRunner.SetTaskParameterIfModified("WorkingHoursEndTime", options, taskParameters, new TimeSpan(0, options.WorkingHoursEndTime, 0));
			this.cmdletRunner.SetTaskParameterIfModified("DefaultReminderTime", options, taskParameters, new TimeSpan(0, (int)options.DefaultReminderTime, 0));
			this.cmdletRunner.SetTaskParameterIfModified("WorkingHoursTimeZone", options, taskParameters, (options.WorkingHoursTimeZone != null) ? ExTimeZoneValue.Parse(options.WorkingHoursTimeZone.TimeZoneId) : null);
			this.cmdletRunner.SetRemainingModifiedTaskParameters(options, taskParameters);
		}

		protected override PSLocalTask<SetMailboxCalendarConfiguration, Microsoft.Exchange.Data.Storage.Management.MailboxCalendarConfiguration> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSetMailboxCalendarConfigurationTask(base.CallContext.AccessingPrincipal);
		}
	}
}
