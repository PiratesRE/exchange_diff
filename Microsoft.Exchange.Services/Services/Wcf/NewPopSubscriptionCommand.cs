using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class NewPopSubscriptionCommand : SingleCmdletCommandBase<NewPopSubscriptionRequest, NewPopSubscriptionResponse, NewPopSubscription, PopSubscriptionProxy>
	{
		public NewPopSubscriptionCommand(CallContext callContext, NewPopSubscriptionRequest request) : base(callContext, request, "New-PopSubscription", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			NewPopSubscription task = this.cmdletRunner.TaskWrapper.Task;
			NewPopSubscriptionData popSubscription = this.request.PopSubscription;
			this.cmdletRunner.SetTaskParameterIfModified("EmailAddress", popSubscription, task, (popSubscription.EmailAddress == null) ? ((SmtpAddress)null) : ((SmtpAddress)popSubscription.EmailAddress));
			this.cmdletRunner.SetTaskParameterIfModified("IncomingPassword", popSubscription, task, (popSubscription.IncomingPassword == null) ? null : popSubscription.IncomingPassword.ConvertToSecureString());
			this.cmdletRunner.SetTaskParameterIfModified("IncomingServer", popSubscription, task, (popSubscription.IncomingServer == null) ? null : new Fqdn(popSubscription.IncomingServer));
			this.cmdletRunner.SetRemainingModifiedTaskParameters(popSubscription, task);
		}

		protected override void PopulateResponseData(NewPopSubscriptionResponse response)
		{
			PopSubscriptionProxy result = this.cmdletRunner.TaskWrapper.Result;
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

		protected override PSLocalTask<NewPopSubscription, PopSubscriptionProxy> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateNewPopSubscriptionTask(base.CallContext.AccessingPrincipal);
		}
	}
}
