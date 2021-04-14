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
	internal sealed class EnableInboxRuleCommand : SingleCmdletCommandBase<EnableInboxRuleRequest, EnableInboxRuleResponse, EnableInboxRule, Microsoft.Exchange.Management.Common.InboxRule>
	{
		public EnableInboxRuleCommand(CallContext callContext, EnableInboxRuleRequest request) : base(callContext, request, "Enable-InboxRule", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			EnableInboxRule task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("AlwaysDeleteOutlookRulesBlob", task, new SwitchParameter(this.request.AlwaysDeleteOutlookRulesBlob));
			this.cmdletRunner.SetTaskParameter("Force", task, new SwitchParameter(this.request.Force));
			this.cmdletRunner.SetTaskParameter("Identity", task, new InboxRuleIdParameter(this.request.Identity));
		}

		protected override PSLocalTask<EnableInboxRule, Microsoft.Exchange.Management.Common.InboxRule> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateEnableInboxRuleTask(base.CallContext.AccessingPrincipal);
		}
	}
}
