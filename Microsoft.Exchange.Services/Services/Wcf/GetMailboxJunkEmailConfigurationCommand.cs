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
	internal sealed class GetMailboxJunkEmailConfigurationCommand : SingleCmdletCommandBase<object, GetMailboxJunkEmailConfigurationResponse, GetMailboxJunkEmailConfiguration, MailboxJunkEmailConfiguration>
	{
		public GetMailboxJunkEmailConfigurationCommand(CallContext callContext) : base(callContext, null, "Get-MailboxJunkEmailConfiguration", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateTaskParameters()
		{
			PSLocalTask<GetMailboxJunkEmailConfiguration, MailboxJunkEmailConfiguration> taskWrapper = this.cmdletRunner.TaskWrapper;
			this.cmdletRunner.SetTaskParameter("Identity", taskWrapper.Task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
		}

		protected override void PopulateResponseData(GetMailboxJunkEmailConfigurationResponse response)
		{
			PSLocalTask<GetMailboxJunkEmailConfiguration, MailboxJunkEmailConfiguration> taskWrapper = this.cmdletRunner.TaskWrapper;
			response.Options = new MailboxJunkEmailConfigurationOptions
			{
				Enabled = taskWrapper.Result.Enabled,
				ContactsTrusted = taskWrapper.Result.ContactsTrusted,
				TrustedListsOnly = taskWrapper.Result.TrustedListsOnly,
				TrustedSendersAndDomains = taskWrapper.Result.TrustedSendersAndDomains.ToArray(),
				BlockedSendersAndDomains = taskWrapper.Result.BlockedSendersAndDomains.ToArray()
			};
		}

		protected override PSLocalTask<GetMailboxJunkEmailConfiguration, MailboxJunkEmailConfiguration> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetMailboxJunkEmailConfigurationTask(base.CallContext.AccessingPrincipal);
		}
	}
}
