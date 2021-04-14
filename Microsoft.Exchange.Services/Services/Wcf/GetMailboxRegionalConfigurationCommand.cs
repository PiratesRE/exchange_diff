using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.StoreTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetMailboxRegionalConfigurationCommand : SingleCmdletCommandBase<GetMailboxRegionalConfigurationRequest, GetMailboxRegionalConfigurationResponse, GetMailboxRegionalConfiguration, MailboxRegionalConfiguration>
	{
		public GetMailboxRegionalConfigurationCommand(CallContext callContext, GetMailboxRegionalConfigurationRequest request) : base(callContext, request, "Get-MailboxRegionalConfiguration", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateTaskParameters()
		{
			GetMailboxRegionalConfiguration task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Identity", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			this.cmdletRunner.SetTaskParameter("VerifyDefaultFolderNameLanguage", task, new SwitchParameter(this.request.VerifyDefaultFolderNameLanguage));
		}

		protected override void PopulateResponseData(GetMailboxRegionalConfigurationResponse response)
		{
			MailboxRegionalConfiguration result = this.cmdletRunner.TaskWrapper.Result;
			response.Options = new GetMailboxRegionalConfigurationData
			{
				DateFormat = result.DateFormat,
				DefaultFolderNameMatchingUserLanguage = result.DefaultFolderNameMatchingUserLanguage,
				Language = result.Language.Name,
				TimeFormat = result.TimeFormat,
				TimeZone = ((result.TimeZone != null) ? result.TimeZone.ToString() : null)
			};
		}

		protected override PSLocalTask<GetMailboxRegionalConfiguration, MailboxRegionalConfiguration> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetMailboxRegionalConfigurationTask(base.CallContext.AccessingPrincipal);
		}
	}
}
