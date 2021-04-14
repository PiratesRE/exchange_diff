using System;
using System.Globalization;
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
	internal sealed class SetMailboxRegionalConfigurationCommand : SingleCmdletCommandBase<SetMailboxRegionalConfigurationRequest, SetMailboxRegionalConfigurationResponse, SetMailboxRegionalConfiguration, MailboxRegionalConfiguration>
	{
		public SetMailboxRegionalConfigurationCommand(CallContext callContext, SetMailboxRegionalConfigurationRequest request) : base(callContext, request, "Set-MailboxRegionalConfiguration", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			SetMailboxRegionalConfiguration task = this.cmdletRunner.TaskWrapper.Task;
			SetMailboxRegionalConfigurationData options = this.request.Options;
			this.cmdletRunner.SetTaskParameter("Identity", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			MailboxRegionalConfiguration taskParameters = (MailboxRegionalConfiguration)task.GetDynamicParameters();
			if (!string.IsNullOrEmpty(options.Language))
			{
				this.cmdletRunner.SetTaskParameterIfModified("Language", options, taskParameters, new CultureInfo(options.Language));
			}
			this.cmdletRunner.SetTaskParameterIfModified("LocalizeDefaultFolderName", options, task, new SwitchParameter(options.LocalizeDefaultFolderName));
			this.cmdletRunner.SetTaskParameterIfModified("TimeZone", options, taskParameters, (options.TimeZone != null) ? ExTimeZoneValue.Parse(options.TimeZone) : null);
			this.cmdletRunner.SetTaskParameterIfModified("DateFormat", options, taskParameters, options.DateFormat);
			this.cmdletRunner.SetTaskParameterIfModified("TimeFormat", options, taskParameters, options.TimeFormat);
		}

		protected override PSLocalTask<SetMailboxRegionalConfiguration, MailboxRegionalConfiguration> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSetMailboxRegionalConfigurationTask(base.CallContext.AccessingPrincipal);
		}
	}
}
