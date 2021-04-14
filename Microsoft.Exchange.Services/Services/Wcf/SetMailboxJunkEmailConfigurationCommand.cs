using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.StoreTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SetMailboxJunkEmailConfigurationCommand : SingleCmdletCommandBase<SetMailboxJunkEmailConfigurationRequest, OptionsResponseBase, SetMailboxJunkEmailConfiguration, MailboxJunkEmailConfiguration>
	{
		public SetMailboxJunkEmailConfigurationCommand(CallContext callContext, SetMailboxJunkEmailConfigurationRequest request) : base(callContext, request, "Set-MailboxJunkEmailConfiguration", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			PSLocalTask<SetMailboxJunkEmailConfiguration, MailboxJunkEmailConfiguration> taskWrapper = this.cmdletRunner.TaskWrapper;
			this.cmdletRunner.SetTaskParameter("Identity", taskWrapper.Task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			MailboxJunkEmailConfiguration taskParameters = (MailboxJunkEmailConfiguration)taskWrapper.Task.GetDynamicParameters();
			this.cmdletRunner.SetTaskParameterIfModified("BlockedSendersAndDomains", this.request.Options, taskParameters, new MultiValuedProperty<string>(this.request.Options.BlockedSendersAndDomains));
			this.cmdletRunner.SetTaskParameterIfModified("TrustedSendersAndDomains", this.request.Options, taskParameters, new MultiValuedProperty<string>(this.request.Options.TrustedSendersAndDomains));
			this.cmdletRunner.SetRemainingModifiedTaskParameters(this.request.Options, taskParameters);
		}

		protected override PSLocalTask<SetMailboxJunkEmailConfiguration, MailboxJunkEmailConfiguration> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSetMailboxJunkEmailConfigurationTask(base.CallContext.AccessingPrincipal);
		}
	}
}
