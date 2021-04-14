using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SetConnectSubscriptionCommand : ConnectSubscriptionCommandBase<SetConnectSubscriptionRequest, SetConnectSubscriptionResponse, SetConnectSubscription, object>
	{
		public SetConnectSubscriptionCommand(CallContext callContext, SetConnectSubscriptionRequest request) : base(callContext, request, "Set-ConnectSubscription", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			SetConnectSubscription task = this.cmdletRunner.TaskWrapper.Task;
			SetConnectSubscriptionData connectSubscription = this.request.ConnectSubscription;
			this.cmdletRunner.SetTaskParameterIfModified("Identity", connectSubscription, task, new AggregationSubscriptionIdParameter(connectSubscription.Identity));
			base.PopulateTaskParametersFromConnectSubscription(connectSubscription);
		}

		protected override PSLocalTask<SetConnectSubscription, object> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSetConnectSubscriptionTask(base.CallContext.AccessingPrincipal, base.CalculateParameterSet(this.request.ConnectSubscription.ConnectSubscriptionType));
		}
	}
}
