using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SetMailboxCommand : SingleCmdletCommandBase<SetMailboxRequest, OptionsResponseBase, SetMailbox, object>
	{
		public SetMailboxCommand(CallContext callContext, SetMailboxRequest request) : base(callContext, request, "Set-Mailbox", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			PSLocalTask<SetMailbox, object> taskWrapper = this.cmdletRunner.TaskWrapper;
			SetMailbox task = taskWrapper.Task;
			MailboxOptions mailbox = this.request.Mailbox;
			this.cmdletRunner.SetTaskParameter("Identity", task, mailbox.Identity.ToIdParameter<MailboxIdParameter>());
			object dynamicParameters = taskWrapper.Task.GetDynamicParameters();
			this.cmdletRunner.SetTaskParameterIfModified("AddressString", "ForwardingSmtpAddress", mailbox, dynamicParameters, string.IsNullOrEmpty(mailbox.AddressString) ? null : ProxyAddress.Parse(mailbox.AddressString));
			this.cmdletRunner.SetRemainingModifiedTaskParameters(mailbox, dynamicParameters);
		}

		protected override PSLocalTask<SetMailbox, object> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSetMailboxTask(base.CallContext.AccessingPrincipal, "Identity");
		}
	}
}
