using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class RemoveConnectSubscriptionCommand : SingleCmdletCommandBase<RemoveConnectSubscriptionRequest, RemoveConnectSubscriptionResponse, RemoveConnectSubscription, object>
	{
		public RemoveConnectSubscriptionCommand(CallContext callContext, RemoveConnectSubscriptionRequest request) : base(callContext, request, "Remove-ConnectSubscription", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			RemoveConnectSubscription task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Identity", task, new AggregationSubscriptionIdParameter(this.request.Identity));
		}

		protected override PSLocalTask<RemoveConnectSubscription, object> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateRemoveConnectSubscriptionTask(base.CallContext.AccessingPrincipal);
		}
	}
}
