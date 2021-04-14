using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.StoreTasks;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetConnectedAccountsCommand : OptionServiceCommandBase<GetConnectedAccountsRequest, GetConnectedAccountsResponse>
	{
		public GetConnectedAccountsCommand(CallContext callContext, GetConnectedAccountsRequest request) : base(callContext, request)
		{
		}

		protected override GetConnectedAccountsResponse CreateTaskAndExecute()
		{
			this.GetSubscriptions();
			this.GetDefaultReplyAddress();
			this.GetSendAddresses();
			return this.MergeResultsAndGenerateSuccessResponse();
		}

		private void GetSubscriptions()
		{
			PSLocalTask<GetSubscription, PimSubscriptionProxy> taskWrapper = CmdletTaskFactory.Instance.CreateGetSubscriptionTask(base.CallContext.AccessingPrincipal);
			CmdletRunner<GetSubscription, PimSubscriptionProxy> cmdletRunner = new CmdletRunner<GetSubscription, PimSubscriptionProxy>(base.CallContext, "Get-Subscription", ScopeLocation.RecipientRead, taskWrapper);
			cmdletRunner.Execute();
			this.subscriptionsList = cmdletRunner.TaskAllResults.ToList<PimSubscriptionProxy>();
		}

		private void GetDefaultReplyAddress()
		{
			PSLocalTask<GetMailboxMessageConfiguration, MailboxMessageConfiguration> pslocalTask = CmdletTaskFactory.Instance.CreateGetMailboxMessageConfigurationTask(base.CallContext.AccessingPrincipal);
			CmdletRunner<GetMailboxMessageConfiguration, MailboxMessageConfiguration> cmdletRunner = new CmdletRunner<GetMailboxMessageConfiguration, MailboxMessageConfiguration>(base.CallContext, "Get-MailboxMessageConfiguration", ScopeLocation.RecipientRead, pslocalTask);
			cmdletRunner.SetTaskParameter("Identity", pslocalTask.Task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			cmdletRunner.Execute();
			this.defaultReplyAddress = cmdletRunner.TaskResult.SendAddressDefault;
		}

		private void GetSendAddresses()
		{
			PSLocalTask<GetSendAddress, SendAddress> pslocalTask = CmdletTaskFactory.Instance.CreateGetSendAddressTask(base.CallContext.AccessingPrincipal);
			CmdletRunner<GetSendAddress, SendAddress> cmdletRunner = new CmdletRunner<GetSendAddress, SendAddress>(base.CallContext, "Get-SendAddress", ScopeLocation.RecipientRead, pslocalTask);
			cmdletRunner.SetTaskParameter("Mailbox", pslocalTask.Task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			cmdletRunner.Execute();
			this.sendAddressesList = cmdletRunner.TaskAllResults.ToList<SendAddress>();
		}

		private GetConnectedAccountsResponse MergeResultsAndGenerateSuccessResponse()
		{
			GetConnectedAccountsResponse getConnectedAccountsResponse = new GetConnectedAccountsResponse();
			ConnectedAccountsInformation connectedAccountsInformation = getConnectedAccountsResponse.ConnectedAccountsInformation;
			connectedAccountsInformation.SendAddresses = (from t in this.sendAddressesList
			select new SendAddressData
			{
				AddressId = t.AddressId,
				DisplayName = t.DisplayName
			}).ToArray<SendAddressData>();
			connectedAccountsInformation.Subscriptions = (from e in this.subscriptionsList
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
			}).ToArray<Subscription>();
			SendAddress sendAddress = (from s in this.sendAddressesList
			where s.Identity.ToString().Equals(this.defaultReplyAddress) || s.AddressId.Equals(this.defaultReplyAddress) || (string.IsNullOrEmpty(s.AddressId) && string.IsNullOrEmpty(this.defaultReplyAddress))
			select s).SingleOrDefault<SendAddress>();
			if (sendAddress == null)
			{
				connectedAccountsInformation.DefaultReplyAddress = new Identity(this.defaultReplyAddress, string.Empty);
			}
			else
			{
				connectedAccountsInformation.DefaultReplyAddress = new Identity(sendAddress.AddressId, sendAddress.DisplayName);
			}
			getConnectedAccountsResponse.WasSuccessful = true;
			return getConnectedAccountsResponse;
		}

		private string defaultReplyAddress;

		private IList<SendAddress> sendAddressesList;

		private IList<PimSubscriptionProxy> subscriptionsList;
	}
}
