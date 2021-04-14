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
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class NewImapSubscriptionCommand : SingleCmdletCommandBase<NewImapSubscriptionRequest, NewImapSubscriptionResponse, NewImapSubscription, IMAPSubscriptionProxy>
	{
		public NewImapSubscriptionCommand(CallContext callContext, NewImapSubscriptionRequest request) : base(callContext, request, "New-ImapSubscription", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			NewImapSubscription task = this.cmdletRunner.TaskWrapper.Task;
			NewImapSubscriptionData imapSubscription = this.request.ImapSubscription;
			this.cmdletRunner.SetTaskParameterIfModified("EmailAddress", imapSubscription, task, (imapSubscription.EmailAddress == null) ? ((SmtpAddress)null) : ((SmtpAddress)imapSubscription.EmailAddress));
			this.cmdletRunner.SetTaskParameterIfModified("IncomingPassword", imapSubscription, task, (imapSubscription.IncomingPassword == null) ? null : imapSubscription.IncomingPassword.ConvertToSecureString());
			this.cmdletRunner.SetTaskParameterIfModified("IncomingServer", imapSubscription, task, (imapSubscription.IncomingServer == null) ? null : new Fqdn(imapSubscription.IncomingServer));
			this.cmdletRunner.SetRemainingModifiedTaskParameters(imapSubscription, task);
		}

		protected override void PopulateResponseData(NewImapSubscriptionResponse response)
		{
			IMAPSubscriptionProxy result = this.cmdletRunner.TaskWrapper.Result;
			response.ImapSubscription = new ImapSubscription
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
				Name = result.Name,
				SendAsState = result.SendAsState,
				Status = result.Status,
				StatusDescription = result.StatusDescription,
				SubscriptionType = result.SubscriptionType
			};
		}

		protected override PSLocalTask<NewImapSubscription, IMAPSubscriptionProxy> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateNewImapSubscriptionTask(base.CallContext.AccessingPrincipal);
		}
	}
}
