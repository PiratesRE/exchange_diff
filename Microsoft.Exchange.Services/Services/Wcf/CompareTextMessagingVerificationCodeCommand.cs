using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.StoreTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class CompareTextMessagingVerificationCodeCommand : SingleCmdletCommandBase<CompareTextMessagingVerificationCodeRequest, CompareTextMessagingVerificationCodeResponse, CompareTextMessagingVerificationCode, object>
	{
		public CompareTextMessagingVerificationCodeCommand(CallContext callContext, CompareTextMessagingVerificationCodeRequest request) : base(callContext, request, "Compare-TextMessagingVerificationCode", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			CompareTextMessagingVerificationCode task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Identity", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			this.cmdletRunner.SetTaskParameter("VerificationCode", task, this.request.VerificationCode);
		}

		protected override PSLocalTask<CompareTextMessagingVerificationCode, object> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateCompareTextMessagingVerificationCodeTask(base.CallContext.AccessingPrincipal);
		}
	}
}
