using System;
using System.Globalization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.StoreTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SetTextMessagingAccountCommand : SingleCmdletCommandBase<SetTextMessagingAccountRequest, SetTextMessagingAccountResponse, SetTextMessagingAccount, object>
	{
		public SetTextMessagingAccountCommand(CallContext callContext, SetTextMessagingAccountRequest request) : base(callContext, request, "Set-TextMessagingAccount", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			SetTextMessagingAccount task = this.cmdletRunner.TaskWrapper.Task;
			SetTextMessagingAccountData options = this.request.Options;
			this.cmdletRunner.SetTaskParameter("Identity", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			Microsoft.Exchange.Data.Storage.Management.TextMessagingAccount taskParameters = (Microsoft.Exchange.Data.Storage.Management.TextMessagingAccount)task.GetDynamicParameters();
			if (!string.IsNullOrEmpty(options.CountryRegionId))
			{
				this.cmdletRunner.SetTaskParameterIfModified("CountryRegionId", options, taskParameters, new RegionInfo(options.CountryRegionId));
			}
			E164Number newPropertyValue = (options.NotificationPhoneNumber != null) ? E164Number.Parse(options.NotificationPhoneNumber) : null;
			this.cmdletRunner.SetTaskParameterIfModified("NotificationPhoneNumber", options, taskParameters, newPropertyValue);
			this.cmdletRunner.SetTaskParameterIfModified("MobileOperatorId", options, taskParameters, options.MobileOperatorId);
		}

		protected override PSLocalTask<SetTextMessagingAccount, object> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSetTextMessagingAccountTask(base.CallContext.AccessingPrincipal);
		}
	}
}
