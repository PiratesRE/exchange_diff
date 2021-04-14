using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.StoreTasks;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SetMailboxAutoReplyConfigurationCommand : SingleCmdletCommandBase<SetMailboxAutoReplyConfigurationRequest, OptionsResponseBase, SetMailboxAutoReplyConfiguration, MailboxAutoReplyConfiguration>
	{
		public SetMailboxAutoReplyConfigurationCommand(CallContext callContext, SetMailboxAutoReplyConfigurationRequest request) : base(callContext, request, "Set-MailboxAutoReplyConfiguration", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			MailboxAutoReplyConfigurationOptions options = this.request.Options;
			SetMailboxAutoReplyConfiguration task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Identity", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			object dynamicParameters = task.GetDynamicParameters();
			if (options.EndTime != null)
			{
				this.cmdletRunner.SetTaskParameter("EndTime", dynamicParameters, (DateTime)ExDateTimeConverter.Parse(options.EndTime));
			}
			if (options.StartTime != null)
			{
				this.cmdletRunner.SetTaskParameter("StartTime", dynamicParameters, (DateTime)ExDateTimeConverter.Parse(options.StartTime));
			}
			this.cmdletRunner.SetRemainingModifiedTaskParameters(options, dynamicParameters);
		}

		protected override PSLocalTask<SetMailboxAutoReplyConfiguration, MailboxAutoReplyConfiguration> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSetMailboxAutoReplyConfigurationTask(base.CallContext.AccessingPrincipal);
		}
	}
}
