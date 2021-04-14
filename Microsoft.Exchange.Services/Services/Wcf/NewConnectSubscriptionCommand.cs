using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class NewConnectSubscriptionCommand : ConnectSubscriptionCommandBase<NewConnectSubscriptionRequest, NewConnectSubscriptionResponse, NewConnectSubscription, ConnectSubscriptionProxy>
	{
		public NewConnectSubscriptionCommand(CallContext callContext, NewConnectSubscriptionRequest request) : base(callContext, request, "New-ConnectSubscription", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			base.PopulateTaskParametersFromConnectSubscription(this.request.ConnectSubscription);
		}

		protected override void PopulateResponseData(NewConnectSubscriptionResponse response)
		{
			ConnectSubscriptionProxy result = this.cmdletRunner.TaskWrapper.Result;
			response.ConnectSubscription = base.CreateConnectSubscriptionData(result);
		}

		protected override PSLocalTask<NewConnectSubscription, ConnectSubscriptionProxy> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateNewConnectSubscriptionTask(base.CallContext.AccessingPrincipal, base.CalculateParameterSet(this.request.ConnectSubscription.ConnectSubscriptionType));
		}
	}
}
