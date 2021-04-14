using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.StoreTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SendTextMessagingVerificationCodeCommand : SingleCmdletCommandBase<SendTextMessagingVerificationCodeRequest, SendTextMessagingVerificationCodeResponse, SendTextMessagingVerificationCode, object>
	{
		public SendTextMessagingVerificationCodeCommand(CallContext callContext, SendTextMessagingVerificationCodeRequest request) : base(callContext, request, "Send-TextMessagingVerificationCode", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			SendTextMessagingVerificationCode task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Identity", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			this.warningCollector = new CmdletResultsWarningCollector();
			this.cmdletRunner.TaskWrapper.Task.PrependTaskIOPipelineHandler(this.warningCollector);
		}

		protected override void PopulateResponseData(SendTextMessagingVerificationCodeResponse response)
		{
			response.WarningMessages = this.warningCollector.GetWarningMessagesForResult(0);
		}

		protected override PSLocalTask<SendTextMessagingVerificationCode, object> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSendTextMessagingVerificationCodeTask(base.CallContext.AccessingPrincipal);
		}

		private CmdletResultsWarningCollector warningCollector;
	}
}
