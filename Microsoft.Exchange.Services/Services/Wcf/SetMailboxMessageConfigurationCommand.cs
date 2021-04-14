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
	internal sealed class SetMailboxMessageConfigurationCommand : SingleCmdletCommandBase<SetMailboxMessageConfigurationRequest, OptionsResponseBase, SetMailboxMessageConfiguration, MailboxMessageConfiguration>
	{
		public SetMailboxMessageConfigurationCommand(CallContext callContext, SetMailboxMessageConfigurationRequest request) : base(callContext, request, "Set-MailboxMessageConfiguration", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			PSLocalTask<SetMailboxMessageConfiguration, MailboxMessageConfiguration> taskWrapper = this.cmdletRunner.TaskWrapper;
			this.cmdletRunner.SetTaskParameter("Identity", taskWrapper.Task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			MailboxMessageConfiguration taskParameters = (MailboxMessageConfiguration)taskWrapper.Task.GetDynamicParameters();
			this.cmdletRunner.SetRemainingModifiedTaskParameters(this.request.Options, taskParameters);
		}

		protected override PSLocalTask<SetMailboxMessageConfiguration, MailboxMessageConfiguration> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSetMailboxMessageConfigurationTask(base.CallContext.AccessingPrincipal);
		}
	}
}
