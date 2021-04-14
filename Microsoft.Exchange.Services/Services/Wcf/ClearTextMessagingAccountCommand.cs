using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.StoreTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class ClearTextMessagingAccountCommand : SingleCmdletCommandBase<ClearTextMessagingAccountRequest, ClearTextMessagingAccountResponse, ClearTextMessagingAccount, object>
	{
		public ClearTextMessagingAccountCommand(CallContext callContext, ClearTextMessagingAccountRequest request) : base(callContext, request, "Clear-TextMessagingAccount", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			ClearTextMessagingAccount task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Identity", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
		}

		protected override PSLocalTask<ClearTextMessagingAccount, object> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateClearTextMessagingAccountTask(base.CallContext.AccessingPrincipal);
		}
	}
}
