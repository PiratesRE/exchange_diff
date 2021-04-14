using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetPopSubscriptionCommand : SingleCmdletCommandBase<IdentityRequest, GetPopSubscriptionResponse, GetPopSubscription, PopSubscriptionProxy>
	{
		public GetPopSubscriptionCommand(CallContext callContext, IdentityRequest request) : base(callContext, request, "Get-PopSubscription", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateTaskParameters()
		{
			GetPopSubscription task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Identity", task, new AggregationSubscriptionIdParameter(this.request.Identity));
		}

		protected override void PopulateResponseData(GetPopSubscriptionResponse response)
		{
			PopSubscriptionProxy result = this.cmdletRunner.TaskWrapper.Result;
			if (result == null)
			{
				response.PopSubscription = null;
				return;
			}
			response.PopSubscription = new PopSubscription
			{
				DetailedStatus = result.DetailedStatus,
				DisplayName = result.DisplayName,
				EmailAddress = result.EmailAddress.ToString(),
				Identity = new Identity(result.Identity.ToString(), result.DisplayName),
				IncomingAuth = result.IncomingAuthentication,
				IncomingPort = result.IncomingPort,
				IncomingSecurity = result.IncomingSecurity,
				IncomingServer = result.IncomingServer,
				IncomingUserName = result.IncomingUserName,
				IsErrorStatus = result.IsErrorStatus,
				IsValid = result.IsValid,
				LastSuccessfulSync = ((result.LastSuccessfulSync == null) ? null : ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime((ExDateTime)result.LastSuccessfulSync.Value)),
				LeaveOnServer = result.LeaveOnServer,
				Name = result.Name,
				SendAsState = result.SendAsState,
				Status = result.Status,
				StatusDescription = result.StatusDescription,
				SubscriptionType = result.SubscriptionType
			};
		}

		protected override PSLocalTask<GetPopSubscription, PopSubscriptionProxy> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetPopSubscriptionTask(base.CallContext.AccessingPrincipal);
		}
	}
}
