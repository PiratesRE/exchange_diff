using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetConnectSubscriptionCommand : ConnectSubscriptionCommandBase<GetConnectSubscriptionRequest, GetConnectSubscriptionResponse, GetConnectSubscription, ConnectSubscriptionProxy>
	{
		public GetConnectSubscriptionCommand(CallContext callContext, GetConnectSubscriptionRequest request) : base(callContext, request, "Get-ConnectSubscription", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateTaskParameters()
		{
			GetConnectSubscription task = this.cmdletRunner.TaskWrapper.Task;
			if (this.request.Identity != null)
			{
				this.cmdletRunner.SetTaskParameter("Identity", task, new AggregationSubscriptionIdParameter(this.request.Identity));
			}
			this.cmdletRunner.SetTaskParameter("Mailbox", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
		}

		protected override void PopulateResponseData(GetConnectSubscriptionResponse response)
		{
			IEnumerable<ConnectSubscriptionProxy> allResults = this.cmdletRunner.TaskWrapper.AllResults;
			IEnumerable<ConnectSubscription> source = from e in allResults
			select base.CreateConnectSubscriptionData(e);
			response.ConnectSubscriptionCollection.ConnectSubscriptions = source.ToArray<ConnectSubscription>();
		}

		protected override PSLocalTask<GetConnectSubscription, ConnectSubscriptionProxy> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetConnectSubscriptionTask(base.CallContext.AccessingPrincipal);
		}
	}
}
