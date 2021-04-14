using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class RemoveSubscriptionCommand : SingleCmdletCommandBase<IdentityRequest, OptionsResponseBase, RemoveSubscription, PimSubscriptionProxy>
	{
		public RemoveSubscriptionCommand(CallContext callContext, IdentityRequest request) : base(callContext, request, "Remove-Subscription", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			RemoveSubscription task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Identity", task, new AggregationSubscriptionIdParameter(this.request.Identity));
		}

		protected override PSLocalTask<RemoveSubscription, PimSubscriptionProxy> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateRemoveSubscriptionTask(base.CallContext.AccessingPrincipal);
		}
	}
}
