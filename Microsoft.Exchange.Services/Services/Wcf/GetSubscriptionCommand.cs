using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Search;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetSubscriptionCommand : SingleCmdletCommandBase<object, GetSubscriptionResponse, GetSubscription, PimSubscriptionProxy>
	{
		public GetSubscriptionCommand(CallContext callContext) : base(callContext, null, "Get-Subscription", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateResponseData(GetSubscriptionResponse response)
		{
			IEnumerable<PimSubscriptionProxy> allResults = this.cmdletRunner.TaskWrapper.AllResults;
			IEnumerable<Subscription> enumerable = from e in allResults
			select new Subscription
			{
				DetailedStatus = e.DetailedStatus,
				DisplayName = e.DisplayName,
				EmailAddress = e.EmailAddress.ToString(),
				Identity = new Identity(e.Identity.ToString()),
				IsErrorStatus = e.IsErrorStatus,
				IsValid = e.IsValid,
				LastSuccessfulSync = ((e.LastSuccessfulSync == null) ? null : ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime((ExDateTime)e.LastSuccessfulSync.Value)),
				Name = e.Name,
				SendAsState = e.SendAsState,
				Status = e.Status,
				StatusDescription = e.StatusDescription,
				SubscriptionType = e.SubscriptionType
			};
			response.SubscriptionCollection.Subscriptions = (enumerable.IsNullOrEmpty<Subscription>() ? null : enumerable.ToArray<Subscription>());
		}

		protected override PSLocalTask<GetSubscription, PimSubscriptionProxy> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetSubscriptionTask(base.CallContext.AccessingPrincipal);
		}
	}
}
