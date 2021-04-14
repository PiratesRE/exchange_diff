using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class RemoveInboxRuleCommand : SingleCmdletCommandBase<RemoveInboxRuleRequest, RemoveInboxRuleResponse, RemoveInboxRule, Microsoft.Exchange.Management.Common.InboxRule>
	{
		public RemoveInboxRuleCommand(CallContext callContext, RemoveInboxRuleRequest request) : base(callContext, request, "Remove-InboxRule", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			RemoveInboxRule task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("AlwaysDeleteOutlookRulesBlob", task, new SwitchParameter(this.request.AlwaysDeleteOutlookRulesBlob));
			this.cmdletRunner.SetTaskParameter("Force", task, new SwitchParameter(this.request.Force));
			this.cmdletRunner.SetTaskParameter("Identity", task, new InboxRuleIdParameter(this.request.Identity));
		}

		protected override PSLocalTask<RemoveInboxRule, Microsoft.Exchange.Management.Common.InboxRule> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateRemoveInboxRuleTask(base.CallContext.AccessingPrincipal);
		}
	}
}
