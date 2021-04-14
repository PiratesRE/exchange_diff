using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetMailboxByIdentityCommand : SingleCmdletCommandBase<IdentityRequest, GetMailboxResponse, GetMailbox, Mailbox>
	{
		public GetMailboxByIdentityCommand(CallContext callContext, IdentityRequest request) : base(callContext, request, "Get-Mailbox", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateTaskParameters()
		{
			GetMailbox task = this.cmdletRunner.TaskWrapper.Task;
			if (this.request.Identity != null)
			{
				this.cmdletRunner.SetTaskParameter("Identity", task, this.request.Identity.ToIdParameter<MailboxIdParameter>());
			}
			else
			{
				this.cmdletRunner.SetTaskParameter("Identity", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			}
			this.cmdletRunner.SetTaskParameter("ResultSize", task, new Unlimited<uint>(1U));
		}

		protected override void PopulateResponseData(GetMailboxResponse response)
		{
			Mailbox result = this.cmdletRunner.TaskWrapper.Result;
			response.MailboxOptions = ((result == null) ? null : new MailboxOptions
			{
				AddressString = ((result.ForwardingSmtpAddress == null) ? null : result.ForwardingSmtpAddress.AddressString),
				Identity = new Identity(result.Identity.ToString(), result.DisplayName),
				DeliverToMailboxAndForward = result.DeliverToMailboxAndForward
			});
		}

		protected override PSLocalTask<GetMailbox, Mailbox> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetMailboxTask(base.CallContext.AccessingPrincipal, "Identity");
		}
	}
}
