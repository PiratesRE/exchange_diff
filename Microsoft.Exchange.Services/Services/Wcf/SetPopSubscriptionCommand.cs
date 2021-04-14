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
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SetPopSubscriptionCommand : SingleCmdletCommandBase<SetPopSubscriptionRequest, OptionsResponseBase, SetPopSubscription, PopSubscriptionProxy>
	{
		public SetPopSubscriptionCommand(CallContext callContext, SetPopSubscriptionRequest request) : base(callContext, request, "Set-PopSubscription", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			SetPopSubscription task = this.cmdletRunner.TaskWrapper.Task;
			SetPopSubscriptionData popSubscription = this.request.PopSubscription;
			this.cmdletRunner.SetTaskParameterIfModified("EmailAddress", popSubscription, task, (popSubscription.EmailAddress == null) ? ((SmtpAddress)null) : ((SmtpAddress)popSubscription.EmailAddress));
			this.cmdletRunner.SetTaskParameterIfModified("Identity", popSubscription, task, (popSubscription.Identity == null) ? null : new AggregationSubscriptionIdParameter(popSubscription.Identity));
			this.cmdletRunner.SetTaskParameterIfModified("IncomingPassword", popSubscription, task, (popSubscription.IncomingPassword == null) ? null : popSubscription.IncomingPassword.ConvertToSecureString());
			this.cmdletRunner.SetTaskParameterIfModified("IncomingServer", popSubscription, task, (popSubscription.IncomingServer == null) ? null : new Fqdn(popSubscription.IncomingServer));
			this.cmdletRunner.SetTaskParameterIfModified("ResendVerification", popSubscription, task, new SwitchParameter(popSubscription.ResendVerification));
			this.cmdletRunner.SetRemainingModifiedTaskParameters(popSubscription, task);
		}

		protected override PSLocalTask<SetPopSubscription, PopSubscriptionProxy> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSetPopSubscriptionTask(base.CallContext.AccessingPrincipal);
		}
	}
}
