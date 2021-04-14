using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SetHotmailSubscriptionCommand : SingleCmdletCommandBase<SetHotmailSubscriptionRequest, OptionsResponseBase, SetHotmailSubscription, HotmailSubscriptionProxy>
	{
		public SetHotmailSubscriptionCommand(CallContext callContext, SetHotmailSubscriptionRequest request) : base(callContext, request, "Set-HotmailSubscription", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			SetHotmailSubscription task = this.cmdletRunner.TaskWrapper.Task;
			SetHotmailSubscriptionData hotmailSubscription = this.request.HotmailSubscription;
			this.cmdletRunner.SetTaskParameterIfModified("Identity", hotmailSubscription, task, (hotmailSubscription.Identity == null) ? null : new AggregationSubscriptionIdParameter(hotmailSubscription.Identity));
			this.cmdletRunner.SetTaskParameterIfModified("Password", hotmailSubscription, task, hotmailSubscription.Password.ConvertToSecureString());
			this.cmdletRunner.SetRemainingModifiedTaskParameters(hotmailSubscription, task);
		}

		protected override PSLocalTask<SetHotmailSubscription, HotmailSubscriptionProxy> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSetHotmailSubscriptionTask(base.CallContext.AccessingPrincipal);
		}

		public const string PasswordTaskPropertyName = "Password";
	}
}
