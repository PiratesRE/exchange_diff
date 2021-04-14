using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SetImapSubscriptionCommand : SingleCmdletCommandBase<SetImapSubscriptionRequest, OptionsResponseBase, SetImapSubscription, IMAPSubscriptionProxy>
	{
		public SetImapSubscriptionCommand(CallContext callContext, SetImapSubscriptionRequest request) : base(callContext, request, "Set-ImapSubscription", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			SetImapSubscription task = this.cmdletRunner.TaskWrapper.Task;
			SetImapSubscriptionData imapSubscription = this.request.ImapSubscription;
			this.cmdletRunner.SetTaskParameterIfModified("EmailAddress", imapSubscription, task, (imapSubscription.EmailAddress == null) ? ((SmtpAddress)null) : ((SmtpAddress)imapSubscription.EmailAddress));
			this.cmdletRunner.SetTaskParameterIfModified("Identity", imapSubscription, task, (imapSubscription.Identity == null) ? null : new AggregationSubscriptionIdParameter(imapSubscription.Identity));
			this.cmdletRunner.SetTaskParameterIfModified("IncomingPassword", imapSubscription, task, (imapSubscription.IncomingPassword == null) ? null : imapSubscription.IncomingPassword.ConvertToSecureString());
			this.cmdletRunner.SetTaskParameterIfModified("IncomingServer", imapSubscription, task, (imapSubscription.IncomingServer == null) ? null : new Fqdn(imapSubscription.IncomingServer));
			this.cmdletRunner.SetTaskParameterIfModified("ResendVerification", imapSubscription, task, new SwitchParameter(imapSubscription.ResendVerification));
			this.cmdletRunner.SetRemainingModifiedTaskParameters(imapSubscription, task);
		}

		protected override PSLocalTask<SetImapSubscription, IMAPSubscriptionProxy> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSetImapSubscriptionTask(base.CallContext.AccessingPrincipal);
		}
	}
}
